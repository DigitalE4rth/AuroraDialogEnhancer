using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate2P33 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.025;

    public override double TemplateHeight => 0.025;

    public override AreaDynamic TemplateSearchArea => new(0.65989, 0.68489, 0, 1);

    public override double DialogOptionWidth => 0.24;

    public override double DialogOptionHeight => 0.025;

    public override double Gap => 0.0045;

    public override double BackgroundPadding => 0.0010416;

    public override double OutlineAreaHeight => 0.93;

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.075, 0.14, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.14, 0.21, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.28, 0.79, 0.87, 0.07),
        new ThresholdAreaDynamic(0.15, 0.235, 0.72, 0.785, 0.07)
    };
}
