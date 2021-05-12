﻿using System;
using System.Globalization;
using System.Windows;

namespace Ariadna
{
    public class NotNullToVisibilityConverter : BaseValueConverter<NotNullToVisibilityConverter>
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value == null ? Visibility.Collapsed : Visibility.Visible;

        public override object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}