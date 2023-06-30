using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBinding;

namespace Extension.HonkaiStarRail;

public sealed class KeyBindingProfile : KeyBindingProfileDefault
{
    public override List<List<GenericKey>> HideCursor { get; set; } = new()
        { new List<GenericKey> { new KeyboardKey(191) } }; // ?

    public override List<List<GenericKey>> Select { get; set; } = new()
    {
        new List<GenericKey> { new KeyboardKey(32) }, // Space
        new List<GenericKey> { new KeyboardKey(70) }  // F
    };

    public override List<List<GenericKey>> Previous { get; set; } = new()
    {
        new List<GenericKey> { new MouseKey(EHighMouseKey.MouseWheelUp) },
        new List<GenericKey> { new KeyboardKey(67) } // C
    };

    public override List<List<GenericKey>> Next { get; set; } = new()
    {
        new List<GenericKey> { new MouseKey(EHighMouseKey.MouseWheelDown) },
        new List<GenericKey> { new KeyboardKey(86) } // V
    };

    public override List<List<GenericKey>> AutoDialog { get; set; } = new()
        { new List<GenericKey> { new KeyboardKey(19) } }; // Pause

    // public override List<List<GenericKey>> FullScreenPopUp { get; set; } = new() { new List<GenericKey> { new KeyboardKey(35) } }; // End
}
