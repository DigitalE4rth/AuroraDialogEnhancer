using System.Runtime.InteropServices;
using AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;

namespace AuroraDialogEnhancer.Backend.PeripheralEmulators;

public class KeyboardEmulationService
{
    [DllImport("user32.dll", SetLastError = true)]
    private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

    public const int KEYEVENTF_KEYDOWN     = 0x0000; // New definition
    public const int KEYEVENTF_EXTENDEDKEY = 0x0001; // Key down flag
    public const int KEYEVENTF_KEYUP       = 0x0002; // Key up flag


    private void KeyDown(byte keyCode)
    {
        keybd_event(keyCode, 0, KEYEVENTF_KEYDOWN, 0);
    }

    private void KeyUp(byte keyCode)
    {
        keybd_event(keyCode, 0, KEYEVENTF_KEYUP, 0);
    }

    public void DoKeyboardClickFull(SplitKeyStruct splitKeyStruct)
    {
        foreach (var t in splitKeyStruct.ModifierKeys)
        {
            KeyDown(t);
        }

        KeyDown(splitKeyStruct.RegularKey);
        KeyUp(splitKeyStruct.RegularKey);

        for (var i = splitKeyStruct.ModifierKeys.Count - 1; i >= 0; i--)
        {
            KeyUp(splitKeyStruct.ModifierKeys[i]);
        }
    }

    public void DoKeyboardClickOnlyRegular(SplitKeyStruct splitKeyStruct)
    {
        KeyDown(splitKeyStruct.RegularKey);
        KeyUp(splitKeyStruct.RegularKey);
    }

    public void DoKeyboardClickModifiers(SplitKeyStruct splitKeyStruct)
    {
        foreach (var t in splitKeyStruct.ModifierKeys)
        {
            KeyDown(t);
        }

        for (var i = splitKeyStruct.ModifierKeys.Count - 1; i >= 0; i--)
        {
            KeyUp(splitKeyStruct.ModifierKeys[i]);
        }
    }
}
