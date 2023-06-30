using System;
using System.Collections.Generic;

namespace AuroraDialogEnhancer.Backend.Hooks.Keyboard;

public class KeyboardHookManagerRecordService : KeyboardHookManagerServiceBase
{
    public event EventHandler<HashSet<int>>? OnKeyDown;
    public event EventHandler<HashSet<int>>? OnKeyUp;

    public KeyboardHookManagerRecordService(ModifierKeysProvider modifierKeysProvider) : base(modifierKeysProvider)
    {
    }

    protected override void HandleSingleKeyboardInput(IntPtr wParam, int virtualKeyCode)
    {
        var isModifier = _modifierKeysProvider.IsModifierKey(virtualKeyCode);

        // If the keyboard event is a KeyDown event (i.e. key pressed)
        if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
        {
            if (isModifier)
            {
                lock (_modifiersLock)
                {
                    DownModifierKeys.Add(virtualKeyCode);
                }
            }

            // Trigger callbacks that are registered for this key, but only once per key press
            if (!DownKeys.Contains(virtualKeyCode))
            {
                HandleKeyPress(virtualKeyCode);
                DownKeys.Add(virtualKeyCode);
                OnKeyDown?.Invoke(this, DownKeys);
                return;
            }
        }

        // If the keyboard event is a KeyUp event (i.e. key released)
        if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSKEYUP)
        {
            if (isModifier)
            {
                lock (_modifiersLock)
                {
                    OnKeyUp?.Invoke(this, DownKeys);
                    DownModifierKeys.Remove(virtualKeyCode);
                }
            }

            DownKeys.Remove(virtualKeyCode);
        }
    }
}
