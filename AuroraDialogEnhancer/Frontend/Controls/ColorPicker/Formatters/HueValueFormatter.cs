using System;
using System.Globalization;
using System.Windows.Data;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Formatters;

internal class HueValueFormatter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isParsed = double.TryParse(value.ToString(), out var result);
        return !isParsed ? value : $"{result:0}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value;
    }
}
