using System.Drawing;

namespace AuroraDialogEnhancer.Backend.KeyBinding.Models.InteractionPoints;

public class InteractionPrecisePoint
{
    public string Id { get; }

    public Point Point { get; }

    public InteractionPrecisePoint(string id, Point point)
    {
        Id = id;
        Point = point;
    }
}
