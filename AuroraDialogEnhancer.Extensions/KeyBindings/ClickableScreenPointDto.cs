using System.Drawing;

namespace AuroraDialogEnhancerExtensions.KeyBindings;

public class ClickableScreenPointDto
{
    public string Id { get; }

    public Point Point { get; }

    public ClickableScreenPointDto(string id, Point point)
    {
        Id = id;
        Point = point;
    }
}
