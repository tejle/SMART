using System;
using System.Globalization;
using System.Windows.Data;
using SMART.Core;

namespace SMART.Gui.Converters
{
    public class PathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            return StringHelper.ShortenPathname(value.ToString(), int.Parse(parameter.ToString()));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;            
        }
    }
}
