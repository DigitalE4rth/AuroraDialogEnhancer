using System;
using System.Globalization;
using System.Windows.Data;

namespace AuroraDialogEnhancer.Frontend.Converters;

public class WindowScaleAdjuster : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Properties.Settings.Default.UI_Scale <= 1 
            ? double.Parse((string)parameter) 
            : double.Parse((string) parameter) * Properties.Settings.Default.UI_Scale;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
