using System;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Process;

namespace AuroraDialogEnhancer.Backend.Hooks.Window;

public class WindowLocationHookService : ProcessHookBase
{
    private readonly ProcessDataProvider _processDataProvider;
    private readonly ProcessInfoService  _processInfoService;

    protected override uint ProcessId => (uint) _processDataProvider.Data!.GameProcess!.Id;
    
    /// <summary>
    /// The movement or resizing of a window has finished. This event is sent by the system, never by servers.
    /// </summary>
    /// 
    public override uint EventMin => 0x000B;
    
    /// <summary>
    /// A window object is about to be minimized. This event is sent by the system, never by servers.
    /// </summary>
    public override uint EventMax => 0x0016;

    public event EventHandler? OnWindowLocationChanged;
    
    public WindowLocationHookService(ProcessDataProvider processDataProvider, ProcessInfoService processInfoService)
    {
        _processDataProvider = processDataProvider;
        _processInfoService  = processInfoService;
    }

    protected override void EventHookCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        if (_processDataProvider.Data!.GameProcess!.MainWindowHandle == IntPtr.Zero) return;
        _processInfoService.SetWindowLocation();
        OnWindowLocationChanged?.Invoke(this, EventArgs.Empty);
    }
}
