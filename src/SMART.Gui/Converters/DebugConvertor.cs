namespace SMART.Gui.Converters
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows.Data;

    /// Converter to debug the binding values
    /// </summary>
    public class DebugConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (Debugger.IsAttached) Debugger.Break();
            return value;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}