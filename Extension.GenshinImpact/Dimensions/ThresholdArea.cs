using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Dimensions;

public class ThresholdArea : Area
{
    public int Threshold;

    public ThresholdArea(Range width, Range height, int threshold) : base(width, height)
    {
        Threshold = threshold;
    }

    public ThresholdArea(int widthFrom, int widthTo, int heightFrom, int heightTo, int threshold) : base(widthFrom, widthTo, heightFrom, heightTo)
    {
        Threshold = threshold;
    }

    public ThresholdArea()
    {
    }
}