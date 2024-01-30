using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate2P38 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.053, 0.0633, 0.043, 0.066);
    public override double IconMaxLength => 0.0375;
    public override AreaDynamic TemplateSearchArea => new(0.7592, 0.833, 0, 1);
    public override RangeDynamic IconHorizontalRange => new(0.7592, 0.779);
    public override RangeDynamic TextHorizontalRange => new(0.7841, 0.833);
    public override double TextLineHeight => 0.0277;
    public override double Gap => 0.0075;
    public override double DialogOptionWidth => 0.17;
}
