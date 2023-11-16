﻿using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings;
using AuroraDialogEnhancerExtensions.KeyBindings.Behaviour;
using AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;
using AuroraDialogEnhancerExtensions.KeyBindings.Keys;

namespace Extension.HonkaiStarRail;

public sealed class KeyBindingProfileDto : KeyBindingProfileDtoDefault
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

    public override List<InteractionPointDto> InteractionPoints { get; set; } = new()
    {
        new InteractionPointDto("autoplay", new List<List<GenericKeyDto>>
        {
            new() { new KeyboardKeyDto(192) } // ~
        })
    };
}
