using System;
using System.Threading;
using System.Threading.Tasks;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Process;

namespace AuroraDialogEnhancer.Backend.Hooks.Window;

public class MinimizationEndObserver : MinimizationEndHook
{
    private readonly ProcessDataProvider _processDataProvider;
    private readonly ProcessInfoService  _processInfoService;

    private SemaphoreSlim? _minimizationEndSemaphore;

    public MinimizationEndObserver(ProcessInfoService processInfoService, ProcessDataProvider processDataProvider) : base(processDataProvider)
    {
        _processInfoService  = processInfoService;
        _processDataProvider = processDataProvider;
    }

    public async Task<bool> AwaitMinimizationEndAsync(CancellationToken cancellationToken)
    {
        if (!_processDataProvider.Data!.GameWindowInfo!.IsMinimized()) return true;

        SetWinEventHook();

        _processDataProvider.SetStateAndNotify(EHookState.Hooked);

        _minimizationEndSemaphore = new SemaphoreSlim(0);

        if (!_processDataProvider.Data!.GameWindowInfo!.IsMinimized())
        {
            UnhookWinEvent();
            _minimizationEndSemaphore?.Release();
            return true;
        }

        try
        {
            await _minimizationEndSemaphore?.WaitAsync(cancellationToken)!;
            _processInfoService.ApplyWindowInfo();
        }
        catch
        {
            return false;
        }

        return true;
    }

    protected override void EventHookCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        UnhookWinEvent();
        _minimizationEndSemaphore?.Release();
    }

    public override void UnhookWinEvent()
    {
        base.UnhookWinEvent();
        _minimizationEndSemaphore?.Release();
    }
}
