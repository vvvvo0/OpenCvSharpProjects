using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


// Rect 객체를 Visibility (UI 요소의 표시 여부) 값으로 변환하는 컨버터
namespace OpenCvSharpProjects.Converters
{
    internal class RectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Rect rect && !rect.IsEmpty)
            {
                return Visibility.Visible; // Rect 객체가 비어있지 않으면 Visible을 반환한다.
            }
            else
            {
                return Visibility.Collapsed; // Rect 객체가 비어있으면 Collapsed를 반환한다.
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); // ConvertBack 메서드는 구현하지 않는다.
        }
    }
}
