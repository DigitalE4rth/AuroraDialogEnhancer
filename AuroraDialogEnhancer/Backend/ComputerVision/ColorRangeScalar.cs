using AuroraDialogEnhancerExtensions.Dimensions;
using OpenCvSharp;

namespace AuroraDialogEnhancer.Backend.ComputerVision;

public class ColorRangeScalar
{
    public Scalar Low { get; set; }

    public Scalar High { get; set; }

    public ColorRangeScalar(Scalar low, Scalar high)
    {
        Low  = low;
        High = high;
    }

    public ColorRangeScalar()
    {
    }

    public ColorRangeScalar(ColorRange colorRange)
    {
        Low  = new Scalar(colorRange.Low!.B,  colorRange.Low!.G,  colorRange.Low!.R,  colorRange.Low!.A);
        High = new Scalar(colorRange.High!.B, colorRange.High!.G, colorRange.High!.R, colorRange.High!.A);
    }
}
