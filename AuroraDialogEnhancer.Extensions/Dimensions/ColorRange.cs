namespace AuroraDialogEnhancerExtensions.Dimensions;

public class ColorRange
{
    public Rgba Low { get; set; }

    public Rgba High { get; set; }

    public ColorRange(Rgba low, Rgba high)
    {
        Low  = low;
        High = high;
    }

    public ColorRange()
    {
        Low = new Rgba();
        High = new Rgba();
    }
}
