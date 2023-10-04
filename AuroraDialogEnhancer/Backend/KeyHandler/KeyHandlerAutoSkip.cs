using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;
using Cursor = System.Windows.Forms.Cursor;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public partial class KeyHandlerService
{
    private bool _isAutoSkip;
    private bool _isAutoSkipChoicePending;

    private void RegisterAutoSkip(AutoSkip autoSkip)
    {
        if (autoSkip.Delay == 0 || autoSkip.ActivationKeys.Count == 0) return;
        _scriptHandlerService.RegisterAction(autoSkip.Id, autoSkip.SkipKeys);
        Register(autoSkip.ActivationKeys, OnAutoSkip);
    }

    private void OnAutoSkip()
    {
        _isAutoSkip = !_isAutoSkip;

        if (!_isAutoSkip || !_computerVisionService.IsDialogMode()) return;

        _isAutoSkipChoicePending      = false;
        Action skipClickDelegate      = _keyBindingProfile!.AutoSkip.IsDoubleClickRequired ? DoAutoSkipDoubleClick : DoAutoSkipSingleClick;
        Func<bool> skipDialogDelegate = _keyBindingProfile.AutoSkip.AutoSkipType == EAutoSkipType.Everything ? DoAutoSkipSkipEverything : DoAutoSkipPartial;

        _cursorPositioningService.Hide();

        while (_isAutoSkip)
        {
            if (!_cursorVisibilityStateProvider.IsVisible())
            {
                _isAutoSkip = false;
                return;
            }

            if (!IsDialogOptionsPresent())
            {
                skipClickDelegate.Invoke();
                Task.Delay(_keyBindingProfile.AutoSkip.Delay).Wait();
                continue;
            }

            var shouldContinue = skipDialogDelegate.Invoke();
            if (shouldContinue) continue;

            _isAutoSkip = false;
            return;
        }
    }

    private void DoAutoSkipSingleClick()
    {
        _scriptHandlerService.DoAction(_keyBindingProfile!.AutoSkip.Id);
        Task.Delay(50).Wait();
    }

    private void DoAutoSkipDoubleClick()
    {
        _scriptHandlerService.DoAction(_keyBindingProfile!.AutoSkip.Id);
        Task.Delay(50).Wait();
        _scriptHandlerService.DoAction(_keyBindingProfile.AutoSkip.Id);
    }

    private bool DoAutoSkipPartial()
    {
        _isAutoSkipChoicePending = true;
        return false;
    }

    private bool DoAutoSkipSkipEverything()
    {
        Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptions.Last());
        HandleSelectPress(true);
        _cursorPositioningService.Hide();
        return true;
    }
}
