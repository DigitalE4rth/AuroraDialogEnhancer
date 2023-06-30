using System.Runtime.InteropServices;
using AuroraDialogEnhancer.Backend.External;

namespace AuroraDialogEnhancer.Backend.Hooks.Mouse;

public class CursorVisibilityStateProvider
{
    // private const int Hidden     = 0x00;
    // private const int Suppressed = 0x02;
    private const int Showing = 0x01;

    public bool IsVisible()
    {
        var cursorInfoStruct = new CursorInfoStruct
        {
            StructByteSize = Marshal.SizeOf(typeof(CursorInfoStruct))
        };
        NativeMethods.GetCursorInfo(ref cursorInfoStruct);
        return (cursorInfoStruct.StateFlags & Showing) != 0;
    }
}
