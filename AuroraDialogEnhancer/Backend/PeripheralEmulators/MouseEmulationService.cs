using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Behaviour;

namespace AuroraDialogEnhancer.Backend.PeripheralEmulators;

public class MouseEmulationService
{
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern void mouse_event(uint dwFlags, int dx, int dy, int dwData, IntPtr dwExtraInfo);

    private const uint MOUSEEVENTF_LEFTDOWN   = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP     = 0x0004;
    private const uint MOUSEEVENTF_RIGHTDOWN  = 0x0008;
    private const uint MOUSEEVENTF_RIGHTUP    = 0x0010;
    private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
    private const uint MOUSEEVENTF_MIDDLEUP   = 0x0040;
    private const uint MOUSEEVENTF_WHEEL      = 0x0800;
    private const uint MOUSEEVENTF_XDOWN      = 0x0080;
    private const uint MOUSEEVENTF_XUP        = 0x0100;
    private const int  MK_XBUTTON1            = 0x0001;
    private const int  MK_XBUTTON2            = 0x0002;
    private const int  WHEEL_DELTA_UP         = 120;
    private const int  WHEEL_DELTA_DOWN       = -120;

    private void DoButtonClick(uint downKey, uint upKey, int dwData = 0)
    {
        mouse_event(downKey, Cursor.Position.X, Cursor.Position.Y, dwData, IntPtr.Zero);
        Task.Delay(15).Wait();
        mouse_event(upKey, Cursor.Position.X, Cursor.Position.Y, dwData, IntPtr.Zero);
    }

    public void DoPrimaryClick() => DoButtonClick(MOUSEEVENTF_LEFTDOWN, MOUSEEVENTF_LEFTUP);

    public void DoMouseAction(EHighMouseKey key)
    {
        switch (key)
        {
            case EHighMouseKey.MiddleButton:
                DoButtonClick(MOUSEEVENTF_MIDDLEDOWN, MOUSEEVENTF_MIDDLEUP);
                return;
            case EHighMouseKey.MouseWheelUp:
                mouse_event(MOUSEEVENTF_WHEEL, Cursor.Position.X, Cursor.Position.Y, WHEEL_DELTA_UP, IntPtr.Zero);
                return;
            case EHighMouseKey.MouseWheelDown:
                mouse_event(MOUSEEVENTF_WHEEL, Cursor.Position.X, Cursor.Position.Y, WHEEL_DELTA_DOWN, IntPtr.Zero);
                return;
            case EHighMouseKey.Forward:
                DoButtonClick(MOUSEEVENTF_XDOWN, MOUSEEVENTF_XUP, MK_XBUTTON2);
                break;
            case EHighMouseKey.Back:
                DoButtonClick(MOUSEEVENTF_XDOWN, MOUSEEVENTF_XUP, MK_XBUTTON1);
                break;
        }
    }
}
