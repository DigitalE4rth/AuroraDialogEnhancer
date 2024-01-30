using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate1P770 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.072, 0.0845, 0.043, 0.066);
    public override double IconMaxLength => 0.039;
    public override RangeDynamic TextHorizontalRange => new(0.7094, 0.779);
    public override double TextLineHeight => 0.027;
    public override double DialogOptionWidth => 0.2275;
}
