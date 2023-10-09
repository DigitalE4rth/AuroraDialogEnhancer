using System;
using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace Extension.GenshinImpact.KeyBindings;

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

    public override List<ClickablePointDto> ClickablePoints { get; set; } = new()
    {
        new ClickablePointDto("autoplay", new List<List<GenericKeyDto>>
        {
            new() { new KeyboardKeyDto(192) } // ~
        })
    };

    public override AutoSkipDto AutoSkipDto { get; set; } = new
    (
        new List<List<GenericKeyDto>> { new() { new KeyboardKeyDto(187) } }, // =
        EAutoSkipTypeDto.Everything,
        new List<GenericKeyDto> { new KeyboardKeyDto(32) }, // Space
        400,
        true,
        125
    );
}
