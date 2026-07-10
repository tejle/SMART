using System.Windows;

namespace SMART.Gui.Controls.DiagramControl.Helpers
{
    public class InterSectionHelper
    {
        public struct InterLine
        {
            public Point a;
           
            public Point b;
        }

      

        public static Rect GetRectWithMargin(double left, double top, double width, double height, int margin)
        {
            Rect rect = new Rect(left, top, width, height);
            rect.Inflate(margin, margin);
            return rect;
        }

        public static InterLine[] GetRectWithMargin(double left, double top, double width, double height)
        {
            InterLine[] sides = new InterLine[4];

            sides[0] = new InterLine()
                           {
                               a =new Point(){X= left,Y= top},
                               b =new Point(){X= left+width,Y = top}
                           };
            sides[1] = new InterLine()
                           {
                               a =new Point(){ X=left+width, Y= top},
                               b=new Point(){X=   left+width,Y= top+height}
                           };
            sides[2]=new InterLine()
                         {
                             a=new Point(){X= left+width,Y= top+height},
                             b=new Point(){X= left,Y= top+height}
                         };
            sides[3]= new InterLine()
                          {
                              a=new Point(){X= left,Y= top+height},
                              b=new Point(){X= left,Y= top}
                          };
            return sides;
        }

      

        public static Point IntersectionLineLine(Point a1, Point a2, Point b1, Point b2)
        {
            Point point = new Point();
            double ua_t = (b2.X - b1.X) * (a1.Y - b1.Y) - (b2.Y - b1.Y) * (a1.X - b1.X);
            double ub_t = (a2.X - a1.X) * (a1.Y - b1.Y) - (a2.Y - a1.Y) * (a1.X - b1.X);
            double u_b = (b2.Y - b1.Y) * (a2.X - a1.X) - (b2.X - b1.X) * (a2.Y - a1.Y);

            if (u_b != 0)
            {
                double ua = ua_t / u_b;
                double ub = ub_t / u_b;
                if (0 <= ua && ua <= 1 && 0 <= ub && ub <= 1)
                {
                    point.X = a1.X + ua * (a2.X - a1.X);
                    point.Y = a1.Y + ua * (a2.Y - a1.Y);
                }
            }
            return point;
        }

        public static  Point IntersectionLineRectangle(Point a1, Point a2, InterLine[] rect)
        {
            var inter1 = IntersectionLineLine(rect[0].a, rect[0].b, a1, a2);
            if (inter1.X != 0 && inter1.Y != 0) return inter1;

            var inter2 = IntersectionLineLine(rect[1].a, rect[1].b, a1, a2);
            if (inter2.X != 0 && inter2.Y != 0) return inter2;

            var inter3 = IntersectionLineLine(rect[2].a, rect[2].b, a1, a2);
            if (inter3.X != 0 && inter3.Y != 0) return inter3;

            var inter4 = IntersectionLineLine(rect[3].a, rect[3].b, a1, a2);
            if (inter4.X != 0 && inter4.Y != 0) return inter4;

            Point point = new Point();
            return point;
        }

        public static  Point IntersectionLineRectangle(Point a1, Point a2, Rect rect)
        {

            var inter1 = IntersectionLineLine(rect.BottomRight, rect.TopRight, a1, a2);
            if (inter1.X != 0 && inter1.Y != 0) return inter1; 
            
            var inter2 = IntersectionLineLine(rect.TopRight, rect.TopLeft, a1, a2);
            if (inter2.X != 0 && inter2.Y != 0) return inter2;
            
            var inter3 = IntersectionLineLine(rect.TopLeft, rect.BottomLeft, a1, a2);
            if (inter3.X != 0 && inter3.Y != 0) return inter3;

            var inter4 = IntersectionLineLine(rect.BottomLeft, rect.BottomRight, a1, a2);
            if (inter4.X != 0 && inter4.Y != 0) return inter4;

            Point point = new Point();
            return point;
        }
    }
}