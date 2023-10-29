using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate3P2 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.0182;

    public override double TemplateHeight => 0.0182;

    public override AreaDynamic TemplateSearchArea => new(0.70, 0.7185, 0, 1);

    public override double DialogOptionWidth => 0.165;

    public override double DialogOptionHeight => 0.0182;

    public override double Gap => 0.003125;

    public override double BackgroundPadding => 0.00078;

    public override double OutlineAreaHeight => 0.92857;

    public override RangeDynamic VerticalOutlineSearchRangeX => new(0, 0.11);

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.076, 0.15, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.12, 0.23, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.28, 0.78, 0.88, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.73, 0.8, 0.07),
    };
}
