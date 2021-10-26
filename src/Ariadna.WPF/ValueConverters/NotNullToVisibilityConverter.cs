using System.Globalization;
using System.Windows;

namespace Ariadna;

public class NotNullToVisibilityConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
        value == null ? Visibility.Collapsed : Visibility.Visible;
    
}