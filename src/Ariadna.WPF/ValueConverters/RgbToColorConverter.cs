using System.Globalization;
using System.Windows.Media;

namespace Ariadna;

public class RgbToColorConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string rgb) return Brushes.BlueViolet;

        var color = ColorConverter.ConvertFromString(rgb);

        if (color is Color color1)
        {
            return new SolidColorBrush(color1);
        }

        return Brushes.BlueViolet;
    }
}