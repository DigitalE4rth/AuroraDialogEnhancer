using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate2P30 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.055, 0.066, 0.043, 0.067);
    public override double IconMaxLength => 0.039;
    public override AreaDynamic TemplateSearchArea => new(0.751, 0.8295, 0, 1);
    public override RangeDynamic IconHorizontalRange => new(0.751, 0.770);
    public override RangeDynamic TextHorizontalRange => new(0.776, 0.8295);
    public override double TextLineHeight => 0.0285;
    public override double Gap => 0.0078;
    public override double DialogOptionWidth => 0.17;
}
