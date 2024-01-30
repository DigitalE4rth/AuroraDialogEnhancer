using AuroraDialogEnhancerExtensions.Dimensions;
using AuroraDialogEnhancerExtensions.Proxy;

namespace Extension.HonkaiStarRail.Presets;

public class PresetConfig : PresetConfigBase
{
    public override CursorConfigBase CursorConfig { get; set; } = new CursorConfig();
}
