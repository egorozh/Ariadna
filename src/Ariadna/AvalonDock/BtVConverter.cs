using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace Ariadna
{
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BtVConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// Converts a <seealso cref="Boolean"/> value
        /// into a <seealso cref="Visibility"/> value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool val && targetType == typeof(Visibility))
            {
                if (val)
                    return Visibility.Visible;

                if (parameter is Visibility)
                    return parameter;

                return Visibility.Collapsed;
            }

            return Visibility.Visible;
        }

        /// <summary>
        /// Converts a <seealso cref="Visibility"/> value
        /// into a <seealso cref="Boolean"/> value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility isVisible))
                return false;

            return isVisible == Visibility.Visible;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}