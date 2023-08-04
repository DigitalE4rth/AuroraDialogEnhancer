namespace AuroraDialogEnhancerExtensions.Dimensions;

public sealed class RangeDynamic : RangeBase<double>
{
    public RangeDynamic(double from, double to) : base(from, to)
    {
    }

    public RangeDynamic() : base(0, 0)
    {
    }
}
