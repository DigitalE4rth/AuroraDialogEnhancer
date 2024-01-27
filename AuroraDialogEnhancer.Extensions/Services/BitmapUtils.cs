using AuroraDialogEnhancerExtensions.Dimensions;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AuroraDialogEnhancerExtensions.Services;

public class BitmapUtils
{
    #region Color range
    public bool IsWithinRange(Bitmap image, ColorRange colorRange, int x, int y)
    {
        var pixel = image.GetPixel(x, y);

        return pixel.R >= colorRange.Low.R && pixel.R <= colorRange.High.R &&
               pixel.G >= colorRange.Low.G && pixel.G <= colorRange.High.G &&
               pixel.B >= colorRange.Low.B && pixel.B <= colorRange.High.B;
    }

    public bool IsWithinChannel(Bitmap image, ChannelRange channelRange, int x, int y)
    {
        var pixel = image.GetPixel(x, y);
        return pixel.R >= channelRange.Low && pixel.R <= channelRange.High;
    }

    public bool IsDarkerThenChannel(Bitmap image, int x, int y, ChannelRange channelRange)
    {
        return image.GetPixel(x, y).R < channelRange.Low;
    }

    public bool IsBrighterThenChannel(Bitmap image, ChannelRange channelRange, int x, int y)
    {
        return image.GetPixel(x, y).R > channelRange.High;
    }

    public bool IsDarkerThenColor(Bitmap image, Rgba color, int x, int y)
    {
        return image.GetPixel(x, y).R < color.R &&
               image.GetPixel(x, y).G < color.G &&
               image.GetPixel(x, y).B < color.B;
    }

    public int CountInRange(Bitmap image, ColorRange colorRange, int x, int y, int maxX, int maxY)
    {
        var count = 0;
        for (var xo = x; xo <= maxX; xo++)
        {
            for (var yo = y; yo <= maxY; yo++)
            {
                if (IsWithinRange(image, colorRange, xo, yo)) count++;
            }
        }

        return count;
    }

    public int CountInRange(Bitmap image, ColorRange colorRange)
    {
        var count = 0;
        for (var x = 0; x < image.Width; x++)
        {
            for (var y = 0; y < image.Height; y++)
            {
                if (IsWithinRange(image, colorRange, x, y)) count++;
            }
        }

        return count;
    }

    public (int, int) GetFirstLineAndCountInRange(Bitmap image, ColorRange colorRange)
    {
        var firstLine = GetFirstLinePosition(image, colorRange);
        return firstLine == -1 
            ? (-1, 0) 
            : (firstLine, CountInRange(image, colorRange, 0, firstLine,  image.Width - 1, image.Height - 1));
    }

    private int GetFirstLinePosition(Bitmap image, ColorRange colorRange)
    {
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                if (IsWithinRange(image, colorRange, x, y)) return y;
            }
        }

        return -1;
    }
    #endregion



    #region Geometry painter
    public void DrawRectangles(Image bitmap, List<Rectangle> rectangles)
    {
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

    public void DrawLine(Image bitmap, Point from, Point to)
    {
        using var graphics = Graphics.FromImage(bitmap);
        using var pen = new Pen(Color.LimeGreen, 1);
        graphics.DrawLine(pen, from, to);
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