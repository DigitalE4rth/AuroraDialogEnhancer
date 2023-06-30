using System.Drawing.Imaging;
using System.Drawing;

namespace AuroraDialogEnhancer.Frontend.Forms.Debug;

internal class BitmapUtils
{
    public Bitmap ToGrayScale(Bitmap image)
    {
        var grayImage = new Bitmap(image.Width, image.Height);

        var colorMatrix = new ColorMatrix(
            new[]
            {
                new[] { .299f, .299f, .299f, 0, 0 },
                new[] { .587f, .587f, .587f, 0, 0 },
                new[] { .114f, .114f, .114f, 0, 0 },
                new[] { 0f, 0, 0, 1, 0 },
                new[] { 0f, 0, 0, 0, 1 }
            });

        using var attributes = new ImageAttributes();
        attributes.SetColorMatrix(colorMatrix);

        using var graphics = Graphics.FromImage(grayImage);
        graphics.DrawImage(image,
            new Rectangle(0, 0, image.Width, image.Height),
            0, 0, image.Width, image.Height, GraphicsUnit.Pixel, attributes);

        return grayImage;
    }

    public Bitmap GetArea(Bitmap image, Rectangle rectangle)
    {
        var croppedImage = new Bitmap(rectangle.Width, rectangle.Height);
        using var graphics = Graphics.FromImage(croppedImage);
        graphics.DrawImage(image, rectangle with { X = 0, Y = 0 }, rectangle, GraphicsUnit.Pixel);
        return croppedImage;
    }
}

