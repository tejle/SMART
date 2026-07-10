using System;

namespace SMART.Core
{
    using System.Globalization;

    [Serializable]
    public class SmartPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public SmartPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static SmartPoint Empty = new SmartPoint(0, 0);        

        public static SmartPoint Parse(string source)
        {
            var xy = source.Split(',');            
            IFormatProvider cultureInfo = CultureInfo.GetCultureInfo("en-us");
            var point = new SmartPoint(Convert.ToDouble(xy[0], cultureInfo), Convert.ToDouble(xy[1], cultureInfo));
            return point;
        }

 

    }
}
