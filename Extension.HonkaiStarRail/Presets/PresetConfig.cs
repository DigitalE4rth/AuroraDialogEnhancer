using AuroraDialogEnhancerExtensions.Dimensions;
using AuroraDialogEnhancerExtensions.Proxy;

namespace Extension.HonkaiStarRail.Presets;

public class PresetConfig : PresetConfigBase
{
    public override CursorPositionConfig CursorPositionData { get; set; } = new(
        new DynamicPoint(0.15, 0.85),
        0.2,
        1);
}
