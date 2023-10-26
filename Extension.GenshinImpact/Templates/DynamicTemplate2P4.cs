using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate2P4 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.0244;

    public override double TemplateHeight => 0.0244;

    public override AreaDynamic TemplateSearchArea => new(0.663, 0.6875, 0, 1);

    public override double DialogOptionWidth => 0.22;

    public override double DialogOptionHeight => 0.0244;

    public override double Gap => 0.00442;

    public override double BackgroundPadding => 0.00103;

    public override double OutlineAreaHeight => 0.925;

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.085, 0.165, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.15, 0.23, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.28, 0.75, 0.85, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.69, 0.75, 0.07),
    };
}
