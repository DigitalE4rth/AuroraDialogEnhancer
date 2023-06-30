using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Converters;

/// <summary>
/// Converts hue [0 - 360] to color.
/// </summary>
internal class HueToColorConverter : IValueConverter
{
    private readonly HsvToColorConverter _hsvToColorConverter = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return _hsvToColorConverter.Convert((double)value, 1, 1);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var color = (Color) value;
        return System.Drawing.Color.FromArgb(color.A, color.R, color.G, color.B).GetHue();
    }
}
