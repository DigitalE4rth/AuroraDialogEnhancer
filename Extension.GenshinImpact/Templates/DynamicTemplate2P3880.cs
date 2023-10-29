using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate2P3880 : DynamicTemplateBase
{
    public override double TemplateWidth => 0.0244;

    public override double TemplateHeight => 0.0244;

    public override AreaDynamic TemplateSearchArea => new(0.6632, 0.688, 0, 1);

    public override double DialogOptionWidth => 0.222;

    public override double DialogOptionHeight => 0.0244;

    public override double Gap => 0.00436;

    public override double BackgroundPadding => 0.0001;

    public override double OutlineAreaHeight => 0.936;

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.075, 0.15, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.14, 0.21, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.28, 0.79, 0.88, 0.07),
        new ThresholdAreaDynamic(0.135, 0.2, 0.71, 0.79, 0.07)
    };
}
