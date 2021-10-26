using System.Globalization;
using System.Windows.Data;

namespace Ariadna;

public class ActiveDocumentConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IDocumentViewModel)
            return value;

        return Binding.DoNothing;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is IDocumentViewModel)
            return value;

        return Binding.DoNothing;
    }
}