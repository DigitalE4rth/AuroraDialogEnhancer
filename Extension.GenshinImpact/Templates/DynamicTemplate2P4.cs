﻿using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate2P4 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.0244;

    public override double TemplateHeight => 0.0244;

    public override AreaDynamic TemplateSearchArea => new(0.6642, 0.6888, 0, 1);

    public override double DialogOptionWidth => 0.222;

    public override double DialogOptionHeight => 0.0244;

    public override double Gap => 0.00442;

    public override double BackgroundPadding => 0.00104;

    public override double OutlineAreaHeight => 0.9255;

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.048, 0.125, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.1, 0.18, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.28, 0.78, 0.87, 0.07),
        new ThresholdAreaDynamic(0.135, 0.2, 0.7, 0.8, 0.07)
    };
}
