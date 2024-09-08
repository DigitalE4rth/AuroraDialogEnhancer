using System;
using System.Threading;
using System.Threading.Tasks;

namespace AuroraDialogEnhancer.Backend.KeyHandler.Scripts;

public class ScriptAutoSkipUtilities : IDisposable
{
    public bool IsAutoSkip;
    public bool IsReplyPending;

    public Func<bool>?              StartConditionDelegate;
    public Action?                  SkipBehaviourDelegate;
    public Task?                    ScriptTask;
    public CancellationTokenSource? CancellationTokenSource;
    public Action?                  RestartDelegate;

    public void Dispose()
    {
        ScriptTask?.Dispose();
        CancellationTokenSource?.Dispose();
        IsAutoSkip     = false;
        IsReplyPending = false;
    }
}
