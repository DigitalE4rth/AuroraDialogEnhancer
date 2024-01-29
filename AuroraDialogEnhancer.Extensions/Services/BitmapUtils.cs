using AuroraDialogEnhancerExtensions.Dimensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;

namespace AuroraDialogEnhancerExtensions.Services;

public class BitmapUtils
{
    #region Color range
    public bool IsWithinRange(Bitmap image, ColorRange<Rgba> colorRangeRgb, int x, int y)
    {
        var pixel = image.GetPixel(x, y);
        
        return pixel.R >= colorRangeRgb.Low.Red  && pixel.R <= colorRangeRgb.High.Red &&
               pixel.G >= colorRangeRgb.Low.Green && pixel.G <= colorRangeRgb.High.Green &&
               pixel.B >= colorRangeRgb.Low.Blue  && pixel.B <= colorRangeRgb.High.Blue;
    }

    public bool IsWithinRange(Bitmap image, ColorRange<Hsba> colorRangeHsb, int x, int y)
    {
        var pixel = image.GetPixel(x, y);
        var pixelHue = pixel.GetHue();
        var pixelSaturation = pixel.GetSaturation();
        var pixelBrightness = pixel.GetBrightness();

        return pixelHue        >= colorRangeHsb.Low.Hue  && pixelHue        <= colorRangeHsb.High.Hue  &&
               pixelSaturation >= colorRangeHsb.Low.Saturation && pixelSaturation <= colorRangeHsb.High.Saturation &&
               pixelBrightness >= colorRangeHsb.Low.Brightness  && pixelBrightness <= colorRangeHsb.High.Brightness;
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
        return image.GetPixel(x, y).R < color.Red  &&
               image.GetPixel(x, y).G < color.Green &&
               image.GetPixel(x, y).B < color.Blue;
    }

    public bool IsDarkerThenColor(Bitmap image, Hsba color, int x, int y)
    {
        return image.GetPixel(x, y).GetBrightness() < color.Brightness;
    }

    public int CountInRange(Bitmap image, ColorRange<Rgba> colorRangeRgb, int x, int y, int maxX, int maxY)
    {
        var count = 0;
        for (var xo = x; xo <= maxX; xo++)
        {
            for (var yo = y; yo <= maxY; yo++)
            {
                if (IsWithinRange(image, colorRangeRgb, xo, yo)) count++;
            }
        }

        return count;
    }

    public int CountInRange(Bitmap image, ColorRange<Hsba> colorRangeHsb, int x, int y, int maxX, int maxY)
    {
        var count = 0;
        for (var xo = x; xo <= maxX; xo++)
        {
            for (var yo = y; yo <= maxY; yo++)
            {
                if (IsWithinRange(image, colorRangeHsb, xo, yo)) count++;
            }
        }

        return count;
    }

    public int CountInRange(Bitmap image, ColorRange<Rgba> colorRangeRgb)
    {
        var count = 0;
        for (var x = 0; x < image.Width; x++)
        {
            for (var y = 0; y < image.Height; y++)
            {
                if (IsWithinRange(image, colorRangeRgb, x, y)) count++;
            }
        }

        return count;
    }

    public (int, int) GetFirstLineAndCountInRange(Bitmap image, ColorRange<Rgba> colorRangeRgb)
    {
        var firstLine = GetFirstLinePosition(image, colorRangeRgb);
        return firstLine == -1 
            ? (-1, 0) 
            : (firstLine, CountInRange(image, colorRangeRgb, 0, firstLine,  image.Width - 1, image.Height - 1));
    }

    private int GetFirstLinePosition(Bitmap image, ColorRange<Rgba> colorRangeRgb)
    {
        for (var y = 0; y < image.Height; y++)
        {
            for (var x = 0; x < image.Width; x++)
            {
                if (IsWithinRange(image, colorRangeRgb, x, y)) return y;
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