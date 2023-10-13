using System.Drawing;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.ClickablePoints;

public class ClickablePrecisePoint
{
    public string Id { get; }

    public Point Point { get; }

    public ClickablePrecisePoint(string id, Point point)
    {
        Id = id;
        Point = point;
    }
}
