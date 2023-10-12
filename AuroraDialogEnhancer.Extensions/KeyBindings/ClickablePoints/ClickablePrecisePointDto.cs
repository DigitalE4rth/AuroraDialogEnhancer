using System.Drawing;

namespace AuroraDialogEnhancerExtensions.KeyBindings.ClickablePoints;

public class ClickablePrecisePointDto
{
    public string Id { get; }

    public Point Point { get; }

    public ClickablePrecisePointDto(string id, Point point)
    {
        Id = id;
        Point = point;
    }
}
