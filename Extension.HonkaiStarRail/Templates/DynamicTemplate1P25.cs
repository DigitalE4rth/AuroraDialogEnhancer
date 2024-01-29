using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate1P25 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.071, 0.086, 0.06, 0.079);
    public override double IconMaxLength => 0.037;
    public override RangeDynamic TextHorizontalRange => new(0.7086, 0.779);
    public override double TextLineHeight => 0.0215;
}
