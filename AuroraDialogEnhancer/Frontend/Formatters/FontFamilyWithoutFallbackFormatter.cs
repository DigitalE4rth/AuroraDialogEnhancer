using System;
using System.Globalization;
using System.Windows.Data;

namespace AuroraDialogEnhancer.Frontend.Formatters;

internal class FontFamilyWithoutFallbackFormatter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var convertedValue = (string) value;
        var indexOfFallback = convertedValue.IndexOf(", ", StringComparison.Ordinal);

        if (convertedValue.Equals(Properties.DefaultSettings.Default.FontStyle_FontFamily)) return value;
        
        return indexOfFallback == -1 
            ? value 
            : convertedValue.Substring(0, indexOfFallback);
    }

    public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
