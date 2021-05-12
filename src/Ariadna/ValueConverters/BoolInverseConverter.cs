using System;
using System.Globalization;

namespace Ariadna
{
    public class BoolInverseConverter : BaseValueConverter<BoolInverseConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
                return !booleanValue;

            return value;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}