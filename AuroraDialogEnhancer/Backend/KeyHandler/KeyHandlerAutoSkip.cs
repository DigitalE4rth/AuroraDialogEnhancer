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
    private Func<bool>? _skipStartDelegate;
    private Func<bool>? _skipDialogDelegate;

    private void RegisterAutoSkip(AutoSkipConfig autoSkipConfig)
    {
        _skipClickDelegate = _keyBindingProfile!.AutoSkipConfig.IsDoubleClickDelay
            ? DoAutoSkipDoubleClick
            : DoAutoSkipSingleClick;

        _skipDialogDelegate = _keyBindingProfile.AutoSkipConfig.SkipMode == ESkipMode.Everything 
            ? DoAutoSkipSkipEverything 
            : DoAutoSkipPartial;

        _skipStartDelegate = _keyBindingProfile.AutoSkipConfig.StartCondition == ESkipStartCondition.Speaker
            ? IsCursorAndSpeakerNamePresent
            : _cursorVisibilityStateProvider.IsVisible;

        _scriptHandlerService.AutoClickScript.Register(autoSkipConfig.SkipKeys);
        Register(autoSkipConfig.ActivationKeys, OnAutoSkip);
    }

    private void OnAutoSkip()
    {
        _isAutoSkip = !_isAutoSkip;

        if (!_isAutoSkip || !_skipStartDelegate!.Invoke())
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
                Task.Delay(_keyBindingProfile!.AutoSkipConfig.Delay).Wait();
                continue;
            }

            if (IsAutoSkipCancellationRequired(taskCompletionSource)) break;
            if (_skipDialogDelegate!.Invoke()) continue;

            taskCompletionSource.SetResult(true);
        }

        return taskCompletionSource.Task;
    }

    private bool IsAutoSkipCancellationRequired(TaskCompletionSource<bool> taskCompletionSource)
    {
        if (_cursorVisibilityStateProvider.IsVisible() && _isAutoSkip) return false;

        _isAutoSkipChoicePending = false;
        _isAutoSkip              = false;
        taskCompletionSource.SetResult(false);
        return true;
    }

    #region Skip Click
    private void DoAutoSkipSingleClick()
    {
        _scriptHandlerService.AutoClickScript.DoAction();
    }

    private void DoAutoSkipDoubleClick()
    {
        _scriptHandlerService.AutoClickScript.DoAction();
        Task.Delay(_keyBindingProfile!.AutoSkipConfig.DoubleClickDelay).Wait();

        if (!_cursorVisibilityStateProvider.IsVisible() ||
            !_isAutoSkip ||
            IsDialogOptionsPresent()) return;

        _scriptHandlerService.AutoClickScript.DoAction();
    }
    #endregion

    #region Skip Start Condition
    private bool IsCursorAndSpeakerNamePresent()
    {
        return _computerVisionService.IsDialogMode() && _cursorVisibilityStateProvider.IsVisible();
    }
    #endregion

    #region Skip Mode
    private bool DoAutoSkipPartial()
    {
        _isAutoSkipChoicePending = true;
        _isAutoSkip              = false;
        return false;
    }

    private bool DoAutoSkipSkipEverything()
    {
        Cursor.Position = _cursorPositioningService.GetDefaultTargetCursorPlacement(_currentDialogOptions.Last());
        HandleSelectPress(true);
        _cursorPositioningService.Hide();
        return true;
    }
    #endregion
}
