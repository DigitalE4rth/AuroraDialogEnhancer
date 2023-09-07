using System.Windows;
using System.Windows.Data;

namespace WhyOrchid.Converters;

public class CornerRadiusConverter : IMultiValueConverter
{
    public object Convert(object[] values, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return new CornerRadius(System.Convert.ToDouble(values[0]),
                                System.Convert.ToDouble(values[1]),
                                System.Convert.ToDouble(values[2]),
                                System.Convert.ToDouble(values[3]));
    }

    public object[]? ConvertBack(object value, System.Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        return null;
    }
}
