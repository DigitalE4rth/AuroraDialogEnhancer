using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate2P35 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.025;

    public override double TemplateHeight => 0.025;

    public override AreaDynamic TemplateSearchArea => new(0.66092, 0.686, 0, 1);

    public override double DialogOptionWidth => 0.225;

    public override double DialogOptionHeight => 0.025;

    public override double Gap => 0.00443;

    public override double BackgroundPadding => 0.00104;

    public override double OutlineAreaHeight => 0.916;

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.07, 0.155, 0.07),
        new ThresholdAreaDynamic(0.15, 0.22, 0.135, 0.225, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.28, 0.78, 0.862, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.72, 0.79, 0.07)
    };
}
