using System;
using System.Globalization;
using System.Windows;

namespace Ariadna
{
    public class BoolToColumnWidthConverter : BaseValueConverter<BoolToColumnWidthConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool isVisible)) return GridLength.Auto;

            if (parameter is int hidenWidth)
            {
                return isVisible ? GridLength.Auto : new GridLength(hidenWidth);
            }

            return isVisible ? GridLength.Auto : new GridLength(0);
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}