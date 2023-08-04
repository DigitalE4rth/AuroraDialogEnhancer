using AuroraDialogEnhancerExtensions.Dimensions;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace AuroraDialogEnhancerExtensions.Services;

public class BitmapUtils
{
    #region Color range
    public bool IsWithinRange(Bitmap image, int x, int y, ColorRange colorRange)
    {
        var pixel = image.GetPixel(x, y);

        return pixel.R >= colorRange.Low.R && pixel.R <= colorRange.High.R &&
               pixel.G >= colorRange.Low.G && pixel.G <= colorRange.High.G &&
               pixel.B >= colorRange.Low.B && pixel.B <= colorRange.High.B;
    }

    public bool IsWithinGrayRange(Bitmap image, int x, int y, ChannelRange channelRange)
    {
        var pixel = image.GetPixel(x, y);
        return pixel.R >= channelRange.Low && pixel.R <= channelRange.High;
    }

    public int CountInRange(Bitmap image, ColorRange colorRange)
    {
        var count = 0;
        for (var x = 0; x < image.Width; x++)
        {
            for (var y = 0; y < image.Height; y++)
            {
                if (IsWithinRange(image, x, y, colorRange)) count++;
            }
        }

        return count;
    }
    #endregion



    #region Geometry painter
    public void DrawRectangles(Image bitmap, List<Rectangle> rectangles)
    {
        if (!rectangles.Any()) return;

        using var graphics = Graphics.FromImage(bitmap);
        using var pen = new Pen(Color.LimeGreen, 1);
        rectangles.ForEach(r => graphics.DrawRectangle(pen, r));
    }

    public void DrawRectangle(Image bitmap, Rectangle rectangle)
    {
        using var graphics = Graphics.FromImage(bitmap);
        using var pen = new Pen(Color.LimeGreen, 1);
        graphics.DrawRectangle(pen, rectangle);
    }
    #endregion



    #region Gray scale
    private readonly ColorMatrix _grayColorMatrix = new(new[] 
    { 
        new[] { .299f, .299f, .299f, 0, 0 },
        new[] { .587f, .587f, .587f, 0, 0 },
        new[] { .114f, .114f, .114f, 0, 0 },
        new[] { 0f, 0, 0, 1, 0 },
        new[] { 0f, 0, 0, 0, 1 }
    });

    public Bitmap ToGrayScale(Bitmap image)
    {
        var grayImage = new Bitmap(image.Width, image.Height);
        using var attributes = new ImageAttributes();
        attributes.SetColorMatrix(_grayColorMatrix);

        using var graphics = Graphics.FromImage(grayImage);
        graphics.DrawImage(image,
            new Rectangle(0, 0, image.Width, image.Height),
            0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

        return grayImage;
    }
    #endregion



    #region Area cropper
    public Bitmap GetArea(Bitmap image, Rectangle rectangle)
    {
        var croppedImage = new Bitmap(rectangle.Width, rectangle.Height);
        using var graphics = Graphics.FromImage(croppedImage);
        graphics.DrawImage(image, rectangle with { X = 0, Y = 0 }, rectangle, GraphicsUnit.Pixel);
        return croppedImage;
    }
    #endregion
}