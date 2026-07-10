namespace SMART.Gui.Controls.DiagramControl.Converters
{
    using System;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Data;
    using System.Windows.Shapes;
using SMART.Gui.Controls.DiagramControl.Shapes;
    using SMART.Gui.Controls.DiagramControl.Helpers;

    public class DefaultAngleCenterBinding : IMultiValueConverter
    {
        private const int margin = 5;
        #region IMultiValueConverter Members
        //value[0] source left 
        //value[1] source top
        //value[2] target left 
        //value[3] target top
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Point centerOffset = (Point)parameter;

            Point p1 = new Point(
                    System.Convert.ToDouble(values[0]) + centerOffset.X,
                    System.Convert.ToDouble(values[1]) + centerOffset.Y);

            Point p2 = new Point(
                    System.Convert.ToDouble(values[2]) + centerOffset.X,
                    System.Convert.ToDouble(values[3]) + centerOffset.Y);

            Rect rect = InterSectionHelper.GetRectWithMargin(
                System.Convert.ToDouble(values[0]),
                System.Convert.ToDouble(values[1]),
                Constants.NODE_WIDTH,
                Constants.NODE_HEIGHT,
                margin);

            //Vector a = new Vector(0, Math.Abs((p1.Y != p2.Y) ? p1.Y - p2.Y : p2.Y));
            //Vector b = new Vector(p2.X - p1.X, p1.Y - p2.Y);
            //double angle = Vector.AngleBetween(b, a);

            //// TODO: provide correct radius for the shape
            ////double radius =  centerOffset.X;
            //double radius = Math.Max(centerOffset.X, centerOffset.Y);
          
            //double x = centerOffset.X * Math.Sin(angle * Math.PI / 180);
            //double y = centerOffset.Y * Math.Cos(angle * Math.PI / 180);

            //Point touchPoint = new Point(p1.X + x, p1.Y - y);

            Point p = InterSectionHelper.IntersectionLineRectangle(p1, p2, rect);
            
            
            return p;
            //return touchPoint;
         
        

        }

       

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}