using System;
using System.Windows.Media;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Converters;

internal class HsvToColorConverter
{
    /// <summary>
    /// Creates a color from HSV / HSB values.
    /// </summary>
    /// <param name="hue">Hue, [0 - 360]</param>
    /// <param name="saturation">Saturation, [0, 1]</param>
    /// <param name="value">Value, [0, 1]</param>
    /// <returns>The color created from hsv values.</returns>
    /// <remarks>Algorithm from https://en.wikipedia.org/wiki/HSL_and_HSV #From Hsv</remarks>
    public Color Convert(double hue, double saturation, double value)
    {
        hue        = Clamp(hue, 0, 360);
        saturation = Clamp(saturation, 0, 1);
        value      = Clamp(value, 0, 1);

        byte f(double n)
        {
            var k = (n + hue / 60) % 6;
            var result = value - value * saturation * Math.Max(Math.Min(Math.Min(k, 4 - k), 1), 0);
            return (byte) Math.Round(result * 255);
        }

        return Color.FromRgb(f(5), f(3), f(1));
    }

    private double Clamp(double value, double min, double max)
    {
        if (value < min)
        {
            return min;
        }

        if (value > max)
        {
            return max;
        }

        return value;
    }
}
