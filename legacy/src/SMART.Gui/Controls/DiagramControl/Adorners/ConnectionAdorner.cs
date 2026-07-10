namespace SMART.Gui.Controls.DiagramControl.Adorners
{
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    using Shapes;
    using View;

    using ViewModel;

    public class ConnectionAdorner : Adorner
    {
        private readonly TransitionViewModel viewModel;
        private readonly DiagramCanvas designerCanvas;
        private readonly Canvas adornerCanvas;
        private readonly TransitionShape connection;
        private PathGeometry pathGeometry;
        private IConnectable fixConnector, dragConnector;
        private Thumb sourceDragThumb, sinkDragThumb;
        private readonly Pen drawingPen;

        //private DesignerItem hitDesignerItem;
        //private DesignerItem HitDesignerItem
        //{
        //    get { return hitDesignerItem; }
        //    set
        //    {
        //        if (hitDesignerItem != value)
        //        {
        //            if (hitDesignerItem != null)
        //                hitDesignerItem.IsDragConnectionOver = false;

        //            hitDesignerItem = value;

        //            if (hitDesignerItem != null)
        //                hitDesignerItem.IsDragConnectionOver = true;
        //        }
        //    }
        //}

        private IConnectable hitConnector;
        private IConnectable HitConnector
        {
            get { return hitConnector; }
            set
            {
                if (hitConnector != value)
                {
                    hitConnector = value;
                }
            }
        }

        private VisualCollection visualChildren;
        protected override int VisualChildrenCount
        {
            get
            {
                return this.visualChildren.Count;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            return this.visualChildren[index];
        }

        public ConnectionAdorner(DiagramCanvas designer, TransitionShape connection)
            : base(designer)
        {
            this.designerCanvas = designer;
            viewModel = connection.DataContext as TransitionViewModel;
            adornerCanvas = new Canvas();
            this.visualChildren = new VisualCollection(this);
            this.visualChildren.Add(adornerCanvas);

            this.connection = connection;
            this.connection.PropertyChanged += this.AnchorPositionChanged;

            InitializeDragThumbs();

            drawingPen = new Pen(Brushes.LightSlateGray, 1);
            drawingPen.LineJoin = PenLineJoin.Round;

            Unloaded += this.ConnectionAdorner_Unloaded;
        }
                

        void AnchorPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("AnchorPositionSource"))
            {
                Canvas.SetLeft(sourceDragThumb, connection.AnchorPositionSource.X);
                Canvas.SetTop(sourceDragThumb, connection.AnchorPositionSource.Y);
            }

            if (e.PropertyName.Equals("AnchorPositionSink"))
            {
                Canvas.SetLeft(sinkDragThumb, connection.AnchorPositionSink.X);
                Canvas.SetTop(sinkDragThumb, connection.AnchorPositionSink.Y);
            }
        }

        void thumbDragThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (HitConnector != null)
            {
                if (connection != null)
                {
                    if (viewModel.Source == fixConnector)
                    {
                        designerCanvas.UpdateTransitionDestination(viewModel, this.HitConnector);
                        viewModel.Target = this.HitConnector;
                    }
                    else
                    {
                        designerCanvas.UpdateTransitionSource(viewModel, this.hitConnector);
                        viewModel.Source = this.HitConnector;                        
                    }
                }
            }

            //this.HitDesignerItem = null;
            this.HitConnector = null;
            this.pathGeometry = null;
            this.connection.StrokeDashArray = null;
            this.InvalidateVisual();
        }

        void thumbDragThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            //this.HitDesignerItem = null;
            this.HitConnector = null;
            this.pathGeometry = null;
            this.Cursor = Cursors.Cross;
            this.connection.StrokeDashArray = new DoubleCollection(new double[] { 1, 2 });

            if (sender == sourceDragThumb)
            {
                fixConnector = viewModel.Target;
                dragConnector = viewModel.Source;
            }
            else if (sender == sinkDragThumb)
            {
                dragConnector = viewModel.Target;
                fixConnector = viewModel.Source;
            }
        }

        void thumbDragThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Point currentPosition = Mouse.GetPosition(this);
            this.HitTesting(currentPosition);
            this.pathGeometry = UpdatePathGeometry(currentPosition);
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawGeometry(null, drawingPen, this.pathGeometry);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            adornerCanvas.Arrange(new Rect(0, 0, this.designerCanvas.ActualWidth, this.designerCanvas.ActualHeight));
            return finalSize;
        }

        private void ConnectionAdorner_Unloaded(object sender, RoutedEventArgs e)
        {
            sourceDragThumb.DragDelta -= this.thumbDragThumb_DragDelta;
            sourceDragThumb.DragStarted -= this.thumbDragThumb_DragStarted;
            sourceDragThumb.DragCompleted -= this.thumbDragThumb_DragCompleted;

            sinkDragThumb.DragDelta -= this.thumbDragThumb_DragDelta;
            sinkDragThumb.DragStarted -= this.thumbDragThumb_DragStarted;
            sinkDragThumb.DragCompleted -= this.thumbDragThumb_DragCompleted;
        }

        private void InitializeDragThumbs()
        {
            Style dragThumbStyle = connection.FindResource("ConnectionAdornerThumbStyle") as Style;

            //source drag thumb
            sourceDragThumb = new Thumb();
            Canvas.SetLeft(sourceDragThumb, connection.AnchorPositionSource.X);
            Canvas.SetTop(sourceDragThumb, connection.AnchorPositionSource.Y);
            this.adornerCanvas.Children.Add(sourceDragThumb);
            if (dragThumbStyle != null)
                sourceDragThumb.Style = dragThumbStyle;

            sourceDragThumb.DragDelta += this.thumbDragThumb_DragDelta;
            sourceDragThumb.DragStarted += this.thumbDragThumb_DragStarted;
            sourceDragThumb.DragCompleted += this.thumbDragThumb_DragCompleted;

            // sink drag thumb
            sinkDragThumb = new Thumb();
            Canvas.SetLeft(sinkDragThumb, connection.AnchorPositionSink.X);
            Canvas.SetTop(sinkDragThumb, connection.AnchorPositionSink.Y);
            this.adornerCanvas.Children.Add(sinkDragThumb);
            if (dragThumbStyle != null)
                sinkDragThumb.Style = dragThumbStyle;

            sinkDragThumb.DragDelta += this.thumbDragThumb_DragDelta;
            sinkDragThumb.DragStarted += this.thumbDragThumb_DragStarted;
            sinkDragThumb.DragCompleted += this.thumbDragThumb_DragCompleted;
        }

        private PathGeometry UpdatePathGeometry(Point position)
        {
            PathGeometry geometry = new PathGeometry();

            //ConnectorOrientation targetOrientation;
            //if (HitConnector != null)
            //    targetOrientation = HitConnector.Orientation;
            //else
            //    targetOrientation = dragConnector.Orientation;

            //List<Point> linePoints = PathFinder.GetConnectionLine(fixConnector.GetInfo(), position, targetOrientation);

            //if (linePoints.Count > 0)
            //{
            //    PathFigure figure = new PathFigure();
            //    figure.StartPoint = linePoints[0];
            //    linePoints.Remove(linePoints[0]);
            //    figure.Segments.Add(new PolyLineSegment(linePoints, true));
            //    geometry.Figures.Add(figure);
            //}

            PathFigure figure = new PathFigure();
            figure.StartPoint = new Point(fixConnector.Left, fixConnector.Top); //connection.StartPoint;
            figure.Segments.Add(new LineSegment(position, true));
            geometry.Figures.Add(figure);

            return geometry;
        }

        private void HitTesting(Point hitPoint)
        {
            var hitObject = designerCanvas.InputHitTest(hitPoint) as DependencyObject;
            
            while (hitObject != null && !(hitObject is DiagramCanvas))
            {
                if (hitObject is FrameworkElement)
                {                    
                    HitConnector = (hitObject as FrameworkElement).DataContext as IConnectable;
                    return;
                }
                hitObject = VisualTreeHelper.GetParent(hitObject);
            }

            HitConnector = null;
        }
    }
}
