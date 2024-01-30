using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate2P35 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.0545, 0.0645, 0.0435, 0.067);
    public override double IconMaxLength => 0.035;
    public override AreaDynamic TemplateSearchArea => new(0.7555, 0.833, 0, 1);
    public override RangeDynamic IconHorizontalRange => new(0.7555, 0.775);
    public override RangeDynamic TextHorizontalRange => new(0.781, 0.833);
    public override double TextLineHeight => 0.0276;
    public override double Gap => 0.00755;
    public override double DialogOptionWidth => 0.17;
}
