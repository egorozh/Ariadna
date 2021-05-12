using System;
using System.Globalization;
using System.Windows.Media;

namespace Ariadna.Engine
{
    internal class RgbToColorConverter : BaseValueConverter<RgbToColorConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string rgb)) return Brushes.BlueViolet;

            var color = ColorConverter.ConvertFromString(rgb);

            if (color is Color color1)
            {
                return new SolidColorBrush(color1);
            }

            return Brushes.BlueViolet;
        }

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}