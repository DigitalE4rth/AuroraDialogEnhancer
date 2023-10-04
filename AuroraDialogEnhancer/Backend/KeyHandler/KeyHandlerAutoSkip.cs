using System;
using System.Linq;
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

        if (!_isAutoSkip || !_computerVisionService.IsDialogMode())
        {
            _isAutoSkip = false;
            return;
        }

        _isAutoSkipChoicePending = false;
        _cursorPositioningService.Hide();

        Task.Run(AutoSkipCycleTask).ConfigureAwait(false);
    }

    private Task AutoSkipCycleTask()
    {
        var taskCompletionSource = new TaskCompletionSource<bool>();
        while (_isAutoSkip)
        {
            if (IsAutoSkipCancellationRequired(taskCompletionSource)) break;
            if (!IsDialogOptionsPresent())
            {
                _skipClickDelegate!.Invoke();
                Task.Delay(_keyBindingProfile!.AutoSkip.Delay).Wait();
                continue;
            }

            if (IsAutoSkipCancellationRequired(taskCompletionSource)) break;
            if (_skipDialogDelegate!.Invoke()) continue;

            taskCompletionSource.SetResult(true);
            _isAutoSkip              = false;
            _isAutoSkipChoicePending = false;
            break;
        }

        return taskCompletionSource.Task;
    }

    private bool IsAutoSkipCancellationRequired(TaskCompletionSource<bool> taskCompletionSource)
    {
        if (_cursorVisibilityStateProvider.IsVisible() || _isAutoSkip) return false;

        _isAutoSkipChoicePending = false;
        _isAutoSkip = false;
        taskCompletionSource.SetResult(false);
        return true;
    }

    private void DoAutoSkipSingleClick()
    {
        _scriptHandlerService.DoAction(_keyBindingProfile!.AutoSkip.Id);
        Task.Delay(50).Wait();
    }

    private void DoAutoSkipDoubleClick()
    {
        _scriptHandlerService.DoAction(_keyBindingProfile!.AutoSkip.Id);
        Task.Delay(110).Wait();
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
