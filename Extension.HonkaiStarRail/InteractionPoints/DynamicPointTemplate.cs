﻿using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.InteractionPoints;

public class DynamicPointTemplate
{
    public DynamicPoint AutoPlay { get; set; } = new(0.03638677, 0.03668692);
    public DynamicPoint HideUi { get; set; } = new(0.03638677, 0.03668692);
    public DynamicPoint FullScreenPopUp { get; set; } = new(0.5, 0.8);
}
