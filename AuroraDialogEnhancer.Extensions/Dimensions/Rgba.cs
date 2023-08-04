namespace AuroraDialogEnhancerExtensions.Dimensions;
public class Rgba
{
    public byte R { get; set; }

    public byte G { get; set; }

    public byte B { get; set; }

    public byte A { get; set; }

    public Rgba(byte a, byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Rgba()
    {
    }
}
