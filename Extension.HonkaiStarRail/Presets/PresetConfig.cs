using AuroraDialogEnhancerExtensions.Dimensions;
using AuroraDialogEnhancerExtensions.Proxy;

namespace Extension.HonkaiStarRail.Presets;

public class PresetConfig : PresetConfigBase
{
    public override CursorPositionConfig CursorPositionData { get; set; } = new()
    {
        InitialPosition       = new DynamicPoint(0.15, 0.85),
        HiddenCursorPositionY = 1,
        PlacementSmoothness   = 0.2,
        MovementSmoothness    = 10,
    };
}
