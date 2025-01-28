using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;


/*
Rectangle의 Width와 Height 속성은 double 타입의 값을 기대하는데, 
GameWindowRect 속성은 Rect 타입이기 때문에 직접 바인딩할 수 없습니다. 
따라서 Rect 객체의 Width와 Height 값을 double 타입으로 변환해주는 RectToDoubleConverter를 사용해야합니다.
 */

namespace OpenCvSharpProjects.Converters
{
    internal class RectToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Rect rect)
            {
                // parameter에 따라 Width 또는 Height 값을 반환합니다.
                if (parameter is string str && str == "Height")
                {
                    return rect.Height;
                }
                else
                {
                    return rect.Width;
                }
            }
            return 0; // 기본값으로 0을 반환합니다.
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
