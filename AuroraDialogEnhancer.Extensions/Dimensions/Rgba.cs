namespace AuroraDialogEnhancerExtensions.Dimensions;

public class Rgba : IColor
{
    public byte Red   { get; set; }
    public byte Green { get; set; }
    public byte Blue  { get; set; }
    public byte Alpha { get; set; }

    public Rgba(byte r, byte g, byte b, byte a)
    {
        Red   = r;
        Green = g;
        Blue  = b;
        Alpha = a;
    }

    public Rgba(byte r, byte g, byte b)
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
