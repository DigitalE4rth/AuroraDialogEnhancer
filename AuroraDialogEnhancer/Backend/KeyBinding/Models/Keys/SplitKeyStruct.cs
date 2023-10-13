using System.Collections.Generic;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.Keys;

public class SplitKeyStruct
{
    public List<byte> ModifierKeys { get; set; }

    public byte RegularKey { get; set; }

    public SplitKeyStruct(List<byte> modifierKeys, byte regularKey)
    {
        ModifierKeys = modifierKeys;
        RegularKey   = regularKey;
    }

    public SplitKeyStruct(List<byte> modifierKeys)
    {
        ModifierKeys = modifierKeys;
        RegularKey   = 0;
    }
}
