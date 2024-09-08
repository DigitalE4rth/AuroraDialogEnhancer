using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using AuroraDialogEnhancer.AppConfig.DependencyInjection;
using AuroraDialogEnhancer.Backend.ComputerVision;
using AuroraDialogEnhancer.Backend.Core;
using AuroraDialogEnhancer.Backend.CursorPositioning;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;
using AuroraDialogEnhancer.Backend.KeyHandler.Scripts;
using AuroraDialogEnhancer.Backend.PeripheralEmulators;
using AuroraDialogEnhancer.Backend.ScreenCapture;
using Microsoft.Extensions.DependencyInjection;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public class KeyActionExecution
{
    private readonly ComputerVisionService    _computerVisionService;
    private readonly CursorPositioningService _cursorPositioningService;
    private readonly KeyActionAccessibility   _keyActionAccessibility;
    private readonly KeyActionUtility         _keyActionUtility;
    private readonly MouseEmulationService    _mouseEmulationService;
    private readonly ProcessDataProvider      _processDataProvider;
    private readonly ScreenCaptureService     _screenCaptureService;
    private readonly ScriptAutoSkipUtilities  _scriptAutoSkipUtilities;

    #region General action
    public KeyActionExecution(ComputerVisionService    computerVisionService,
                              CursorPositioningService cursorPositioningService,
                              KeyActionAccessibility   keyActionAccessibility,
                              KeyActionUtility         keyActionUtility,
                              MouseEmulationService    mouseEmulationService,
                              ProcessDataProvider      processDataProvider,
                              ScreenCaptureService     screenCaptureService,
                              ScriptAutoSkipUtilities  scriptAutoSkipUtilities)
    {
        _computerVisionService    = computerVisionService;
        _cursorPositioningService = cursorPositioningService;
        _keyActionAccessibility   = keyActionAccessibility;
        _keyActionUtility         = keyActionUtility;
        _mouseEmulationService    = mouseEmulationService;
        _processDataProvider      = processDataProvider;
        _screenCaptureService     = screenCaptureService;
        _scriptAutoSkipUtilities  = scriptAutoSkipUtilities;
    }

    public void HideCursorOnReload() => _keyActionAccessibility.WithAccess(() =>
    {
        if (!_computerVisionService.IsDialogMode()) return;
        _cursorPositioningService.Hide();
    });

    public void OnReload()
    {
        Task.Run(() => AppServices.ServiceProvider.GetRequiredService<CoreService>()
            .RestartAutoDetection(_processDataProvider.Data!.ExtensionConfig!.Id, true))
            .ConfigureAwait(false);
    }

    public void OnScreenshot()
    {
        _screenCaptureService.CaptureAndSave();
    }

    public void OnHideCursor()
    {
        _keyActionAccessibility.WithAccess(() => _cursorPositioningService.Hide());
    }

    public void ClickPrimaryMouseButton(Point relativePoint)
    {
        var cursorPositionTemp = new Point(Cursor.Position.X, Cursor.Position.Y);
        Cursor.Position = _cursorPositioningService.GetAbsoluteFromRelativePoint(relativePoint);

        _keyActionUtility.IsPrimaryMouseButtonSuspended = true;
        _mouseEmulationService.DoPrimaryClick();
        Task.Delay(50).Wait();
        Cursor.Position = cursorPositionTemp;
    }

    public void ClickPrimaryMouseButtonWithAccess(Point relativePoint) => _keyActionAccessibility.WithAccess(() => ClickPrimaryMouseButton(relativePoint));
    #endregion

    #region Numeric
    public void OnOnePress()   => OnNumericPress(0);
    public void OnTwoPress()   => OnNumericPress(1);
    public void OnThreePress() => OnNumericPress(2);
    public void OnFourPress()  => OnNumericPress(3);
    public void OnFivePress()  => OnNumericPress(4);
    public void OnSixPress()   => OnNumericPress(5);
    public void OnSevenPress() => OnNumericPress(6);
    public void OnEightPress() => OnNumericPress(7);
    public void OnNinePress()  => OnNumericPress(8);
    public void OnTenPress()   => OnNumericPress(9);

    private void OnNumericPress(int selectedIndex) => _keyActionAccessibility.WithExecution(() =>
    {
        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_keyActionUtility.DialogOptions);
        _cursorPositioningService.ApplyRelativeCursorPosition(cursorPosition, _keyActionUtility.DialogOptions);

        if (HandleSingleNumericDialogOption(cursorPosition, selectedIndex)) return;
        HandleNumericSelection(cursorPosition, selectedIndex);
    });

    private bool HandleSingleNumericDialogOption(CursorPositionInfo cursorPositionInfo, int selectedIndex)
    {
        if (_keyActionUtility.DialogOptions.Count != 1) return false;

        if (selectedIndex != 0)
        {
            HandleSingleDialogOption(cursorPositionInfo, cursorPositionInfo.HighlightedIndex != -1);
            return true;
        }

        if (cursorPositionInfo.HighlightedIndex == -1)
        {
            Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_keyActionUtility.DialogOptions[0]);
        }

        if (_keyActionUtility.KeyBindingProfile.NumericActionBehaviour != ENumericActionBehaviour.Select) return false;

        HandleSelectPressArtificial();

        return true;
    }

    private void HandleNumericSelection(CursorPositionInfo cursorPositionInfo, int selectedIndex)
    {
        if (selectedIndex > _keyActionUtility.DialogOptions.Count - 1) return;

        var cursorDialogOptionPosition = _cursorPositioningService.GetPositionByDialogOptions(_keyActionUtility.DialogOptions);
        var newCursorPosition = _cursorPositioningService.GetTargetCursorPlacement(_keyActionUtility.DialogOptions[selectedIndex]);

        if (cursorDialogOptionPosition.ClosestLowerIndex == selectedIndex || cursorDialogOptionPosition.ClosestUpperIndex == selectedIndex)
        {
            _cursorPositioningService.SetCursorPositionWithAnimation(newCursorPosition);
        }
        else
        {
            Cursor.Position = newCursorPosition;
        }

        if (_keyActionUtility.KeyBindingProfile.NumericActionBehaviour == ENumericActionBehaviour.Select || cursorPositionInfo.HighlightedIndex == selectedIndex)
        {
            HandleSelectPressArtificial();
        }
    }
    #endregion

    #region General
    public void OnSelectPress() => _keyActionAccessibility.WithExecution(() =>
    {
        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_keyActionUtility.DialogOptions);
        _cursorPositioningService.ApplyRelativeCursorPosition(cursorPosition, _keyActionUtility.DialogOptions);

        if (HandleSingleDialogOption(cursorPosition, true)) return;
        if (HandleHighlightOnNothing(cursorPosition)) return;
        HandleSelectPressArtificial();
    });

    public void OnNextPress() => _keyActionAccessibility.WithExecution(() =>
    {
        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_keyActionUtility.DialogOptions);

        _cursorPositioningService.ApplyRelativeCursorPosition(cursorPosition, _keyActionUtility.DialogOptions);
        if (HandleSingleDialogOption(cursorPosition)) return;
        if (HandleHighlightOnNothing(cursorPosition, 0, cursorPosition.ClosestLowerIndex)) return;
        if (HandleCycleThrough(_keyActionUtility.DialogOptions.Count - 1, cursorPosition.HighlightedIndex, 0)) return;

        HighlightNext(true, cursorPosition.HighlightedIndex);
    });
    
    public void OnPreviousPress() => _keyActionAccessibility.WithExecution(() =>
    {
        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_keyActionUtility.DialogOptions);

        _cursorPositioningService.ApplyRelativeCursorPosition(cursorPosition, _keyActionUtility.DialogOptions);
        if (HandleSingleDialogOption(cursorPosition)) return;
        if (HandleHighlightOnNothing(cursorPosition, _keyActionUtility.DialogOptions.Count - 1, cursorPosition.ClosestUpperIndex)) return;
        if (HandleCycleThrough(0, cursorPosition.HighlightedIndex, _keyActionUtility.DialogOptions.Count - 1)) return;

        HighlightNext(false, cursorPosition.HighlightedIndex);
    });

    public void OnLastPress() => _keyActionAccessibility.WithExecution(() =>
    {
        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_keyActionUtility.DialogOptions);

        _cursorPositioningService.ApplyRelativeCursorPosition(cursorPosition, _keyActionUtility.DialogOptions);

        var position = _cursorPositioningService.GetTargetCursorPlacement(_keyActionUtility.DialogOptions.Last());
        if (cursorPosition.ClosestLowerIndex != -1 &&
            cursorPosition.ClosestUpperIndex != -1 &&
            cursorPosition.ClosestLowerIndex == _keyActionUtility.DialogOptions.Count - 1)
        {
            _cursorPositioningService.SetCursorPositionWithAnimation(position);
            HandleSelectPressArtificial();
            return;
        }
        
        Cursor.Position = position;
        HandleSelectPressArtificial();
    });
    #endregion

    #region Utility
    public void OnMousePrimaryClick(bool isPrimaryDown, Point clickedPoint) => _keyActionAccessibility.WithAccess(() => 
    {
        lock (_keyActionUtility.MouseClickLock)
        {
            if (isPrimaryDown)
            {
                if (_keyActionUtility.IsPrimaryMouseButtonSuspended) return;
                _keyActionUtility.PrimaryMouseButtonDownPoint = clickedPoint;
                return;
            }

            if (_keyActionUtility.IsPrimaryMouseButtonSuspended)
            {
                _keyActionUtility.IsPrimaryMouseButtonSuspended = false;
                return;
            }
            _keyActionUtility.PrimaryMouseButtonUpPoint = clickedPoint;

            HandleSelectPress();
        }
    });

    public void HandleSelectPress()
    {
        if (!_keyActionAccessibility.AreDialogOptionsPresent()) return;
        
        var upCursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_keyActionUtility.DialogOptions, _keyActionUtility.PrimaryMouseButtonUpPoint);
        if (upCursorPosition.HighlightedIndex == -1) return;

        if (_keyActionUtility.PrimaryMouseButtonDownPoint != _keyActionUtility.PrimaryMouseButtonUpPoint)
        {
            var downCursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_keyActionUtility.DialogOptions, _keyActionUtility.PrimaryMouseButtonDownPoint);
            if (downCursorPosition.HighlightedIndex != upCursorPosition.HighlightedIndex) return;
        }

        _cursorPositioningService.ApplyRelative(_keyActionUtility.DialogOptions[upCursorPosition.HighlightedIndex]);
        _keyActionUtility.DialogOptions.Clear();

        if (_scriptAutoSkipUtilities.IsReplyPending)
        {
            _scriptAutoSkipUtilities.RestartDelegate!.Invoke();
            return;
        }

        if (_keyActionUtility.KeyBindingProfile.CursorBehaviour == ECursorBehaviour.Nothing || !_keyActionUtility.KeyBindingProfile.IsCursorHideOnManualClick) return;
        Task.Delay(50).Wait();
        _cursorPositioningService.Hide();
    }

    public void HandleSelectPressArtificial()
    {
        if (!_keyActionUtility.DialogOptions.Any()) return;

        var cursorPosition = _cursorPositioningService.GetPositionByDialogOptions(_keyActionUtility.DialogOptions);
        if (cursorPosition.HighlightedIndex == -1)
        {
            _keyActionUtility.DialogOptions.Clear();
            return;
        }

        _cursorPositioningService.ApplyRelative(_keyActionUtility.DialogOptions[cursorPosition.HighlightedIndex]);
        _keyActionUtility.DialogOptions.Clear();

        _keyActionUtility.IsPrimaryMouseButtonSuspended = true;
        _mouseEmulationService.DoPrimaryClick();
        Task.Delay(50).Wait();

        if (_scriptAutoSkipUtilities.IsReplyPending)
        {
            _scriptAutoSkipUtilities.RestartDelegate!.Invoke();
            return;
        }

        if (_keyActionUtility.KeyBindingProfile.CursorBehaviour == ECursorBehaviour.Nothing) return;
        _cursorPositioningService.Hide();
    }

    private bool HandleSingleDialogOption(CursorPositionInfo cursorPositionInfo, bool isSelectAction = false)
    {
        if (_keyActionUtility.DialogOptions.Count != 1) return false;

        switch (_keyActionUtility.KeyBindingProfile.SingleDialogOptionBehaviour)
        {
            case ESingleDialogOptionBehaviour.Highlight when cursorPositionInfo.HighlightedIndex == -1:
                Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_keyActionUtility.DialogOptions[0]);
                return true;
            case ESingleDialogOptionBehaviour.Highlight:
            {
                if (isSelectAction) HandleSelectPressArtificial();
                return true;
            }
            case ESingleDialogOptionBehaviour.Select:
            {
                if (cursorPositionInfo.HighlightedIndex == -1) Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_keyActionUtility.DialogOptions[0]);
                HandleSelectPressArtificial();
                return true;
            }
            default:
                return false;
        }
    }

    private bool HandleHighlightOnNothing(CursorPositionInfo cursorPositionInfo, int targetIndex = 0, int closestIndex = 0)
    {
        if (cursorPositionInfo.HighlightedIndex != -1) return false;

        Cursor.Position = cursorPositionInfo.IsWithinBoundaries
            ? _cursorPositioningService.GetTargetCursorPlacement(closestIndex == -1
                ? _keyActionUtility.DialogOptions[targetIndex]
                : _keyActionUtility.DialogOptions[closestIndex])
            : _cursorPositioningService.GetTargetCursorPlacement(_keyActionUtility.DialogOptions[targetIndex]);

        return true;
    }

    private bool HandleCycleThrough(int boundaryIndex, int highlightedIndex, int targetIndex)
    {
        if (highlightedIndex != boundaryIndex) return false;
        if (!_keyActionUtility.KeyBindingProfile.IsCycleThrough) return true;

        Cursor.Position = _cursorPositioningService.GetRelatedNormalizedPoint(_keyActionUtility.DialogOptions[targetIndex]);
        return true;
    }

    private void HighlightNext(bool direction, int highlightedIndex)
    {
        var indexedDirection = direction ? highlightedIndex + 1 : highlightedIndex - 1;
        var position = _cursorPositioningService.GetRelatedNormalizedPoint(_keyActionUtility.DialogOptions[indexedDirection]);
        _cursorPositioningService.SetCursorPositionWithAnimation(position);
    }
    #endregion
}
