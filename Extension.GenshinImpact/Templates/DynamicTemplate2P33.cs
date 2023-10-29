using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate2P33 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.025;

    public override double TemplateHeight => 0.025;

    public override AreaDynamic TemplateSearchArea => new(0.65989, 0.685, 0, 1);

    public override double DialogOptionWidth => 0.225;

    public override double DialogOptionHeight => 0.025;

    public override double Gap => 0.0045;

    public override double BackgroundPadding => 0.0010416;

    public override double OutlineAreaHeight => 0.93;

    public override RangeDynamic VerticalOutlineSearchRangeX => new(0, 0.12);

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.07, 0.135, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.135, 0.206, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.3, 0.79, 0.87, 0.07),
        new ThresholdAreaDynamic(0.15, 0.235, 0.73, 0.795, 0.07)
    };
}
