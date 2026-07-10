using System;

namespace SMART.Gui.Converters
{
    using System.Globalization;
    using System.Windows.Data;

    [ValueConversion(typeof(bool?), typeof(bool))]
    public class RadioButtonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool param = bool.Parse(parameter.ToString());
            if (value == null)
            {
                return false;
            }
            else
            {
                return !((bool)value ^ param);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool param = bool.Parse(parameter.ToString());
            return !((bool)value ^ param);
        }
    }
}
