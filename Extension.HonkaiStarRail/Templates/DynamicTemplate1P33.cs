using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate1P33 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.071, 0.086, 0.032, 0.05);
    public override double IconMaxLength => 0.0285;
    public override RangeDynamic TextHorizontalRange => new(0.7094, 0.779);
    public override double TextLineHeight => 0.022;
    public override double DialogOptionWidth => 0.2275;
}
