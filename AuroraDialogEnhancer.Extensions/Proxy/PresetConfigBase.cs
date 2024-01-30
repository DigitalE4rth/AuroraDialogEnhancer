using AuroraDialogEnhancerExtensions.Dimensions;

namespace AuroraDialogEnhancerExtensions.Proxy;

public abstract class PresetConfigBase
{
    public virtual CursorPositionConfig CursorPositionData { get; set; } = new()
    {
        InitialPosition       = new DynamicPoint(0.15, 0.85),
        HiddenCursorPositionY = 3,
        PlacementSmoothness   = 0.2,
        MovementSmoothness    = 10,
    };
}
