using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate2P33 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.0545, 0.0648, 0.0435, 0.067);
    public override double IconMaxLength => 0.039;
    public override AreaDynamic TemplateSearchArea => new(0.7538, 0.833, 0, 1);
    public override RangeDynamic IconHorizontalRange => new(0.7538, 0.773);
    public override RangeDynamic TextHorizontalRange => new(0.779, 0.833);
    public override double TextLineHeight => 0.0285;
    public override double Gap => 0.0076;
    public override double DialogOptionWidth => 0.17;
}
