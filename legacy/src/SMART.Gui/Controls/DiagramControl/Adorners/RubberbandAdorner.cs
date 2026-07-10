using SMART.Gui.ViewModel;

namespace SMART.Gui.Controls.DiagramControl.Adorners
{
    using System;
    using System.ComponentModel.Design;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    using Commands;

    using View;
    using System.Collections.Generic;

    public class RubberbandAdorner : Adorner
    {
        private Point? startPoint;
        private Point? endPoint;
        private readonly Pen rubberbandPen;
        private readonly Brush rubberbandBrush;

        private readonly DiagramCanvas theCanvas;

        private readonly DiagramView designerCanvas;

        private readonly ISelectionService selectionService;

        public RubberbandAdorner(DiagramCanvas theCanvas, DiagramView designerCanvas, ISelectionService selectionService, Point? dragStartPoint)
            : base(designerCanvas)
        {
            this.theCanvas = theCanvas;
            this.designerCanvas = designerCanvas;
            this.selectionService = selectionService;
            this.startPoint = dragStartPoint;
            this.rubberbandPen = new Pen(Brushes.SteelBlue, 1);
            //this.rubberbandPen = new Pen(Brushes.LightSlateGray, 1);
            //this.rubberbandPen.DashStyle = new DashStyle(new double[] { 2 }, 1);
            this.rubberbandBrush = Brushes.LightSteelBlue.Clone();
            this.rubberbandBrush.Opacity = 0.5;
            this.rubberbandBrush.Freeze();
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (!this.IsMouseCaptured)
                    this.CaptureMouse();

                this.endPoint = e.GetPosition(designerCanvas);
                if (theCanvas.EditMode != DiagramCanvas.EditorMode.ZoomRect)
                {
                    this.UpdateSelection();
                }
                this.InvalidateVisual();
            }
            else
            {
                if (this.IsMouseCaptured) this.ReleaseMouseCapture();
            }

            e.Handled = true;
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            // release mouse capture
            if (this.IsMouseCaptured) this.ReleaseMouseCapture();

            // remove this adorner from adorner layer
            var adornerLayer = AdornerLayer.GetAdornerLayer(this.designerCanvas);
            if (adornerLayer != null)
                adornerLayer.Remove(this);

            if (theCanvas.EditMode == DiagramCanvas.EditorMode.ZoomRect)
            {
                var zoomRect = SetZoomRect(startPoint.Value, endPoint.Value);// new Rect(startPoint.Value, endPoint.Value);
                //var zoomRect = new Rect(theCanvas.TranslatePointToView(startPoint.Value), theCanvas.TranslatePointToView(endPoint.Value));
                
                if (zoomRect.Size.Width > 10 && zoomRect.Size.Height > 10)
                {
                    theCanvas.ZoomRect(zoomRect, false);
                }
                theCanvas.EditMode = theCanvas.PrevEditMode;
                //(theCanvas.ZoomRectBinding.Command as ZoomRectCommand).ReturnValue = false;
            }

            e.Handled = true;
        }

        private Rect SetZoomRect(Point pt1, Point pt2)
        {
            //Calculate the top left corner of the rectangle 
            //regardless of drag direction
            double x = pt1.X < pt2.X ? pt1.X : pt2.X;
            double y = pt1.Y < pt2.Y ? pt1.Y : pt2.Y;

            //Set its size
            double width = Math.Abs(pt2.X - pt1.X);
            double height = Math.Abs(pt2.Y - pt1.Y);

            return new Rect(x, y, width, height);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            // without a background the OnMouseMove event would not be fired!
            // Alternative: implement a Canvas as a child of this adorner, like
            // the ConnectionAdorner does.
            dc.DrawRectangle(Brushes.Transparent, null, new Rect(this.RenderSize));

            if (this.startPoint.HasValue && this.endPoint.HasValue)
                dc.DrawRectangle(rubberbandBrush, this.rubberbandPen, new Rect(this.startPoint.Value, this.endPoint.Value));
            //dc.DrawRectangle(Brushes.Transparent, this.rubberbandPen, new Rect(this.startPoint.Value, this.endPoint.Value));
        }

        private void UpdateSelection()
        {
            this.selectionService.SetSelectedComponents(null);

            var rubberBand = new Rect(this.startPoint.Value, this.endPoint.Value);
            //foreach (Control item in this.designerCanvas.Children)
            var selection = new List<ISelectable>();
            foreach (IDiagramItem item in this.designerCanvas.ItemsSource)
            {
                if (item is ISelectable)
                {
                    var itemAsControl = (item as IViewModel).View;
                    if (itemAsControl != null)
                    {
                        var itemRect = VisualTreeHelper.GetDescendantBounds(itemAsControl);
                        var itemBounds = itemAsControl.TransformToAncestor(this.designerCanvas).TransformBounds(itemRect);

                        if (rubberBand.Contains(itemBounds))
                        {
                            selection.Add(item as ISelectable);
                        }
                    }
                }
            }
            selectionService.SetSelectedComponents(selection);
        }
    }
}