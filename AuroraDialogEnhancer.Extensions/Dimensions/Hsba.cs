namespace AuroraDialogEnhancerExtensions.Dimensions;

public class Hsba : IColor
{
    public int    Hue        { get; set; }
    public double Saturation { get; set; }
    public double Brightness { get; set; }
    public int    Alpha      { get; set; }

    public Hsba(int h, double s, double b, int a)
    {
        Hue        = h;
        Saturation = s;
        Brightness = b;
        Alpha      = a;
    }

    public Hsba(int h, double s, double b)
    {
        Hue        = h;
        Saturation = s;
        Brightness = b;
        Alpha      = 255;
    }

    public Hsba()
    {
    }
}
