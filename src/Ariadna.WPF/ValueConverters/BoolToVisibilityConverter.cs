using System.Globalization;
using System.Windows;

namespace Ariadna;

/// <summary>
/// Конвертирует <see cref="bool"/> в <see cref="Visibility"/>
/// </summary>
public class BoolToVisibilityConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool isVisible) return Visibility.Collapsed;

        if (parameter != null)
        {
            if (parameter is Visibility hideType)
                return isVisible ? Visibility.Visible : hideType;
            else
                return isVisible ? Visibility.Collapsed : Visibility.Visible;
        }

        return isVisible ? Visibility.Visible : Visibility.Collapsed;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Visibility visibility) return false;

        if (parameter != null)
            return visibility != Visibility.Visible;

        return visibility == Visibility.Visible;
    }
}