using AuroraDialogEnhancerExtensions.Dimensions;

namespace AuroraDialogEnhancerExtensions.Proxy;

public abstract class CursorConfigBase
{
    public virtual DynamicPoint InitialPosition       { get; set; } = new (0.15, 0.85);
    public virtual int          InitialPositionX      { get; set; } 
    public virtual int          HiddenCursorPositionY { get; set; } = 3;
    public virtual double       PlacementSmoothness   { get; set; } = 0.1;
    public virtual double       MovementSmoothness    { get; set; } = 3;
}
