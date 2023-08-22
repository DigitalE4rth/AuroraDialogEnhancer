using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Dimensions;

public class AreaDynamicWithThreshold : AreaDynamic
{
    public double Threshold;

    public AreaDynamicWithThreshold(RangeDynamic width, RangeDynamic height, double threshold) : base(width, height)
    {
        Threshold = threshold;
    }

    public AreaDynamicWithThreshold(double widthFrom, double widthTo, double heightFrom, double heightTo, double threshold) : base(widthFrom, widthTo, heightFrom, heightTo)
    {
        Threshold = threshold;
    }
}
