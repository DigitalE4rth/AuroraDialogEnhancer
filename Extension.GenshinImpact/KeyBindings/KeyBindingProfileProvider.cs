﻿using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.KeyBindings;

namespace Extension.GenshinImpact.KeyBindings;

public class KeyBindingProfileProvider : KeyBindingProfileProviderDto
{
    public override KeyBindingProfileDto KeyBindingProfileDto { get; } = new KeyBindingProfile();
    public override List<ClickablePointVmDto> ClickablePointsVm { get; } = new()
    {
        new ClickablePointVmDto(
            "autoplay",
            Properties.Resources.ClickablePoint_AutoPlay_Name,
            Properties.Resources.ClickablePoint_AutoPlay_Description,
            "M464 256A208 208 0 1 0 48 256a208 208 0 1 0 416 0zM0 256a256 256 0 1 1 512 0A256 256 0 1 1 0 256zM188.3 147.1c7.6-4.2 16.8-4.1 24.3 .5l144 88c7.1 4.4 11.5 12.1 11.5 20.5s-4.4 16.1-11.5 20.5l-144 88c-7.4 4.5-16.7 4.7-24.3 .5s-12.3-12.2-12.3-20.9V168c0-8.7 4.7-16.7 12.3-20.9z")
    };
}
