using System.Collections.Generic;
using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Templates;

public class DynamicTemplate1P77 : DynamicTemplateBase
{
    #region Measurements
    public override double TemplateWidth => 0.032;

    public override double TemplateHeight => 0.033;

    public override AreaDynamic TemplateSearchArea => new(0.66, 0.692, 0, 1);

    public override double DialogOptionWidth => 0.29;

    public override double DialogOptionHeight => 0.033;

    public override double Gap => 0.0056;

    public override double BackgroundPadding => 0.0015;

    public override double OutlineAreaHeight => 0.95;
    #endregion

    #region Lines
    public override RangeDynamic VerticalOutlineSearchRangeX => new(0, 0.1);

    public override RangeDynamic VerticalOutlineSearchRangeY => new(0.47, 0.52);

    public override double VerticalOutlineThreshold => 1;

    public override RangeDynamic HorizontalOutlineSearchRangeX => new(0.5, 1);

    public override double HorizontalOutlineThreshold => 0.9;

    public override RangeDynamic TopOutlineSearchRangeY => new(0, 0.1);

    public override RangeDynamic BottomOutlineSearchRangeY => new(0.9, 1);

    public override List<ThresholdAreaDynamic> CornerOutlineAreas => new()
    {
        // Upper
        new ThresholdAreaDynamic(0.225, 0.3, 0.05, 0.15, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.125, 0.2, 0.07),
        
        // Bottom
        new ThresholdAreaDynamic(0.225, 0.3, 0.8, 0.875, 0.07),
        new ThresholdAreaDynamic(0.15, 0.225, 0.75, 0.825, 0.07)
    };
    #endregion
}
