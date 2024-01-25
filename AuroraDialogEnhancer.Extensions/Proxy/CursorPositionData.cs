using AuroraDialogEnhancerExtensions.Dimensions;

namespace AuroraDialogEnhancerExtensions.Proxy;

public class CursorPositionConfig
{
    public DynamicPoint InitialPosition     { get; set; } = new();
    public int          InitialPositionX    { get; set; }
    public double       SmoothingPercentage { get; set; }

    public CursorPositionConfig(DynamicPoint initialPosition, double smoothingPercentage)
    {
        InitialPosition     = initialPosition;
        SmoothingPercentage = smoothingPercentage;
    }

    public CursorPositionConfig()
    {
    }
}
