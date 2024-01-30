using AuroraDialogEnhancerExtensions.Dimensions;

namespace AuroraDialogEnhancerExtensions.Proxy;

public abstract class PresetConfigBase
{
    public virtual CursorConfigBase CursorConfig { get; set; } = new CursorConfigDefault();
}
