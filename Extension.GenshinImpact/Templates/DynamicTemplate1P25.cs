using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

/// <summary>
/// 5:4
/// </summary>
public class DynamicTemplate1P25 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.0328;

    public override double TemplateHeight => 0.0328;

    public override AreaDynamic TemplateSearchArea => new(0.6594, 0.6935, 0, 1);

    public override double DialogOptionWidth => 0.3;

    public override double DialogOptionHeight => 0.0328;

    public override double Gap => 0.005468;

    public override double BackgroundPadding => 0.00156;

    public override double OutlineAreaHeight => 0.9285;

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.073, 0.14, 0.07),
        new ThresholdAreaDynamic(0.15, 0.22, 0.135, 0.215, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.28, 0.79, 0.88, 0.07),
        new ThresholdAreaDynamic(0.12, 0.19, 0.69, 0.76, 0.07)
    };
}
