using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate2P37 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.054, 0.0637, 0.0435, 0.066);
    public override double IconMaxLength => 0.036;
    public override AreaDynamic TemplateSearchArea => new(0.7575, 0.833, 0, 1);
    public override RangeDynamic IconHorizontalRange => new(0.7575, 0.777);
    public override RangeDynamic TextHorizontalRange => new(0.782, 0.833);
    public override double TextLineHeight => 0.0277;
    public override double Gap => 0.0075;
    public override double DialogOptionWidth => 0.17;
}
