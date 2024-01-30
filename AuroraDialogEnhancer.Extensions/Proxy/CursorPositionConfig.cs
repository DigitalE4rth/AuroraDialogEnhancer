using AuroraDialogEnhancerExtensions.Dimensions;

namespace AuroraDialogEnhancerExtensions.Proxy;

public class CursorPositionConfig
{
    public DynamicPoint InitialPosition       { get; set; } = new();
    public int          InitialPositionX      { get; set; }
    public double       PlacementSmoothness   { get; set; }
    public double       MovementSmoothness    { get; set; }
    public int          HiddenCursorPositionY { get; set; }

    public CursorPositionConfig(DynamicPoint initialPosition,
                                int          hiddenCursorPositionY,
                                double       placementSmoothness,
                                double       movementSmoothness)
    {
        InitialPosition       = initialPosition;
        PlacementSmoothness   = placementSmoothness;
        HiddenCursorPositionY = hiddenCursorPositionY;
        MovementSmoothness    = movementSmoothness;
    }

    public CursorPositionConfig()
    {
    }
}
