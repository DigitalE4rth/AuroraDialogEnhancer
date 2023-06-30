using System.Collections.Generic;

namespace AuroraDialogEnhancer.Backend.Hooks.Keyboard;

public class ModifierKeysProvider
{
    private readonly HashSet<int> _modifierKeysHashSet = new()
    {
        0x12, 0xA4, 0xA5, // Alt
        0xA2, 0xA3, 0x11, // Control
        0xA0, 0xA1, 0x11, // Shift
        0x5B, 0x5B        // Windows 
    };

    public bool IsModifierKey(int virtualKeyCode) => _modifierKeysHashSet.Contains(virtualKeyCode);
}
