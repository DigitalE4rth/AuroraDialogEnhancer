using System;
using AuroraDialogEnhancer.Backend.External;
using AuroraDialogEnhancer.Backend.Hooks.Game;
using AuroraDialogEnhancer.Backend.Hooks.Process;

namespace AuroraDialogEnhancer.Backend.Hooks.WindowGi;

public class FocusHookGiService : ProcessHookBase
{
    private readonly ProcessDataProvider _processDataProvider;

    /// <summary>
    /// The foreground window has changed. The system sends this event even if the foreground window has changed to another window in the same thread. Server applications never send this event.
    /// For this event, the WinEventProc callback function's hwnd parameter is the handle to the window that is in the foreground, the idObject parameter is OBJID_WINDOW, and the idChild parameter is CHILDID_SELF.
    /// </summary>
    public override uint EventMin => 0x0003;
    
    /// <summary>
    /// The foreground window has changed. The system sends this event even if the foreground window has changed to another window in the same thread. Server applications never send this event.
    /// For this event, the WinEventProc callback function's hwnd parameter is the handle to the window that is in the foreground, the idObject parameter is OBJID_WINDOW, and the idChild parameter is CHILDID_SELF.
    /// </summary>
    public override uint EventMax => 0x0003;
    
    public FocusHookGiService(ProcessDataProvider processDataProvider)
    {
        _processDataProvider = processDataProvider;
    }

    protected override void EventHookCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
    }

    public bool IsTargetWindowForeground() => IsTargetWindowForeground(NativeMethods.GetForegroundWindow());
    public bool IsTargetWindowForeground(IntPtr focusedWindowHandle)
    {
        if (_processDataProvider.Data?.GameProcess is null) return false;

        NativeMethods.GetWindowThreadProcessId(focusedWindowHandle, out var activeProcId);

        return activeProcId == _processDataProvider.Data.GameProcess.Id;
    }
}
