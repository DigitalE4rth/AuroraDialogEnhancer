using System.Drawing;

namespace AuroraDialogEnhancerExtensions.KeyBindings.InteractionPoints;

public class InteractionPrecisePointDto
{
    public string Id { get; }

    public Point Point { get; }

    public InteractionPrecisePointDto(string id, Point point)
    {
        Id = id;
        Point = point;
    }
}
