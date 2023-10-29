using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate2P37 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.0246;

    public override double TemplateHeight => 0.0246;

    public override AreaDynamic TemplateSearchArea => new(0.6619, 0.6865, 0, 1);

    public override double DialogOptionWidth => 0.225;

    public override double DialogOptionHeight => 0.0246;

    public override double Gap => 0.00443;

    public override double BackgroundPadding => 0.000781;

    public override double OutlineAreaHeight => 0.916;

    public override RangeDynamic VerticalOutlineSearchRangeX => new(0, 0.11);

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.05, 0.13, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.115, 0.2, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.28, 0.76, 0.85, 0.07),
        new ThresholdAreaDynamic(0.12, 0.2, 0.67, 0.75, 0.07)
    };
}
