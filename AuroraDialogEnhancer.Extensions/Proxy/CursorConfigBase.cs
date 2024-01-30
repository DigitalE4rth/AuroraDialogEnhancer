using AuroraDialogEnhancerExtensions.Dimensions;

namespace AuroraDialogEnhancerExtensions.Proxy;

public abstract class CursorConfigBase
{
    public virtual DynamicPoint InitialPosition       { get; set; } = new();
    public virtual int          InitialPositionX      { get; set; }
    public virtual int          HiddenCursorPositionY { get; set; }
    public virtual double       PlacementSmoothness   { get; set; }
    public virtual double       MovementSmoothness    { get; set; }
}
