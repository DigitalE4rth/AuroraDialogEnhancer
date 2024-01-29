namespace AuroraDialogEnhancerExtensions.Dimensions;

public class Rgba : IColor
{
    public int Red   { get; set; }
    public int Green { get; set; }
    public int Blue  { get; set; }
    public int Alpha { get; set; }

    public Rgba(int r, byte g, byte b, int a)
    {
        Red   = r;
        Green = g;
        Blue  = b;
        Alpha = a;
    }

    public Rgba(int r, byte g, byte b)
    {
        Red   = r;
        Green = g;
        Blue  = b;
        Alpha = 255;
    }

    public Rgba()
    {
    }
}
