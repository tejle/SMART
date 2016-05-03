namespace SMART.Gui.Controls.DiagramControl.Adorners
{
    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;

    using Helpers;

    using View;
    using Shapes;

    using ViewModel;

    public class ResizeThumb : Thumb
    {
        private Size originalSize;
        private Size currentSize;

        public ResizeThumb()
        {
            DragStarted += this.ResizeThumb_DragStarted;
            DragDelta += this.ResizeThumb_DragDelta;
            DragCompleted += this.ResizeThumb_DragCompleted;
        }

        void ResizeThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            var item = this.DataContext as Control;

            if (item != null)
            {
                originalSize = new Size(item.Width, item.Height);
            }
            
        }

        void ResizeThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            ParentCanvas.IsDragEnabled = true;
            var item = this.DataContext as StateViewModel;

            if (item != null)
            {
                currentSize = new Size(item.Width, item.Height);
                ParentCanvas.ChangeSize(item, originalSize, currentSize);                
            }
        }

        private void ResizeThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var item = this.DataContext as Control;

            if (item != null)
            {
                ParentCanvas.IsDragEnabled = false;

                double deltaVertical, deltaHorizontal;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange,
                            item.ActualHeight - item.MinHeight);
                        item.Height -= deltaVertical;
                        break;
                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange,
                            item.ActualHeight - item.MinHeight);
                        Canvas.SetTop(item, Canvas.GetTop(item) + deltaVertical);
                        item.Height -= deltaVertical;
                        break;
                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange,
                            item.ActualWidth - item.MinWidth);
                        Canvas.SetLeft(item, Canvas.GetLeft(item) + deltaHorizontal);
                        item.Width -= deltaHorizontal;
                        break;
                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange,
                            item.ActualWidth - item.MinWidth);
                        item.Width -= deltaHorizontal;
                        break;
                    default:
                        break;
                }
            }

            e.Handled = true;
        }

        private DiagramCanvas ParentCanvas
        {
            get { return VisualTreeHelperEx.GetParent<DiagramCanvas>(this) as DiagramCanvas; }
        }
    }
}
