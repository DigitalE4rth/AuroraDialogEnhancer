namespace AuroraDialogEnhancerExtensions.Dimensions;

public sealed class Range : RangeBase<int>
{
    public int Length { get; }

    public Range(int from, int to) : base(from, to)
    {
        Length = to - from;
    }

    public Range() : base(0, 0)
    {
    }
}
