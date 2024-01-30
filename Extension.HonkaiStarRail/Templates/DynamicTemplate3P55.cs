using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.HonkaiStarRail.Templates;

public class DynamicTemplate3P55 : DynamicTemplateBase
{
    public override AreaDynamic DialogIndicationArea => new(0.191, 0.1986, 0.0435, 0.066);
    public override double IconMaxLength => 0.039;
    public override AreaDynamic TemplateSearchArea => new(0.682, 0.733, 0, 1);
    public override RangeDynamic IconHorizontalRange => new(0.682, 0.695);
    public override RangeDynamic TextHorizontalRange => new(0.699, 0.733);
    public override double TextLineHeight => 0.0275;
    public override double Gap => 0.005;
    public override double DialogOptionWidth => 0.12;
}
