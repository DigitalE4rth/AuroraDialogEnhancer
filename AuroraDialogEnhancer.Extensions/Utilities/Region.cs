namespace AuroraDialogEnhancerExtensions.Utilities;

public class Region
{
    /// <summary>
    /// X coordinate.
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Y coordinate.
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Rectangle width.
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Rectangle height.
    /// </summary>
    public int Height { get; set; }

    public Region(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Region()
    {
    }
}
