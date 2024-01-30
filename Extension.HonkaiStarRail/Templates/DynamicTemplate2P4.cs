using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate2P4 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.053, 0.063, 0.043, 0.066);
    public override double IconMaxLength => 0.0375;
    public override AreaDynamic TemplateSearchArea => new(0.7602, 0.833, 0, 1);
    public override RangeDynamic IconHorizontalRange => new(0.7602, 0.78);
    public override RangeDynamic TextHorizontalRange => new(0.785, 0.833);
    public override double TextLineHeight => 0.0275;
    public override double Gap => 0.00775;
    public override double DialogOptionWidth => 0.17;
}
