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

            if (paramEnum == null)
            {
                return DependencyProperty.UnsetValue;
            }

            return paramEnum.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var paramEnum = ParseParameterEnum(targetType, parameter);

            if (paramEnum == null || false.Equals(value))
            {
                return DependencyProperty.UnsetValue;
            }

            return paramEnum;
        }

        private object ParseParameterEnum(Type enumType, object parameter)
        {
            var paramString = parameter as string;

            if (paramString == null || !Enum.IsDefined(enumType, paramString))
            {
                return null;
            }

            return Enum.Parse(enumType, paramString);
        }
    }
}
