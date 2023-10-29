using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate2P30 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.025;

    public override double TemplateHeight => 0.025;

    public override AreaDynamic TemplateSearchArea => new(0.658, 0.683, 0, 1);

    public override double DialogOptionWidth => 0.24;

    public override double DialogOptionHeight => 0.025;

    public override double Gap => 0.0046;

    public override double BackgroundPadding => 0.00104;

    public override double OutlineAreaHeight => 0.9375;

    public override RangeDynamic VerticalOutlineSearchRangeX => new(0, 0.12);

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.082, 0.16, 0.07),
        new ThresholdAreaDynamic(0.15, 0.22, 0.14, 0.22, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.28, 0.78, 0.86, 0.07),
        new ThresholdAreaDynamic(0.12, 0.19, 0.69, 0.75, 0.07),
    };
}
