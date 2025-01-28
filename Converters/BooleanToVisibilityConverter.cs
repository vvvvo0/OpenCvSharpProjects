using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenCvSharpProjects.Converters
{
    internal class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolean && boolean)
            {
                return Visibility.Visible; // bool 값이 true이면 Visibility.Visible을 반환합니다.
            }
            else
            {
                return Visibility.Collapsed; // bool 값이 false이면 Visibility.Collapsed를 반환합니다.
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // ConvertBack 메서드는 구현하지 않습니다.
        }
    }
}
