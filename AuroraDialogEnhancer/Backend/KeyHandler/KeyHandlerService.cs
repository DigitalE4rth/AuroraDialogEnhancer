using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Keyboard;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using AuroraDialogEnhancer.Backend.Hooks.Process;
using AuroraDialogEnhancer.Backend.KeyBinding;
using AuroraDialogEnhancer.Backend.OpenCv;
using AuroraDialogEnhancer.Backend.ScreenCapture;
using AuroraDialogEnhancerExtensions.KeyBinding;
using Microsoft.Extensions.DependencyInjection;
using Cursor = System.Windows.Forms.Cursor;
using Point = System.Drawing.Point;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public class KeyHandlerService : IDisposable
{
    private readonly CursorPositioningService      _cursorPositioningService;
    private readonly CursorVisibilityStateProvider _cursorVisibilityStateProvider;
    private readonly HookedGameDataProvider        _hookedGameDataProvider;
    private readonly KeyBindingProfileService      _keyBindingProfileService;
    private readonly KeyboardHookManagerService    _keyboardHookManagerService;
    private readonly MouseEmulationService         _mouseEmulationService;
    private readonly MouseHookManagerService       _mouseHookManagerService;
    private readonly OpenCvService                 _openCvService;
    private readonly ScreenCaptureService          _screenCaptureService;
    private readonly WindowHookService             _windowHookService;

    private KeyBindingProfileDto? _keyBindingProfile;

    private List<Point> _currentDialogOptionsCoordinates;
    private bool        _isSpeakerNamePresent;
    private Point       _primaryDownPoint;
    private Point       _primaryUpPoint;

    private bool _isWindowFocused;
    private bool _isProcessing;
    private bool _isPrimarySuppressed;
    private bool _isPaused;

    private readonly object _focusLock;
    private readonly object _processingLock;
    private readonly object _mouseClickLock;

    public KeyHandlerService(CursorPositioningService      cursorPositioningService,
                             CursorVisibilityStateProvider cursorVisibilityStateProvider,
                             HookedGameDataProvider        hookedGameDataProvider,
                             KeyBindingProfileService      keyBindingProfileService,
                             KeyboardHookManagerService    keyboardHookManagerService,
                             MouseEmulationService         mouseEmulationService,
                             MouseHookManagerService       mouseHookManagerService,
                             OpenCvService                 openCvService,
                             ScreenCaptureService          screenCaptureService,
                             WindowHookService             windowHookService)
    {
        _cursorPositioningService      = cursorPositioningService;
        _cursorVisibilityStateProvider = cursorVisibilityStateProvider;
        _hookedGameDataProvider        = hookedGameDataProvider;
        _keyboardHookManagerService    = keyboardHookManagerService;
        _mouseEmulationService         = mouseEmulationService;
        _mouseHookManagerService       = mouseHookManagerService;
        _openCvService                 = openCvService;
        _keyBindingProfileService      = keyBindingProfileService;
        _screenCaptureService          = screenCaptureService;
        _windowHookService             = windowHookService;

        _currentDialogOptionsCoordinates = new List<Point>();
        _focusLock      = new object();
        _processingLock = new object();
        _mouseClickLock = new object();
    }

    #region Initializing
    public void ApplyKeyBinds()
    {
        StopPeripheryHook();
        _keyboardHookManagerService.UnRegisterAll();
        _mouseHookManagerService.UnRegisterAll();
        InitializeKeyBinds();
        StartPeripheryHook();
    }

    public void AttachFocusHook()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            _windowHookService.OnFocusChanged -= OnWindowFocusChanged;
            _windowHookService.OnFocusChanged += OnWindowFocusChanged;
        });
    }

    private void OnWindowFocusChanged(object sender, bool e)
    {
        lock (_focusLock)
        {
            if (_isWindowFocused == e) return;

            if (e)
            {
                StartPeripheryHook();
                _isWindowFocused = true;
                return;
            }

            _isWindowFocused = false;
            StopPeripheryHook();
        }
    }

    public void StartPeripheryHook()
    {
        _keyboardHookManagerService.Start();
        _mouseHookManagerService.Start();
    }

    public void StopPeripheryHook()
    {
        _keyboardHookManagerService.Stop();
        _mouseHookManagerService.Stop();
        _currentDialogOptionsCoordinates.Clear();
    }

    private void InitializeKeyBinds()
    {
        _keyBindingProfile = _keyBindingProfileService.Get(_hookedGameDataProvider.Data!.ExtensionConfig!.Id);
        RegisterKeyBinds();
    }

    private void RegisterKeyBinds()
    {
        Register(_keyBindingProfile!.PauseResume, OnPauseSwitch);
        Register(_keyBindingProfile.Reload,       OnReload);
        Register(_keyBindingProfile.Screenshot,   OnScreenshot);
        Register(_keyBindingProfile.HideCursor,   OnHideCursor);
        _cursorPositioningService.SetHiddenSetting(_keyBindingProfile.HiddenCursorSetting);

        Register(_keyBindingProfile.Select,          OnSelectPress);
        Register(_keyBindingProfile.Next,            OnNextPress);
        Register(_keyBindingProfile.Previous,        OnPreviousPress);
        Register(_keyBindingProfile.AutoDialog,      OnAutoDialog);
        Register(_keyBindingProfile.HideUi,          OnHideUi);
        Register(_keyBindingProfile.FullScreenPopUp, OnFullScreenPopUpClick);

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
    #endregion

    #region General action
    public void OnPauseSwitch()
    {
        if (_isPaused)
        {
            UnRegister(_keyBindingProfile!.PauseResume);
            RegisterKeyBinds();

            _hookedGameDataProvider.SetStateAndNotify(EHookState.Hooked);
            _isPaused = false;
            return;
        }

        UnRegisterAll();
        Register(_keyBindingProfile!.PauseResume, OnPauseSwitch);

        _hookedGameDataProvider.SetStateAndNotify(EHookState.Paused);
        _isPaused = true;
    }

    private void OnReload()
    {
        StopPeripheryHook();

        Task.Run(() => AppServices.ServiceProvider.GetRequiredService<CoreService>()
            .RestartAutoDetection(_hookedGameDataProvider.Data!.ExtensionConfig!.Id, true))
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
        if (!CanBeExecuted() || !IsDialogOptionsPresent()) return;

        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptionsCoordinates);

        ApplyRelativeCursorPosition(cursorPosition);

        if (HandleSingleNumericDialogOption(cursorPosition, selectedIndex)) return;

        HandleNumericSelection(cursorPosition, selectedIndex);

        _isProcessing = false;
    }

    private bool HandleSingleNumericDialogOption(DialogOptionCursorPositionInfo cursorPositionInfo, int selectedIndex)
    {
        if (_currentDialogOptionsCoordinates.Count != 1) return false;

        if (selectedIndex != 0)
        {
            HandleSingleDialogOption(cursorPositionInfo, cursorPositionInfo.HighlightedIndex != -1);
            _isProcessing = false;
            return true;
        }

        if (cursorPositionInfo.HighlightedIndex == -1)
        {
            Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptionsCoordinates[0]);
        }

        if (_keyBindingProfile!.NumericActionBehaviour != ENumericActionBehaviour.Select) return false;

        HandleSelectPress(true);

        _isProcessing = false;
        return true;
    }

    private void HandleNumericSelection(DialogOptionCursorPositionInfo cursorPositionInfo, int selectedIndex)
    {
        if (selectedIndex > _currentDialogOptionsCoordinates.Count - 1) return;

        Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptionsCoordinates[selectedIndex]);

        if (_keyBindingProfile!.NumericActionBehaviour == ENumericActionBehaviour.Select || cursorPositionInfo.HighlightedIndex == selectedIndex)
        {
            HandleSelectPress(true);
        }
    }
    #endregion

    #region Main
    private void OnNextPress()
    {
        if (!CanBeExecuted() || !IsDialogOptionsPresent()) return;

        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptionsCoordinates);

        ApplyRelativeCursorPosition(cursorPosition);

        if (HandleSingleDialogOption(cursorPosition)) return;

        if (HandleHighlightOnNothing(cursorPosition, 0, cursorPosition.ClosestLowerIndex)) return;

        if (HandleCycleThrough(_currentDialogOptionsCoordinates.Count - 1, cursorPosition.HighlightedIndex, 0)) return;

        HighlightNext(true, cursorPosition.HighlightedIndex);

        _isProcessing = false;
    }

    private void OnPreviousPress()
    {
        if (!CanBeExecuted() || !IsDialogOptionsPresent()) return;

        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptionsCoordinates);

        ApplyRelativeCursorPosition(cursorPosition);

        if (HandleSingleDialogOption(cursorPosition)) return;

        if (HandleHighlightOnNothing(cursorPosition, _currentDialogOptionsCoordinates.Count - 1, cursorPosition.ClosestUpperIndex)) return;

        if (HandleCycleThrough(0, cursorPosition.HighlightedIndex, _currentDialogOptionsCoordinates.Count - 1)) return;

        HighlightNext(false, cursorPosition.HighlightedIndex);

        _isProcessing = false;
    }

    private void OnSelectPress()
    {
        if (!CanBeExecuted() || !IsDialogOptionsPresent()) return;

        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptionsCoordinates);

        ApplyRelativeCursorPosition(cursorPosition);

        if (HandleSingleDialogOption(cursorPosition, true)) return;

        if (HandleHighlightOnNothing(cursorPosition)) return;

        HandleSelectPress(true);

        _isProcessing = false;
    }

    private void OnAutoDialog()
    {
        ClickCursor(_hookedGameDataProvider.Data!.CvPreset!.AutoSkipLocation);
    }

    private void OnHideUi()
    {
        ClickCursor(_hookedGameDataProvider.Data!.CvPreset!.HideUiLocation);
    }

    private void OnFullScreenPopUpClick()
    {
        ClickCursor(_hookedGameDataProvider.Data!.CvPreset!.FullScreenPopUpLocation);
    }

    private void ClickCursor(Point relativeLocation)
    {
        if (!CanBeExecuted()) return;

        var cursorPositionTemp = new Point(Cursor.Position.X, Cursor.Position.Y);
        Cursor.Position = _cursorPositioningService.GetAbsoluteFromRelativePoint(relativeLocation);

        _isPrimarySuppressed = true;
        _mouseEmulationService.DoMouseClick();
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
        if (!_currentDialogOptionsCoordinates.Any()) return;

        if (isArtificialClick)
        {
            var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptionsCoordinates);

            _cursorPositioningService.ApplyRelative(_currentDialogOptionsCoordinates[cursorPosition.HighlightedIndex]);
            _currentDialogOptionsCoordinates.Clear();

            _isPrimarySuppressed = true;
            _mouseEmulationService.DoMouseClick();
            Task.Delay(50).Wait();

            if (_keyBindingProfile!.CursorBehaviour == ECursorBehaviour.Nothing) return;
            _cursorPositioningService.Hide();

            return;
        }

        var upCursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptionsCoordinates, _primaryUpPoint);
        if (upCursorPosition.HighlightedIndex == -1) return;

        if (_primaryDownPoint != _primaryUpPoint)
        {
            var downCursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_currentDialogOptionsCoordinates, _primaryDownPoint);
            if (downCursorPosition.HighlightedIndex != upCursorPosition.HighlightedIndex) return;
        }

        _cursorPositioningService.ApplyRelative(_currentDialogOptionsCoordinates[upCursorPosition.HighlightedIndex]);
        _currentDialogOptionsCoordinates.Clear();

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
        
        if (_isWindowFocused && _cursorVisibilityStateProvider.IsVisible())
        {
            return true;
        }

        _isProcessing = false;
        return false;
    }

    private bool IsDialogOptionsPresent()
    {
        if (_currentDialogOptionsCoordinates.Any()) return true;

        (_isSpeakerNamePresent, _currentDialogOptionsCoordinates) = _openCvService.GetDialogOptionsCoordinates();

        if (_currentDialogOptionsCoordinates.Any()) return true;

        if (_isSpeakerNamePresent && _keyBindingProfile!.CursorBehaviour == ECursorBehaviour.Hide)
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
            _cursorPositioningService.ApplyRelative(_currentDialogOptionsCoordinates[cursorPositionInfo.HighlightedIndex]);
        }
        else
        {
            _cursorPositioningService.ApplyRelativeX(_currentDialogOptionsCoordinates[0]);
        }
    }

    private bool HandleSingleDialogOption(DialogOptionCursorPositionInfo cursorPositionInfo, bool isSelectAction = false)
    {
        if (_currentDialogOptionsCoordinates.Count != 1) return false;

        if (_keyBindingProfile!.SingleDialogOptionBehaviour == ESingleDialogOptionBehaviour.Highlight)
        {
            if (cursorPositionInfo.HighlightedIndex == -1)
            {
                Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptionsCoordinates[0]);
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
                Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptionsCoordinates[0]);
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
                ? _currentDialogOptionsCoordinates[targetIndex]
                : _currentDialogOptionsCoordinates[closestIndex])
            : _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptionsCoordinates[targetIndex]);

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

        Cursor.Position = _cursorPositioningService.GetRelatedNormalizedPoint(
            _currentDialogOptionsCoordinates[highlightedIndex],
            _currentDialogOptionsCoordinates[targetIndex]);

        _isProcessing = false;
        return true;
    }

    private void HighlightNext(bool direction, int highlightedIndex)
    {
        var indexedDirection = direction ? highlightedIndex + 1 : highlightedIndex - 1;
        Cursor.Position = _cursorPositioningService.GetRelatedNormalizedPoint(
            _currentDialogOptionsCoordinates[highlightedIndex],
            _currentDialogOptionsCoordinates[indexedDirection]);
    }
    #endregion
    #endregion

    public void Dispose()
    {
        _windowHookService.OnFocusChanged -= OnWindowFocusChanged;
        StopPeripheryHook();
    }
}
