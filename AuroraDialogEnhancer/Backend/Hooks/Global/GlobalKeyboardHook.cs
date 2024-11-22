using AuroraDialogEnhancer.Backend.Hooks.Process;

namespace AuroraDialogEnhancer.Backend.Hooks.Global;

public class GlobalKeyboardHook : ProcessHookBase
{
    /// <summary>
    /// An object has received the keyboard focus. The system sends this event for the following user interface elements: list-view control, menu bar, pop-up menu, switch window, tab control, tree view control, and window object. Server applications send this event for their accessible objects.
    /// The hwnd parameter of the WinEventProc callback function identifies the window that receives the keyboard focus.
    /// </summary>
    public override uint EventMin => 0x8005;
    
    /// <summary>
    /// An object has received the keyboard focus. The system sends this event for the following user interface elements: list-view control, menu bar, pop-up menu, switch window, tab control, tree view control, and window object. Server applications send this event for their accessible objects.
    /// The hwnd parameter of the WinEventProc callback function identifies the window that receives the keyboard focus.
    /// </summary>
    public override uint EventMax => 0x8005;
}
