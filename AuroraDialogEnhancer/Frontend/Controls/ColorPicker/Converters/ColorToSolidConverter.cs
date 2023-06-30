using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Converters;

internal class ColorToSolidConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var color = (Color) value;
        color.A = 255;
        return color;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
