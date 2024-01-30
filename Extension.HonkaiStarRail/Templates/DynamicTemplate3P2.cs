using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate3P2 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.158, 0.1652, 0.044, 0.066);
    public override double IconMaxLength => 0.0375;
    public override AreaDynamic TemplateSearchArea => new(0.703, 0.759, 0, 1);
    public override RangeDynamic IconHorizontalRange => new(0.703, 0.717);
    public override RangeDynamic TextHorizontalRange => new(0.7206, 0.759);
    public override double TextLineHeight => 0.0275;
    public override double Gap => 0.0057;
    public override double DialogOptionWidth => 0.12;
}
