using AuroraDialogEnhancerExtensions.Dimensions;

namespace AuroraDialogEnhancerExtensions.Proxy;

public abstract class PresetConfigBase
{
    public virtual CursorPositionConfig CursorPositionData { get; set; } = new(new DynamicPoint(0.15, 0.85), 0.2);
}
