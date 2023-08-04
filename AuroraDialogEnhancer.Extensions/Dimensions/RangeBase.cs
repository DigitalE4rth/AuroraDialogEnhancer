namespace AuroraDialogEnhancerExtensions.Dimensions;

public abstract class RangeBase<T>
{
    public T From { get; }
    public T To { get; }

    protected RangeBase(T from, T to)
    {
        From = from;
        To = to;
    }
}
