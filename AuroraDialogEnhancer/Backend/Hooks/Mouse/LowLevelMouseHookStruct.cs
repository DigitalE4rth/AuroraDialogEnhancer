using System.Drawing;
using System.Runtime.InteropServices;

namespace AuroraDialogEnhancer.Backend.Hooks.Mouse;

[StructLayout(LayoutKind.Sequential)]
public struct LowLevelMouseHookStruct
{
    public Point pt { get; set; }
    public int mouseData { get; set; }
    public int flags { get; set; }
    public int time { get; set; }
    public long dwExtraInfo { get; set; }
}
