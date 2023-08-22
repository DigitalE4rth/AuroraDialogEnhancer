using AuroraDialogEnhancerExtensions.Dimensions;

namespace Extension.GenshinImpact.Dimensions;

public class AreaWithThreshold : Area
{
    public int Threshold;

    public AreaWithThreshold(Range width, Range height, int threshold) : base(width, height)
    {
        Threshold = threshold;
    }

    public AreaWithThreshold(int widthFrom, int widthTo, int heightFrom, int heightTo, int threshold) : base(widthFrom, widthTo, heightFrom, heightTo)
    {
        Threshold = threshold;
    }

    public AreaWithThreshold()
    {
    }
}
