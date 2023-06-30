namespace AuroraDialogEnhancerExtensions.Utilities;
public class Rgba
{
    public int R { get; set; }

    public int G { get; set; }

    public int B { get; set; }

    public int A { get; set; }

    public Rgba(int a, int r, int g, int b)
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
