using System.Globalization;

namespace Ariadna;

public class BoolInverseConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool booleanValue)
            return !booleanValue;

        return value;
    }
}