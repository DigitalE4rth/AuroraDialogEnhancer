using System.Runtime.InteropServices;
using AuroraDialogEnhancer.Backend.External;

namespace AuroraDialogEnhancer.Backend.Hooks.Mouse;

public class CursorVisibilityProvider
{
    // private const int Hidden     = 0x00;
    // private const int Suppressed = 0x02;
    private const int Showing = 0x01;

    private CursorInfoStruct _cursorInfoStruct;
    private readonly int     _structSize;

    public CursorVisibilityProvider()
    {
        _structSize       = Marshal.SizeOf(typeof(CursorInfoStruct));
        _cursorInfoStruct = new CursorInfoStruct { StructByteSize = _structSize };
    }

    public bool IsVisible()
    {
        // The native method changes the size of the struct to 0 every time after calling it.
        // Therefore, the new struct data will not be reflected until the struct size is specified again.
        _cursorInfoStruct.StructByteSize = _structSize;
        NativeMethods.GetCursorInfo(ref _cursorInfoStruct);
        return (_cursorInfoStruct.StateFlags & Showing) != 0;
    }
}
