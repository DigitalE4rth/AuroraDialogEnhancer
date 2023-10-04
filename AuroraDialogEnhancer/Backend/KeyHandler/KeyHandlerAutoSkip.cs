using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;
using Cursor = System.Windows.Forms.Cursor;

namespace AuroraDialogEnhancer.Backend.KeyHandler;

public partial class KeyHandlerService
{
    private bool        _isAutoSkip;
    private bool        _isAutoSkipChoicePending;
    private Action?     _skipClickDelegate;
    private Func<bool>? _skipDialogDelegate;
    private CancellationTokenSource? _autoSkipCancelTokenSource;

    private void RegisterAutoSkip(AutoSkip autoSkip)
    {
        if (autoSkip.Delay == 0 || autoSkip.ActivationKeys.Count == 0) return;

        _skipClickDelegate  = _keyBindingProfile!.AutoSkip.IsDoubleClickRequired 
            ? DoAutoSkipDoubleClick 
            : DoAutoSkipSingleClick;

        _skipDialogDelegate = _keyBindingProfile.AutoSkip.AutoSkipType == EAutoSkipType.Everything 
            ? DoAutoSkipSkipEverything 
            : DoAutoSkipPartial;

        _scriptHandlerService.RegisterAction(autoSkip.Id, autoSkip.SkipKeys);
        Register(autoSkip.ActivationKeys, OnAutoSkip);
    }

    private void OnAutoSkip()
    {
        _isAutoSkip = !_isAutoSkip;

        if (!_isAutoSkip || !_computerVisionService.IsDialogMode()) return;

        _isAutoSkipChoicePending = false;
        _cursorPositioningService.Hide();

        Task.Run(AutoSkipCycleTask).ConfigureAwait(false);
    }

    private Task AutoSkipCycleTask()
    {
        var tcs = new TaskCompletionSource<bool>();
        _autoSkipCancelTokenSource?.Dispose();
        _autoSkipCancelTokenSource = new CancellationTokenSource();

        bool IsCancellationRequested()
        {
            if (!_autoSkipCancelTokenSource.Token.IsCancellationRequested) return false;

            tcs.SetResult(false);
            _autoSkipCancelTokenSource.Dispose();
            _autoSkipCancelTokenSource = null;
            _isAutoSkip = false;
            return true;
        }

        while (_isAutoSkip && !_autoSkipCancelTokenSource.Token.IsCancellationRequested)
        {
            if (!_cursorVisibilityStateProvider.IsVisible())
            {
                tcs.SetResult(false);
                _isAutoSkip = false;
                break;
            }

            if (IsCancellationRequested()) break;

            if (!IsDialogOptionsPresent())
            {
                _skipClickDelegate!.Invoke();
                Task.Delay(_keyBindingProfile!.AutoSkip.Delay).Wait();
                continue;
            }

            if (IsCancellationRequested()) break;

            if (_skipDialogDelegate!.Invoke()) continue;

            tcs.SetResult(true);
            _isAutoSkip = false;
            break;
        }

        return tcs.Task;
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
