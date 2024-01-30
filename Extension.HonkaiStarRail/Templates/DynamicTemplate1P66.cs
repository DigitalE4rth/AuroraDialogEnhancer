using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate1P66 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.071, 0.085, 0.04, 0.06);
    public override double IconMaxLength => 0.033;
    public override RangeDynamic TextHorizontalRange => new(0.7094, 0.779);
    public override double TextLineHeight => 0.027;
    public override double DialogOptionWidth => 0.2275;
}
