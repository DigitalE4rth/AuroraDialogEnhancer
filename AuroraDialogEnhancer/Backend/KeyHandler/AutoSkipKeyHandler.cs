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
    private bool        _isAutoSkipReplyPending;
    private Func<bool>? _skipStartDelegate;
    private Action?     _skipTaskLoopDelegate;
    private Task?       _runningSkipTask;
    private CancellationTokenSource? _autoSkipCts;

    private void RegisterAutoSkip(AutoSkipConfig autoSkipConfig)
    {
        _skipStartDelegate = _keyBindingProfile!.AutoSkipConfig.StartCondition == ESkipStartCondition.Speaker
            ? IsCursorAndSpeakerNamePresent
            : _cursorVisibilityStateProvider.IsVisible;

        _skipTaskLoopDelegate = _keyBindingProfile!.AutoSkipConfig.SkipMode switch
        {
            ESkipMode.Everything => StartAutoSkipLoopTextAndRelies,
            ESkipMode.Replies    => StartAutoSkipLoopRepliesOnly,
            ESkipMode.Text       => StartAutoSkipLoopText,
            _                    => StartAutoSkipLoopTextAndRelies
        };

        _scriptHandlerService.AutoClickScript.Register(autoSkipConfig.SkipKeys);
        Register(autoSkipConfig.ActivationKeys, OnAutoSkip);
    }

    private void OnAutoSkip()
    {
        _isAutoSkip = !_isAutoSkip;

        if (!_isAutoSkip || !_skipStartDelegate!.Invoke())
        {
            _isAutoSkip = false;
            _autoSkipCts?.Cancel();
            return;
        }

        _isAutoSkipReplyPending = false;
        _cursorPositioningService.Hide();

        if (_runningSkipTask is not null 
            && !_runningSkipTask.IsCompleted 
            && _runningSkipTask.Status == TaskStatus.Running)
        {
            _autoSkipCts!.Cancel();
            _runningSkipTask.Wait();
            _autoSkipCts.Dispose();
        }

        _autoSkipCts     = new CancellationTokenSource();
        _runningSkipTask = Task.Run(() => _skipTaskLoopDelegate!);
    }

    #region Loops ~OoOoOoOo
    private bool ReplyRegularScanLoop()
    {
        while (_isAutoSkip)
        {
            if (IsAutoSkipCancellationRequired()) break;
            if (AreDialogOptionsPresent()) return true;
            Task.Delay(_keyBindingProfile!.AutoSkipConfig.ScanDelayRegular).Wait(_autoSkipCts!.Token);
        }

        return false;
    }

    private void StartAutoSkipLoopTextAndRelies()
    {
        var repliesScanAsync = Task.Run(ReplyRegularScanLoop).ConfigureAwait(false);
        
        while (_isAutoSkip)
        {
            if (IsAutoSkipCancellationRequired()) break;
            if (repliesScanAsync.GetAwaiter().IsCompleted && repliesScanAsync.GetAwaiter().GetResult())
            {
                DoClickLastReply();
                repliesScanAsync = Task.Run(ReplyRegularScanLoop).ConfigureAwait(false);
                continue;
            }

            DoTextSkipSingleClick();
            Task.Delay(_keyBindingProfile!.AutoSkipConfig.ClickDelayRegular).Wait(_autoSkipCts!.Token);
        }
    }

    private void StartAutoSkipLoopText()
    {
        var repliesScanAsync = Task.Run(ReplyRegularScanLoop).ConfigureAwait(false);
        var taskAwaiter = repliesScanAsync.GetAwaiter();

        while (_isAutoSkip)
        {
            if (IsAutoSkipCancellationRequired()) break;
            if (taskAwaiter.IsCompleted && taskAwaiter.GetResult())
            {
                DoWaitForManualReplyClick();
                return;
            }

            DoTextSkipSingleClick();
            Task.Delay(_keyBindingProfile!.AutoSkipConfig.ClickDelayRegular).Wait(_autoSkipCts!.Token);
        }
    }

    private void StartAutoSkipLoopRepliesOnly()
    {
        while (_isAutoSkip)
        {
            if (IsAutoSkipCancellationRequired()) break;
            if (AreDialogOptionsPresent())
            {
                Task.Delay(_keyBindingProfile!.AutoSkipConfig.ClickDelayReply).Wait(_autoSkipCts!.Token);
                if (IsAutoSkipCancellationRequired()) break;
                DoClickLastReply();
                continue;
            }

            Task.Delay(_keyBindingProfile!.AutoSkipConfig.ScanDelayReply).Wait(_autoSkipCts!.Token);
        }
    }
    #endregion

    private bool IsAutoSkipCancellationRequired()
    {
        if (_cursorVisibilityStateProvider.IsVisible() 
            && _isAutoSkip 
            && !_autoSkipCts?.IsCancellationRequested == true) return false;

        _isAutoSkipReplyPending = false;
        _isAutoSkip             = false;
        return true;
    }

    #region Skip Click
    private void DoTextSkipSingleClick()
    {
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
    private void DoWaitForManualReplyClick()
    {
        _isAutoSkipReplyPending = true;
        _isAutoSkip             = false;
        _autoSkipCts?.Cancel();
    }

    private void DoClickLastReply()
    {
        Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_currentDialogOptions.Last());
        HandleSelectPress(true);
        _cursorPositioningService.Hide();
    }
    #endregion
}
