using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate3P55 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.0164;

    public override double TemplateHeight => 0.0164;

    public override AreaDynamic TemplateSearchArea => new(0.711, 0.728, 0, 1);

    public override double DialogOptionWidth => 0.15;

    public override double DialogOptionHeight => 0.0164;

    public override double Gap => 0.0028;

    public override double BackgroundPadding => 0.00078;

    public override double OutlineAreaHeight => 0.9206;

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.08, 0.16, 0.07),
        new ThresholdAreaDynamic(0.143, 0.22, 0.15, 0.22, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.28, 0.78, 0.88, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.72, 0.8, 0.07)
    };
}
