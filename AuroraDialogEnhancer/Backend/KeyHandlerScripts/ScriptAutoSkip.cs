using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AuroraDialogEnhancer.Backend.CursorPositioning;
using AuroraDialogEnhancer.Backend.Hooks.Mouse;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Scripts;
using AuroraDialogEnhancer.Backend.KeyHandler;

namespace AuroraDialogEnhancer.Backend.KeyHandlerScripts;

public class ScriptAutoSkip : IDisposable
{
    private readonly CursorPositioningService _cursorPositioningService;
    private readonly CursorVisibilityProvider _cursorVisibilityProvider;
    private readonly KeyActionAccessibility   _keyActionAccessibility;
    private readonly KeyActionExecution       _keyActionExecution;
    private readonly KeyActionUtility         _keyActionUtility;
    private readonly ScriptAutoClick          _scriptAutoClick;
    private readonly ScriptAutoSkipUtilities  _autoSkipUtilities;

    public ScriptAutoSkip(CursorPositioningService cursorPositioningService,
                          CursorVisibilityProvider cursorVisibilityProvider,
                          KeyActionAccessibility   keyActionAccessibility,
                          KeyActionExecution       keyActionExecution,
                          KeyActionUtility         keyActionUtility, 
                          ScriptAutoClick          scriptAutoClick, 
                          ScriptAutoSkipUtilities  autoSkipUtilities)
    {
        _cursorPositioningService = cursorPositioningService;
        _cursorVisibilityProvider = cursorVisibilityProvider;
        _keyActionAccessibility   = keyActionAccessibility;
        _keyActionExecution       = keyActionExecution;
        _keyActionUtility         = keyActionUtility;
        _scriptAutoClick          = scriptAutoClick;
        _autoSkipUtilities        = autoSkipUtilities;
        _autoSkipUtilities.RestartDelegate = () => Start(true);
    }

    #region Controls
    public void OnAutoSkip() => Start();

    public void Start(bool isRestart = false) => _keyActionAccessibility.WithAccess(() =>
    {
        if (!CanAutoSkipBeExecuted(isRestart)) return;

        _autoSkipUtilities.IsReplyPending = false;

        if (_autoSkipUtilities.ScriptTask is not null  &&
            !_autoSkipUtilities.ScriptTask.IsCompleted &&
            _autoSkipUtilities.ScriptTask.Status == TaskStatus.Running)
        {
            try
            {
                _autoSkipUtilities.CancellationTokenSource?.Cancel();
                _autoSkipUtilities.ScriptTask.Wait();
                _autoSkipUtilities.CancellationTokenSource?.Dispose();
            }
            catch (Exception)
            {
                // Ignored cancellation in synchronized call
            }
        }

        _cursorPositioningService.Hide();
        _autoSkipUtilities.CancellationTokenSource = new CancellationTokenSource();
        _autoSkipUtilities.ScriptTask = Task.Run(_autoSkipUtilities.SkipBehaviourDelegate!);
    });

    public void Stop()
    {
        _autoSkipUtilities.IsReplyPending = false;
        _autoSkipUtilities.IsAutoSkip     = false;

        if (_autoSkipUtilities.ScriptTask is null     ||
            _autoSkipUtilities.ScriptTask.IsCompleted ||
            _autoSkipUtilities.ScriptTask.Status != TaskStatus.Running)
        {
            return;
        }

        try
        {
            _autoSkipUtilities.CancellationTokenSource?.Cancel();
            _autoSkipUtilities.ScriptTask.Wait();
            _autoSkipUtilities.CancellationTokenSource?.Dispose();
        }
        catch (Exception)
        {
            // Ignored cancellation in synchronized call
        }
    }

    private bool Restart()
    {
        if (_autoSkipUtilities.StartConditionDelegate!.Invoke())
        {
            _autoSkipUtilities.IsAutoSkip = true;
            return true;
        }

        _autoSkipUtilities.IsAutoSkip = false;
        _autoSkipUtilities.CancellationTokenSource?.Cancel();
        return false;
    }

    private bool SwitchRunningState()
    {
        _autoSkipUtilities.IsAutoSkip = !_autoSkipUtilities.IsAutoSkip;
        if (_autoSkipUtilities.IsAutoSkip && _autoSkipUtilities.StartConditionDelegate!.Invoke()) return true;

        _autoSkipUtilities.IsAutoSkip = false;
        _autoSkipUtilities.CancellationTokenSource?.Cancel();
        return false;
    }

    private bool CanAutoSkipBeExecuted(bool isRestart)
    {
        return isRestart ? Restart() : SwitchRunningState();
    }

    public void ApplySettings()
    {
        _autoSkipUtilities.StartConditionDelegate = _keyActionUtility.KeyBindingProfile.AutoSkipConfig.StartCondition == ESkipStartCondition.Speaker
            ? _keyActionAccessibility.AreCursorAndSpeakerNamePresent
            : _cursorVisibilityProvider.IsVisible;

        _autoSkipUtilities.SkipBehaviourDelegate = _keyActionUtility.KeyBindingProfile.AutoSkipConfig.SkipMode switch
        {
            ESkipMode.Everything => StartLoopTextAndRelies,
            ESkipMode.Replies    => StartLoopRepliesOnly,
            ESkipMode.Text       => StartLoopText,
            _                    => StartLoopTextAndRelies
        };
    }
    #endregion

    #region Loops
    private void StartLoopTextAndRelies()
    {
        var repliesScanAsync = Task.Run(StartLoopScanReplies, _autoSkipUtilities.CancellationTokenSource!.Token);

        while (_autoSkipUtilities.IsAutoSkip)
        { 
            if (IsCancellationRequired()) break;
            if (repliesScanAsync.IsCompleted && repliesScanAsync.Result)
            {
                DoClickLastReply();
                repliesScanAsync = Task.Run(StartLoopScanReplies, _autoSkipUtilities.CancellationTokenSource!.Token);
                continue;
            }

            DoTextSkipSingleClick();
            var delayTask = Task.Delay(_keyActionUtility.KeyBindingProfile.AutoSkipConfig.ClickDelayRegular, _autoSkipUtilities.CancellationTokenSource.Token);
            Task.WhenAny(repliesScanAsync, delayTask).Wait(_autoSkipUtilities.CancellationTokenSource.Token);
        }
    }

    private void StartLoopRepliesOnly()
    {
        while (_autoSkipUtilities.IsAutoSkip)
        {
            if (IsCancellationRequired()) break;
            if (_keyActionAccessibility.AreDialogOptionsPresent())
            {
                var clickDelayTask = Task.Delay(_keyActionUtility.KeyBindingProfile.AutoSkipConfig.ClickDelayReply, _autoSkipUtilities.CancellationTokenSource!.Token);
                var cancellationScanLoopTask = Task.Run(() => StartLoopScanForRepliesOnly(clickDelayTask), _autoSkipUtilities.CancellationTokenSource!.Token);

                Task.WhenAny(clickDelayTask, cancellationScanLoopTask).Wait(_autoSkipUtilities.CancellationTokenSource!.Token);

                if (IsCancellationRequired()) break;
                DoClickLastReply();
                continue;
            }

            Task.Delay(_keyActionUtility.KeyBindingProfile.AutoSkipConfig.ScanDelayReply).Wait(_autoSkipUtilities.CancellationTokenSource!.Token);
        }
    }

    private void StartLoopText()
    {
        var repliesScanAsync = Task.Run(StartLoopScanReplies, _autoSkipUtilities.CancellationTokenSource!.Token);

        while (_autoSkipUtilities.IsAutoSkip)
        {
            if (IsCancellationRequired()) break;
            if (repliesScanAsync.IsCompleted && repliesScanAsync.Result)
            {
                DoWaitForManualReplyClick();
                return;
            }

            DoTextSkipSingleClick();

            var delayTask = Task.Delay(_keyActionUtility.KeyBindingProfile.AutoSkipConfig.ClickDelayRegular, _autoSkipUtilities.CancellationTokenSource.Token);
            Task.WhenAny(repliesScanAsync, delayTask).Wait(_autoSkipUtilities.CancellationTokenSource.Token);
        }
    }

    private bool StartLoopScanReplies()
    {
        while (_autoSkipUtilities.IsAutoSkip)
        {
            if (IsCancellationRequired()) break;
            if (_keyActionAccessibility.AreDialogOptionsPresent()) return true;
            Task.Delay(_keyActionUtility.KeyBindingProfile.AutoSkipConfig.ScanDelayRegular).Wait(_autoSkipUtilities.CancellationTokenSource!.Token);
        }

        return false; 
    }

    private void StartLoopScanForRepliesOnly(IAsyncResult clickDelayTask)
    {
        while (_autoSkipUtilities.IsAutoSkip && !clickDelayTask.IsCompleted)
        {
            if (IsCancellationRequired())
            {
                _autoSkipUtilities.CancellationTokenSource!.Cancel();
                break;
            }

            if (!_keyActionUtility.DialogOptions.Any()) break;
            Task.Delay(_keyActionUtility.KeyBindingProfile.AutoSkipConfig.ScanDelayReply).Wait(_autoSkipUtilities.CancellationTokenSource!.Token);
        }
    }
    #endregion

    #region Unitls
    private void DoWaitForManualReplyClick()
    {
        _autoSkipUtilities.IsReplyPending = true;
        _autoSkipUtilities.IsAutoSkip     = false;
        _autoSkipUtilities.CancellationTokenSource!.Cancel();
    }

    private bool IsCancellationRequired()
    {
        if (_cursorVisibilityProvider.IsVisible() &&
            _autoSkipUtilities.IsAutoSkip         &&
            !_autoSkipUtilities.CancellationTokenSource!.IsCancellationRequested)
        {
            return false;
        }

        _autoSkipUtilities.IsReplyPending = false;
        _autoSkipUtilities.IsAutoSkip     = false;
        return true;
    }

    private void DoClickLastReply() => _keyActionAccessibility.WithAccess(() =>
    {
        if (!_keyActionUtility.DialogOptions.Any()) return;

        Cursor.Position = _cursorPositioningService.GetTargetCursorPlacement(_keyActionUtility.DialogOptions.Last());
        _keyActionExecution.HandleSelectPressArtificial();
        _cursorPositioningService.Hide();
    });
    

    private void DoTextSkipSingleClick() => _scriptAutoClick.DoAction();
    #endregion

    public void Dispose()
    {
        _autoSkipUtilities.Dispose();
    }
}
