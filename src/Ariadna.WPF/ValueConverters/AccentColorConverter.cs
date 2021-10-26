using System.Globalization;
using System.Windows.Media;

namespace Ariadna;

internal class AccentColorConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string accentValue) return Brushes.Transparent;

        var colorValueAndCommandValue = accentValue
            .Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);

        return new SolidColorBrush(GetColorFromString(colorValueAndCommandValue[0]));
    }

    private static Color GetColorFromString(string colorString)
    {
        //#CCA0522D
        var alpha = byte.Parse(colorString.Substring(1, 2), NumberStyles.AllowHexSpecifier);
        var r = byte.Parse(colorString.Substring(3, 2), NumberStyles.AllowHexSpecifier);
        var g = byte.Parse(colorString.Substring(5, 2), NumberStyles.AllowHexSpecifier);
        var b = byte.Parse(colorString.Substring(7, 2), NumberStyles.AllowHexSpecifier);
        return Color.FromArgb(alpha, r, g, b);
    }
}