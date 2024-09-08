using System;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Process;

namespace AuroraDialogEnhancer.Backend.Hooks.Window;

public class MinimizationHook : ProcessHookBase
{
    private readonly ProcessDataProvider _processDataProvider;
    
    protected override uint ProcessId => (uint) _processDataProvider.Data!.GameProcess!.Id;
    
    /// <summary>
    /// A window object is about to be minimized. This event is sent by the system, never by servers.
    /// </summary>
    public override uint EventMin => 0x0016;
    
    /// <summary>
    /// A window object is about to be restored. This event is sent by the system, never by servers.
    /// </summary>
    public override uint EventMax => 0x0017;

    private bool IsMinimized { get; set; }

    public event EventHandler<bool>? OnMinimizationChanged;

    public MinimizationHook(ProcessDataProvider processDataProvider)
    {
        _processDataProvider = processDataProvider;
    }

    public override void SetWinEventHook()
    {
        IsMinimized = _processDataProvider.Data!.GameWindowInfo!.IsMinimized();
        base.SetWinEventHook();
    }

    protected override void EventHookCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        IsMinimized = eventType == EventMin;
        OnMinimizationChanged?.Invoke(this, IsMinimized);
    }
}
