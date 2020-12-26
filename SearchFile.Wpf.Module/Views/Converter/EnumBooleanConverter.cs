using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SearchFile.Wpf.Module.Views.Converter
{
    public class EnumBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var paramEnum = ParseParameterEnum(value.GetType(), parameter);

            if (paramEnum is null)
            {
                return DependencyProperty.UnsetValue;
            }

            return paramEnum.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var paramEnum = ParseParameterEnum(targetType, parameter);

            if (paramEnum is null || value is false)
            {
                return DependencyProperty.UnsetValue;
            }

            return paramEnum;
        }

        private static object? ParseParameterEnum(Type enumType, object parameter)
        {
            if (parameter is not string paramString || !Enum.IsDefined(enumType, paramString))
            {
                return null;
            }

            return Enum.Parse(enumType, paramString);
        }
    }
}
