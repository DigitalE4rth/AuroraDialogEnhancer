using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Dimensions;

public class ThresholdAreaDynamic : AreaDynamic
{
    public double Threshold;

    public ThresholdAreaDynamic(RangeDynamic width, RangeDynamic height, double threshold) : base(width, height)
    {
        Threshold = threshold;
    }

    public ThresholdAreaDynamic(double widthFrom, double widthTo, double heightFrom, double heightTo, double threshold) : base(widthFrom, widthTo, heightFrom, heightTo)
    {
        Threshold = threshold;
    }
}
