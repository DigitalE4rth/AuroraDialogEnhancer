using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBinding;

namespace Extension.HonkaiStarRail;

public sealed class KeyBindingProfileDto : KeyBindingProfileDtoDefault
{
    public override List<List<GenericKeyExt>> HideCursor { get; set; } = new()
        { new List<GenericKeyExt> { new KeyboardKeyExt(191) } }; // ?

    public override List<List<GenericKeyExt>> Select { get; set; } = new()
    {
        new List<GenericKeyExt> { new KeyboardKeyExt(32) }, // Space
        new List<GenericKeyExt> { new KeyboardKeyExt(70) }  // F
    };

    public override List<List<GenericKeyExt>> Previous { get; set; } = new()
    {
        new List<GenericKeyExt> { new MouseKeyExt(EHighMouseKeyExt.MouseWheelUp) },
        new List<GenericKeyExt> { new KeyboardKeyExt(67) } // C
    };

    public override List<List<GenericKeyExt>> Next { get; set; } = new()
    {
        new List<GenericKeyExt> { new MouseKeyExt(EHighMouseKeyExt.MouseWheelDown) },
        new List<GenericKeyExt> { new KeyboardKeyExt(86) } // V
    };

    public override List<List<GenericKeyExt>> AutoDialog { get; set; } = new()
        { new List<GenericKeyExt> { new KeyboardKeyExt(19) } }; // Pause

    // public override List<List<GenericKey>> FullScreenPopUp { get; set; } = new() { new List<GenericKey> { new KeyboardKey(35) } }; // End
}
