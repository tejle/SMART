using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace SMART.Gui.Converters
{
    /// Converter to debug the binding values
    /// </summary>
    public class ObjectToBoolConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value != null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}