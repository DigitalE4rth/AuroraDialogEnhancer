using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AuroraDialogEnhancer.Frontend.Converters;

public class UiZoomToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return Math.Abs((double) value - 1) == 0 ? Visibility.Collapsed : Visibility.Visible;
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
