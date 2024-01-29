namespace AuroraDialogEnhancerExtensions.Dimensions;

public class ChannelRange
{
    public int Low { get; }
    public int High { get; }

    public ChannelRange(int low, int high)
    {
        Low  = low;
        High = high;
    }

    public ChannelRange()
    {
    }
}
