using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

/// <summary>
/// 4:3
/// </summary>
public class DynamicTemplate1P33 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.033;

    public override double TemplateHeight => 0.033;

    public override AreaDynamic TemplateSearchArea => new(0.66, 0.695, 0, 1);

    public override double DialogOptionWidth => 0.3;

    public override double DialogOptionHeight => 0.033;

    public override double Gap => 0.005468;

    public override double BackgroundPadding => 0.0015;

    public override double OutlineAreaHeight => 0.92;

    public override double VerticalOutlineThreshold => 0.95;

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.071, 0.14, 0.07),
        new ThresholdAreaDynamic(0.15, 0.22, 0.135, 0.215, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.28, 0.75, 0.87, 0.07),
        new ThresholdAreaDynamic(0.12, 0.19, 0.68, 0.77, 0.07)
    };
}
