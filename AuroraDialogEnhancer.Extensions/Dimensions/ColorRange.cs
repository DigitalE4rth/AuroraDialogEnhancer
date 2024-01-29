namespace AuroraDialogEnhancerExtensions.Dimensions;

public class ColorRange<T> where T : IColor
{
    public T Low  { get; set; }
    public T High { get; set; }

    public ColorRange(T low, T high)
    {
        Low  = low;
        High = high;
    }
}
