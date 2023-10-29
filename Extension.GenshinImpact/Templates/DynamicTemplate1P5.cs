using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

/// <summary>
/// 3:2
/// </summary>
public class DynamicTemplate1P5 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.033;

    public override double TemplateHeight => 0.033;

    public override AreaDynamic TemplateSearchArea => new(0.6594, 0.693, 0, 1);

    public override double DialogOptionWidth => 0.3;

    public override double DialogOptionHeight => 0.033;

    public override double Gap => 0.0055;

    public override double BackgroundPadding => 0.0014;

    public override double OutlineAreaHeight => 0.92;

    public override double VerticalOutlineThreshold => 0.95;

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.215, 0.29, 0.071, 0.14, 0.07),
        new ThresholdAreaDynamic(0.14, 0.21, 0.135, 0.215, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.28, 0.75, 0.87, 0.07),
        new ThresholdAreaDynamic(0.12, 0.19, 0.68, 0.77, 0.07)
    };
}
