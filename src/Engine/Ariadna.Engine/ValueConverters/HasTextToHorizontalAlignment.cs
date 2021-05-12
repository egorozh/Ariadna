using System;
using System.Globalization;
using System.Windows;

namespace Ariadna.Engine
{
    internal class HasTextToHorizontalAlignment : BaseValueConverter<HasTextToHorizontalAlignment>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool hasText)) return HorizontalAlignment.Center;

            return hasText ? HorizontalAlignment.Stretch : HorizontalAlignment.Center;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}