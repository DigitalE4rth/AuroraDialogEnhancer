using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate1P5 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.071, 0.085, 0.036, 0.056);
    public override double IconMaxLength => 0.03;
    public override RangeDynamic TextHorizontalRange => new(0.7094, 0.779);
    public override double TextLineHeight => 0.022;
    public override double DialogOptionWidth => 0.2275;
}
