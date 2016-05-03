using System;

namespace SMART.Core
{
    using System.Globalization;

    [Serializable]
    public class SmartSize
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public SmartSize(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public static SmartSize Empty = new SmartSize(0, 0);

        public static SmartSize Parse(string source)
        {
            var xy = source.Split(',');
            IFormatProvider cultureInfo = CultureInfo.GetCultureInfo("en-us");
            var point = new SmartSize(Convert.ToDouble(xy[0], cultureInfo), Convert.ToDouble(xy[1], cultureInfo));
            return point;
        }



    }
}
