using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OpenCvSharpProjects.Converters
{
    public class RectToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Rect rect)
            {
                return new Thickness(rect.Left, rect.Top, 0, 0); // 왼쪽, 위쪽 여백을 Rect의 Left, Top 값으로 설정하고, 오른쪽, 아래쪽 여백은 0으로 설정합니다.
            }
            return new Thickness(0); // 기본값으로 0을 반환합니다.
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // ConvertBack 메서드는 구현하지 않습니다.
        }
    }
}
