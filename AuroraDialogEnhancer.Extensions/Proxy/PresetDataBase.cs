using AuroraDialogEnhancerExtensions.Dimensions;

namespace AuroraDialogEnhancerExtensions.Proxy;

public abstract class PresetDataBase
{
    public virtual DynamicPoint InitialCursorPosition { get; set; } = new(0.15, 0.85);
    public virtual double CursorSmoothingPercentage { get; set; } = 0.02;
}
