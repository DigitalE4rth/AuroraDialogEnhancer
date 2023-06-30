using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AuroraDialogEnhancer.Backend.Hooks.Mouse;

[StructLayout(LayoutKind.Sequential)]
public struct CursorInfoStruct
{
    /// <summary>
    /// The structure size in bytes that must be set via calling Marshal.SizeOf(typeof(CursorInfoStruct)).
    /// </summary>
    public int StructByteSize { get; set; }

    /// <summary>
    /// The cursor state.
    /// </summary>
    /// <remarks>
    /// <para>0 == hidden.</para>
    /// <para>1 == showing.</para>
    /// <para>2 == suppressed (is supposed to be when finger touch is used, but in practice finger touch results in 0, not 2).</para>
    /// </remarks>
    public int StateFlags { get; set; }

    /// <summary>
    /// A handle to the cursor.
    /// </summary>
    public IntPtr CursorHandle { get; set; }

    /// <summary>
    /// The cursor screen coordinates.
    /// </summary>
    public Point PointStruct { get; set; }
}
