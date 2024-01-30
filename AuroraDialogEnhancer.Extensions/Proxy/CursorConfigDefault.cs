using AuroraDialogEnhancerExtensions.Dimensions;

namespace AuroraDialogEnhancerExtensions.Proxy;

internal class CursorConfigDefault : CursorConfigBase
{
    public override DynamicPoint InitialPosition  => new(0.15, 0.85);
    public override int          InitialPositionX => 3;
    public override int          HiddenCursorPositionY { get; set; }
    public override double       PlacementSmoothness => 0.2;
    public override double       MovementSmoothness  => 10;
}
