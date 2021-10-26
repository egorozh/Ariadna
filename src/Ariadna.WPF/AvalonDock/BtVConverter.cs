using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Ariadna;

[ValueConversion(typeof(bool), typeof(Visibility))]
public class BtVConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool val && targetType == typeof(Visibility))
        {
            if (val)
                return Visibility.Visible;

            if (parameter is Visibility)
                return parameter;

            return Visibility.Collapsed;
        }

        return Visibility.Visible;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Visibility isVisible)
            return false;

        return isVisible == Visibility.Visible;
    }
}