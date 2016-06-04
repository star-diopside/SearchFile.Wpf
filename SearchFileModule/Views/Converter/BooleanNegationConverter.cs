﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SearchFile.Views.Converter
{
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanNegationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool ? !(bool)value : DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool ? !(bool)value : DependencyProperty.UnsetValue;
        }
    }
}
