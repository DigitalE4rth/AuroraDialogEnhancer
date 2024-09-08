using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.Backend.ComputerVision;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Keyboard;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using AuroraDialogEnhancer.Backend.KeyBinding;
using AuroraDialogEnhancer.Backend.KeyBinding.Models;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.InteractionPoints;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;
using AuroraDialogEnhancer.Backend.PeripheralEmulators;
using AuroraDialogEnhancer.Backend.ScreenCapture;
using AuroraDialogEnhancer.Backend.ScriptHandlers;
using Microsoft.Extensions.DependencyInjection;
using Cursor = System.Windows.Forms.Cursor;
using Point  = System.Drawing.Point;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public partial class KeyHandlerService : IDisposable
{
    private readonly CursorPositioningService      _cursorPositioningService;
    private readonly CursorVisibilityProvider      _cursorVisibilityProvider;
    private readonly ProcessDataProvider           _processDataProvider;
    private readonly KeyBindingProfileService      _keyBindingProfileService;
    private readonly KeyboardHookManagerService    _keyboardHookManagerService;
    private readonly MouseEmulationService         _mouseEmulationService;
    private readonly MouseHookManagerService       _mouseHookManagerService;
    private readonly ComputerVisionService         _computerVisionService;
    private readonly ScreenCaptureService          _screenCaptureService;
    private readonly ScriptHandlerService          _scriptHandlerService;

    private IGameFocusService _gameFocusService;
    private KeyBindingProfile?  _keyBindingProfile;

    private List<Rectangle> _currentDialogOptions;
    private Point           _primaryDownPoint;
    private Point           _primaryUpPoint;
    private Dictionary<string, Point> _interactionPoints;
    
    private bool _isProcessing;
    private bool _isPrimarySuppressed;
    private bool _isPaused;

    private readonly object _processingLock;
    private readonly object _mouseClickLock;

    public KeyHandlerService(ComputerVisionService         computerVisionService,
                             CursorPositioningService      cursorPositioningService,
                             FocusHookServiceFactory       focusHookServiceFactory,
                             CursorVisibilityProvider      cursorVisibilityProvider,
                             KeyBindingProfileService      keyBindingProfileService,
                             KeyboardHookManagerService    keyboardHookManagerService,
                             MouseEmulationService         mouseEmulationService,
                             MouseHookManagerService       mouseHookManagerService,
                             ProcessDataProvider           processDataProvider,
                             ScreenCaptureService          screenCaptureService,
                             ScriptHandlerService          scriptHandlerService)
    {
        _computerVisionService         = computerVisionService;
        _cursorPositioningService      = cursorPositioningService;
        _cursorVisibilityProvider      = cursorVisibilityProvider;
        _keyBindingProfileService      = keyBindingProfileService;
        _keyboardHookManagerService    = keyboardHookManagerService;
        _mouseEmulationService         = mouseEmulationService;
        _mouseHookManagerService       = mouseHookManagerService;
        _processDataProvider           = processDataProvider;
        _screenCaptureService          = screenCaptureService;
        _scriptHandlerService          = scriptHandlerService;
        _gameFocusService              = focusHookServiceFactory.Get();

        _interactionPoints    = new Dictionary<string, Point>(0);
        _currentDialogOptions = new List<Rectangle>(0);
        _processingLock = new object();
        _mouseClickLock = new object();
    }

    #region Initializing
    public void InitializeInteractionPoints(List<InteractionPrecisePoint> interactionPoints)
    {
        _interactionPoints = interactionPoints.ToDictionary(point => point.Id, point => point.Point);
    }

    public void ApplyKeyBinds()
    {
        StopPeripheryHook();
        _keyboardHookManagerService.UnRegisterAll();
        _mouseHookManagerService.UnRegisterAll();
        _scriptHandlerService.UnRegisterAll();
        InitializeKeyBinds();
        StartPeripheryHook();
    }

    public void AttachFocusHook(IGameFocusService gameFocusService)
    {
        _gameFocusService.OnFocusChanged -= GameFocusChanged;
        _gameFocusService = gameFocusService;
        _gameFocusService.OnFocusChanged += GameFocusChanged;
    }

    private void GameFocusChanged(object sender, bool state)
    {
        if (state)
        {
            StartPeripheryHook();
            return;
        }

        StopPeripheryHook();
    }

    private void StartPeripheryHook()
    {
        _keyboardHookManagerService.Start();
        _mouseHookManagerService.Start();
    }

    private void StopPeripheryHook()
    {
        StopScripts();
        _keyboardHookManagerService.Stop();
        _mouseHookManagerService.Stop();
        _currentDialogOptions.Clear();
    }

    private void StopScripts()
    {
        _isAutoSkip = false;
        _isAutoSkipReplyPending = false;
    }

    private void InitializeKeyBinds()
    {
        _keyBindingProfile = _keyBindingProfileService.Get(_processDataProvider.Data!.ExtensionConfig!.Id);
        RegisterKeyBinds();
    }

    private void RegisterKeyBinds()
    {
        Register(_keyBindingProfile!.PauseResume, OnPauseSwitch);
        Register(_keyBindingProfile.Reload,       OnReload);
        Register(_keyBindingProfile.Screenshot,   OnScreenshot);
        Register(_keyBindingProfile.HideCursor,   OnHideCursor);

        Register(_keyBindingProfile.Select,   OnSelectPress);
        Register(_keyBindingProfile.Next,     OnNextPress);
        Register(_keyBindingProfile.Previous, OnPreviousPress);
        Register(_keyBindingProfile.Last,     OnLastPress);

        Register(_keyBindingProfile.One,   OnOnePress);
        Register(_keyBindingProfile.Two,   OnTwoPress);
        Register(_keyBindingProfile.Three, OnThreePress);
        Register(_keyBindingProfile.Four,  OnFourPress);
        Register(_keyBindingProfile.Five,  OnFivePress);
        Register(_keyBindingProfile.Six,   OnSixPress);
        Register(_keyBindingProfile.Seven, OnSevenPress);
        Register(_keyBindingProfile.Eight, OnEightPress);
        Register(_keyBindingProfile.Nine,  OnNinePress);
        Register(_keyBindingProfile.Ten,   OnTenPress);
        Register(_keyBindingProfile.InteractionPoints);

        RegisterAutoSkip(_keyBindingProfile.AutoSkipConfig);

        _mouseHookManagerService.RegisterPrimaryClick(OnMousePrimaryClick);
    }

    private void Register(List<List<GenericKey>> keysOfTriggers, Action action)
    {
        foreach (var genericKeys in keysOfTriggers)
        {
            if (genericKeys.Count == 1 && genericKeys[0].GetType() == typeof(MouseKey))
            {
                _mouseHookManagerService.RegisterHotKey(genericKeys[0].KeyCode, action);
                continue;
            }

            _keyboardHookManagerService.RegisterHotKeys(genericKeys.Select(key => key.KeyCode), action);
        }
    }

    private void Register(List<InteractionPoint> interactionPoints)
    {
        foreach (var interactionPoint in interactionPoints)
        {
            foreach (var activationKeys in interactionPoint.ActivationKeys)
            {
                if (activationKeys.Count == 1 && activationKeys[0].GetType() == typeof(MouseKey))
                {
                    _mouseHookManagerService.RegisterHotKey(
                        activationKeys[0].KeyCode, 
                        () => { ClickCursor(_interactionPoints[interactionPoint.Id]); });

                    continue;
                }

                _keyboardHookManagerService.RegisterHotKeys(
                    activationKeys.Select(key => key.KeyCode), 
                    () => { ClickCursor(_interactionPoints[interactionPoint.Id]); });
            }
        }
    }

    private void UnRegisterAll()
    {
        _mouseHookManagerService.UnRegisterAll();
        _keyboardHookManagerService.UnRegisterAll();
    }

    private void UnRegister(List<List<GenericKey>> keysOfTriggers)
    {
        foreach (var genericKeys in keysOfTriggers)
        {
            if (genericKeys.Count == 1 && genericKeys[0].GetType() == typeof(MouseKey))
            {
                _mouseHookManagerService.UnRegisterHotKey(genericKeys[0].KeyCode);
                continue;
            }

            _keyboardHookManagerService.UnRegisterHotKeys(genericKeys.Select(key => key.KeyCode));
        }
    }

    public void HideCursorOnReload()
    {
        if (!CanBeExecuted() || !_computerVisionService.IsDialogMode())
        {
            _isProcessing = false;
            return;
        }
        _cursorPositioningService.Hide();
        _isProcessing = false;
    }
    #endregion

    #region General action
    public void OnPauseSwitch()
    {
        if (_isPaused)
        {
            StopScripts();
            UnRegister(_keyBindingProfile!.PauseResume);
            RegisterKeyBinds();

            _processDataProvider.SetStateAndNotify(EHookState.Hooked);
            _isPaused = false;
            return;
        }

        UnRegisterAll();
        Register(_keyBindingProfile!.PauseResume, OnPauseSwitch);

        _processDataProvider.SetStateAndNotify(EHookState.Paused);
        _isPaused = true;
    }

    private void OnReload()
    {
        StopPeripheryHook();

        Task.Run(() => AppServices.ServiceProvider.GetRequiredService<CoreService>()
            .RestartAutoDetection(_processDataProvider.Data!.ExtensionConfig!.Id, true))
            .ConfigureAwait(false);
    }

    private void OnScreenshot()
    {
        _screenCaptureService.CaptureAndSave();
    }

    private void OnHideCursor()
    {
        if (!CanBeExecuted()) return;
        _cursorPositioningService.Hide();
        _isProcessing = false;
    }
    #endregion

    #region Control actions
    #region Numeric
    private void OnOnePress()   => OnNumericPress(0);
    private void OnTwoPress()   => OnNumericPress(1);
    private void OnThreePress() => OnNumericPress(2);
    private void OnFourPress()  => OnNumericPress(3);
    private void OnFivePress()  => OnNumericPress(4);
    private void OnSixPress()   => OnNumericPress(5);
    private void OnSevenPress() => OnNumericPress(6);
    private void OnEightPress() => OnNumericPress(7);
    private void OnNinePress()  => OnNumericPress(8);
    private void OnTenPress()   => OnNumericPress(9);

    private void OnNumericPress(int selectedIndex)
    {
        if (IsLockedByAutoSkip() ||
            !CanBeExecuted() ||
            !AreDialogOptionsPresent()) return;

        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptions);

        ApplyRelativeCursorPosition(cursorPosition);

        if (HandleSingleNumericDialogOption(cursorPosition, selectedIndex)) return;

        HandleNumericSelection(cursorPosition, selectedIndex);

        _isProcessing = false;
    }

    private bool HandleSingleNumericDialogOption(DialogOptionCursorPositionInfo cursorPositionInfo, int selectedIndex)
    {
        if (_currentDialogOptions.Count != 1) return false;

        if (selectedIndex != 0)
        {
            HandleSingleDialogOption(cursorPositionInfo, cursorPositionInfo.HighlightedIndex != -1);
            _isProcessing = false;
            return true;
        }

        if (cursorPositionInfo.HighlightedIndex == -1)
        {
            Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptions[0]);
        }

        if (_keyBindingProfile!.NumericActionBehaviour != ENumericActionBehaviour.Select) return false;

        HandleSelectPress(true);

        _isProcessing = false;
        return true;
    }

    private void HandleNumericSelection(DialogOptionCursorPositionInfo cursorPositionInfo, int selectedIndex)
    {
        if (selectedIndex > _currentDialogOptions.Count - 1) return;

        var cursorDialogOptionPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptions);
        var newCursorPosition = _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptions[selectedIndex]);

        if (cursorDialogOptionPosition.ClosestLowerIndex == selectedIndex ||
            cursorDialogOptionPosition.ClosestUpperIndex == selectedIndex)
        {
            _cursorPositioningService.SetCursorPositionWithAnimation(newCursorPosition);
        }
        else
        {
            Cursor.Position = newCursorPosition;
        }

        if (_keyBindingProfile!.NumericActionBehaviour == ENumericActionBehaviour.Select || cursorPositionInfo.HighlightedIndex == selectedIndex)
        {
            HandleSelectPress(true);
        }
    }
    #endregion

    #region Main
    private void OnNextPress()
    {
        if (IsLockedByAutoSkip() ||
            !CanBeExecuted() ||
            !AreDialogOptionsPresent()) return;

        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptions);

        ApplyRelativeCursorPosition(cursorPosition);

        if (HandleSingleDialogOption(cursorPosition)) return;

        if (HandleHighlightOnNothing(cursorPosition, 0, cursorPosition.ClosestLowerIndex)) return;

        if (HandleCycleThrough(_currentDialogOptions.Count - 1, cursorPosition.HighlightedIndex, 0)) return;

        HighlightNext(true, cursorPosition.HighlightedIndex);

        _isProcessing = false;
    }

    private void OnPreviousPress()
    {
        if (IsLockedByAutoSkip() ||
            !CanBeExecuted() ||
            !AreDialogOptionsPresent()) return;

        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptions);

        ApplyRelativeCursorPosition(cursorPosition);

        if (HandleSingleDialogOption(cursorPosition)) return;

        if (HandleHighlightOnNothing(cursorPosition, _currentDialogOptions.Count - 1, cursorPosition.ClosestUpperIndex)) return;

        if (HandleCycleThrough(0, cursorPosition.HighlightedIndex, _currentDialogOptions.Count - 1)) return;

        HighlightNext(false, cursorPosition.HighlightedIndex);

        _isProcessing = false;
    }

    private void OnSelectPress()
    {
        if (IsLockedByAutoSkip() ||
            !CanBeExecuted() ||
            !AreDialogOptionsPresent()) return;

        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptions);

        ApplyRelativeCursorPosition(cursorPosition);

        if (HandleSingleDialogOption(cursorPosition, true)) return;

        if (HandleHighlightOnNothing(cursorPosition)) return;

        HandleSelectPress(true);

        _isProcessing = false;
    }

    private void OnLastPress()
    {
        if (IsLockedByAutoSkip() ||
            !CanBeExecuted() ||
            !AreDialogOptionsPresent()) return;

        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptions);

        ApplyRelativeCursorPosition(cursorPosition);

        var position = _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptions.Last());
        if (cursorPosition.ClosestLowerIndex != -1 &&
            cursorPosition.ClosestUpperIndex != -1 &&
            cursorPosition.ClosestLowerIndex == _currentDialogOptions.Count - 1)
        {
            _cursorPositioningService.SetCursorPositionWithAnimation(position);
        }
        else
        {
            Cursor.Position = position;
        }

        HandleSelectPress(true);

        _isProcessing = false;
    }

    private void ClickCursor(Point relativeLocation)
    {
        if (!CanBeExecuted()) return;

        var cursorPositionTemp = new Point(Cursor.Position.X, Cursor.Position.Y);
        Cursor.Position = _cursorPositioningService.GetAbsoluteFromRelativePoint(relativeLocation);

        _isPrimarySuppressed = true;
        _mouseEmulationService.DoPrimaryClick();
        Task.Delay(50).Wait();
        Cursor.Position = cursorPositionTemp;

        _isProcessing = false;
    }

    private void OnMousePrimaryClick(bool isPrimaryDown, Point clickedPoint)
    {
        lock (_mouseClickLock)
        {
            if (!CanBeExecuted()) return;

            if (isPrimaryDown)
            {
                if (_isPrimarySuppressed)
                {
                    _isProcessing = false;
                    return;
                }
                _primaryDownPoint = clickedPoint;
                _isProcessing     = false;
                return;
            }

            if (_isPrimarySuppressed)
            {
                _isPrimarySuppressed = false;
                _isProcessing        = false;
                return;
            }
            _primaryUpPoint = clickedPoint;

            HandleSelectPress(false);
            _isProcessing = false;
        }
    }

    private void HandleSelectPress(bool isArtificialClick)
    {
        if (!_currentDialogOptions.Any()) return;

        if (isArtificialClick)
        {
            var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptions);
            if (cursorPosition.HighlightedIndex == -1)
            {
                _currentDialogOptions.Clear();
                return;
            }
            
            _cursorPositioningService.ApplyRelative(_currentDialogOptions[cursorPosition.HighlightedIndex]);
            _currentDialogOptions.Clear();

            _isPrimarySuppressed = true;
            _mouseEmulationService.DoPrimaryClick();
            Task.Delay(50).Wait();

            if (_isAutoSkipReplyPending)
            {
                RunAutoSkip(true);
                return;
            }

            if (_keyBindingProfile!.CursorBehaviour == ECursorBehaviour.Nothing) return;
            _cursorPositioningService.Hide();

            return;
        }

        var upCursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptions, _primaryUpPoint);
        if (upCursorPosition.HighlightedIndex == -1) return;

        if (_primaryDownPoint != _primaryUpPoint)
        {
            var downCursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptions, _primaryDownPoint);
            if (downCursorPosition.HighlightedIndex != upCursorPosition.HighlightedIndex) return;
        }

        _cursorPositioningService.ApplyRelative(_currentDialogOptions[upCursorPosition.HighlightedIndex]);
        _currentDialogOptions.Clear();

        if (_isAutoSkipReplyPending)
        {
            RunAutoSkip(true);
            return;
        }

        if (_keyBindingProfile!.CursorBehaviour == ECursorBehaviour.Nothing || !_keyBindingProfile.IsCursorHideOnManualClick) return;
        Task.Delay(50).Wait();
        _cursorPositioningService.Hide();
    }
    #endregion

    #region Utils
    private bool CanBeExecuted()
    {
        lock (_processingLock)
        {
            if (_isProcessing)
            {
                return false;
            }
            _isProcessing = true;
        }
        
        if (_gameFocusService.IsFocused &&
            _cursorVisibilityProvider.IsVisible() &&
            _cursorPositioningService.IsCursorInsideClient())
        {
            return true;
        }

        _isProcessing = false;
        return false;
    }

    private bool AreDialogOptionsPresent()
    {
        if (_currentDialogOptions.Any()) return true;

        if (_computerVisionService.IsDialogMode() == false)
        {
            _isProcessing = false;
            return false;
        }
        
        _currentDialogOptions = _computerVisionService.GetDialogOptions();

        if (_currentDialogOptions.Any()) return true;
        
        if (!_isAutoSkip &&
            _keyBindingProfile!.CursorBehaviour == ECursorBehaviour.Hide)
        {
            _cursorPositioningService.Hide();
        }

        _isProcessing = false;
        return false;
    }

    private void ApplyRelativeCursorPosition(DialogOptionCursorPositionInfo cursorPositionInfo)
    {
        if (!cursorPositionInfo.IsWithinBoundaries) return;

        if (cursorPositionInfo.HighlightedIndex != -1)
        {
            _cursorPositioningService.ApplyRelative(_currentDialogOptions[cursorPositionInfo.HighlightedIndex]);
        }
        else
        {
            _cursorPositioningService.ApplyRelativeX(_currentDialogOptions[0]);
        }
    }

    private bool HandleSingleDialogOption(DialogOptionCursorPositionInfo cursorPositionInfo, bool isSelectAction = false)
    {
        if (_currentDialogOptions.Count != 1) return false;

        if (_keyBindingProfile!.SingleDialogOptionBehaviour == ESingleDialogOptionBehaviour.Highlight)
        {
            if (cursorPositionInfo.HighlightedIndex == -1)
            {
                Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptions[0]);

                _isProcessing = false;
                return true;
            }

            if (isSelectAction)
            {
                HandleSelectPress(true);
            }

            _isProcessing = false;
            return true;
        }

        if (_keyBindingProfile.SingleDialogOptionBehaviour == ESingleDialogOptionBehaviour.Select)
        {
            if (cursorPositionInfo.HighlightedIndex == -1)
            {
                Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptions[0]);
            }

            HandleSelectPress(true);
            _isProcessing = false;
            return true;
        }

        return false;
    }

    private bool HandleHighlightOnNothing(DialogOptionCursorPositionInfo cursorPositionInfo, int targetIndex = 0, int closestIndex = 0)
    {
        if (cursorPositionInfo.HighlightedIndex != -1) return false;

        Cursor.Position = cursorPositionInfo.IsWithinBoundaries
            ? _cursorPositioningService.GetTargetCursorPlacement(closestIndex == -1
                ? _currentDialogOptions[targetIndex]
                : _currentDialogOptions[closestIndex])
            : _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptions[targetIndex]);

        _isProcessing = false;
        return true;
    }

    private bool HandleCycleThrough(int boundaryIndex, int highlightedIndex, int targetIndex)
    {
        if (highlightedIndex != boundaryIndex) return false;

        if (!_keyBindingProfile!.IsCycleThrough)
        {
            _isProcessing = false;
            return true;
        }

        Cursor.Position = _cursorPositioningService.GetRelatedNormalizedPoint(_currentDialogOptions[targetIndex]);

        _isProcessing = false;
        return true;
    }

    private void HighlightNext(bool direction, int highlightedIndex)
    {
        var indexedDirection = direction ? highlightedIndex + 1 : highlightedIndex - 1;
        var position = _cursorPositioningService.GetRelatedNormalizedPoint(_currentDialogOptions[indexedDirection]);
        _cursorPositioningService.SetCursorPositionWithAnimation(position);
    }
    #endregion
    #endregion

    public void Dispose()
    {
        _gameFocusService.OnFocusChanged -= GameFocusChanged;
        _gameFocusService.UnhookWinEvent();
        
        _interactionPoints.Clear();
        _scriptHandlerService.Dispose();
        StopPeripheryHook();
        _autoSkipCts?.Dispose();
    }
}
