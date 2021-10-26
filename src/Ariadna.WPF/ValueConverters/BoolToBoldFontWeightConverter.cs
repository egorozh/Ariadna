using System.Globalization;
using System.Windows;

namespace Ariadna;

public class BoolToBoldFontWeightConverter : BaseValueConverter
{
    public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not bool isVisible) return FontWeights.Normal;
            
        return isVisible ? FontWeights.Bold : FontWeights.Normal;
    }

    public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return true;
    }
}