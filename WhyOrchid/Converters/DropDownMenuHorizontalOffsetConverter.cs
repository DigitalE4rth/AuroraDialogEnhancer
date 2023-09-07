using System;
using System.Globalization;
using System.Windows.Data;

namespace WhyOrchid.Converters;

public class DropDownMenuHorizontalOffsetConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        return 0 - ((double) values[0] / 2) - ((double) values[1] / 2) + (double) values[2];
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
