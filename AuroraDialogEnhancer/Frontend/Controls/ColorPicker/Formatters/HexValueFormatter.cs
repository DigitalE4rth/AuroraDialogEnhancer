using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Formatters;

internal class HexValueFormatter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var color = value.ToString();
        if (color[0] != '#')
        {
            return (Color)ColorConverter.ConvertFromString(color.Insert(0, "#"));
        }
        return color;
    }
}
