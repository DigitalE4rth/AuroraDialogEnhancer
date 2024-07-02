using System;
using AuroraDialogEnhancer.Backend.External;
using AuroraDialogEnhancer.Backend.Hooks.Process;

namespace AuroraDialogEnhancer.Backend.Hooks.Game;

public class FocusHookGameService : ProcessHookBase, IGameFocusService
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
    
    public bool IsFocused { get; private set; }
    public event EventHandler<bool>? OnFocusChanged;

    public FocusHookGameService(ProcessDataProvider processDataProvider)
    {
        _processDataProvider = processDataProvider;
    }
    
    public override void SetWinEventHook()
    {
        base.SetWinEventHook();
        IsFocused = IsTargetWindowForeground();
        OnFocusChanged?.Invoke(this, IsFocused);
    }

    protected override void EventHookCallback(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
    {
        IsFocused = IsTargetWindowForeground(hwnd);
        OnFocusChanged?.Invoke(this, IsFocused);
    }

    private bool IsTargetWindowForeground() => IsTargetWindowForeground(NativeMethods.GetForegroundWindow());

    private bool IsTargetWindowForeground(IntPtr focusedWindowHandle)
    {
        NativeMethods.GetWindowThreadProcessId(focusedWindowHandle, out var activeProcId);
        return activeProcId == _processDataProvider.Data!.GameProcess!.Id;
    }

    public void SendFocusedEvent()
    {
        IsFocused = IsTargetWindowForeground();
        OnFocusChanged?.Invoke(this, IsFocused);
    }
}
