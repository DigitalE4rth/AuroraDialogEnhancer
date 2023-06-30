using System;
using System.Globalization;
using System.Windows.Data;

namespace AuroraDialogEnhancer.Frontend.Controls.ColorPicker.Converters;

/// <summary>
/// Converts from range [0 - 1] to percent
/// </summary>
internal class UnitToPercentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return (int) ((double)value * 100);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isParsed = int.TryParse((string) value, out var result);
        if (!isParsed) throw new ArgumentException();
        return result / 100.0;
    }
}
