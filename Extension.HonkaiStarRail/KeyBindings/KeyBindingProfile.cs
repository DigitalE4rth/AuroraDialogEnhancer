using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings;
using AuroraDialogEnhancerExtensions.KeyBindings.Behaviour;
using AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;
using AuroraDialogEnhancerExtensions.KeyBindings.Keys;
using AuroraDialogEnhancerExtensions.KeyBindings.Scripts;

namespace Extension.HonkaiStarRail.KeyBindings;

public sealed class KeyBindingProfile : KeyBindingProfileDtoDefault
{
    public override List<List<GenericKeyDto>> HideCursor { get; set; } = new()
        { new List<GenericKeyDto> { new KeyboardKeyDto(191) } }; // ?

    public override List<List<GenericKeyDto>> Select { get; set; } = new()
    {
        new List<GenericKeyDto> { new KeyboardKeyDto(32) }, // Space
        new List<GenericKeyDto> { new KeyboardKeyDto(70) }  // F
    };

    public override List<List<GenericKeyDto>> Previous { get; set; } = new()
    {
        new List<GenericKeyDto> { new MouseKeyDto(EHighMouseKeyDto.MouseWheelUp) },
        new List<GenericKeyDto> { new KeyboardKeyDto(67) } // C
    };

    public override List<List<GenericKeyDto>> Next { get; set; } = new()
    {
        new List<GenericKeyDto> { new MouseKeyDto(EHighMouseKeyDto.MouseWheelDown) },
        new List<GenericKeyDto> { new KeyboardKeyDto(86) } // V
    };

    public override List<List<GenericKeyDto>> Last { get; set; } = new()
        { new List<GenericKeyDto> { new KeyboardKeyDto(27) } }; // Esc

    public override List<InteractionPointDto> InteractionPoints { get; set; } = new()
    {
        new InteractionPointDto("autoplay", new List<List<GenericKeyDto>>
        { new() { new KeyboardKeyDto(192) } }), // ~
        new InteractionPointDto("hideui", new List<List<GenericKeyDto>>()),
        new InteractionPointDto("fullscreenpopup", new List<List<GenericKeyDto>>
        { new() { new KeyboardKeyDto(38) } }) // Up
    };

    public override AutoSkipConfigDto AutoSkipConfigDto { get; set; } = new
    (
        new List<List<GenericKeyDto>> { new() { new KeyboardKeyDto(187) } }, // =
        ESkipModeDto.Everything,
        ESkipStartConditionDto.Speaker,
        new List<GenericKeyDto> { new KeyboardKeyDto(32) }, // Space
        350,
        300,
        600,
        10000
    );
}
