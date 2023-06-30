using System;
using System.Windows.Media;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Services;

internal class HsbColorService
{
    /// <summary>
    /// Gets hue in hsb model.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The hue of the color from [0, 360]</returns>
    internal double GetHue(Color color)
    {
        return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B).GetHue();
    }

    /// <summary>
    /// Gets the brightness in hsb model.
    /// </summary>
    /// <param name="color">The color.</param>
    /// <returns>The brightness of the color from [0, 1]</returns>
    internal double GetBrightness(Color color)
    {
        // HSL to HSB conversion
        var c = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        var hslSat = c.GetSaturation();
        var l = c.GetBrightness(); // actual luminance

        return l + hslSat * Math.Min(l, 1 - l);
    }

    /// <summary>
    /// Gets the saturation in hsb model.
    /// </summary>
    /// <param name="color">The color</param>
    /// <returns>The saturation of the color from [0, 1]</returns>
    internal double GetSaturation(Color color)
    {
        // HSL to HSB conversion
        var c = System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B);
        var l = c.GetBrightness();
        var b = GetBrightness(color);

        if (b == 0)
        {
            return 0;
        }

        return 2 - (2 * l / b);
    }
}
