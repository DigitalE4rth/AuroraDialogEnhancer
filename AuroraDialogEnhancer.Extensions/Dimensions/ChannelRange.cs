namespace AuroraDialogEnhancerExtensions.Dimensions;

public class ChannelRange
{
    public byte Low { get; }
    public byte High { get; }

    public ChannelRange(byte low, byte high)
    {
        Low = low;
        High = high;
    }

    public ChannelRange()
    {
    }
}
