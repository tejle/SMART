using SMART.Core.DomainModel.Layouts;

namespace SMART.Gui.Controls.DiagramControl.Shapes
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;

    using Adorners;

    using Commands;

    using Helpers;

    using View;

    using ViewModel;

    public enum ArrowSymbol
    {
        None,
        Arrow,
        Diamond
    }

    /// <summary>
    /// Interaction logic for TransitionShape.xaml
    /// </summary>
    public partial class TransitionShape : INotifyPropertyChanged
    {
        private TransitionViewModel ViewModel;
        private Adorner adorner;        

        public DiagramView ItemHost
        {
            get
            {
                return ItemsControl.GetItemsOwner(this) as DiagramView;
            }
        }

        public DiagramCanvas TheCanvas
        {
            get { return VisualTreeHelperEx.GetParent<DiagramCanvas>(this) as DiagramCanvas; }
        }

        // connection path geometry
        private PathGeometry pathGeometry;
        public PathGeometry PathGeometry
        {
            get { return pathGeometry; }
            set
            {
                if (pathGeometry != value)
                {
                    pathGeometry = value;
                    UpdateAnchorPosition();
                    SendPropertyChanged("PathGeometry");
                }
            }
        }

        // between source connector position and the beginning 
        // of the path geometry we leave some space for visual reasons; 
        // so the anchor position source really marks the beginning 
        // of the path geometry on the source side
        private Point anchorPositionSource;
        public Point AnchorPositionSource
        {
            get { return anchorPositionSource; }
            set
            {
                if (anchorPositionSource != value)
                {
                    anchorPositionSource = value;
                    SendPropertyChanged("AnchorPositionSource");
                }
            }
        }

        // slope of the path at the anchor position
        // needed for the rotation angle of the arrow
        private double anchorAngleSource;
        public double AnchorAngleSource
        {
            get { return anchorAngleSource; }
            set
            {
                if (anchorAngleSource != value)
                {
                    anchorAngleSource = value;
                    SendPropertyChanged("AnchorAngleSource");
                }
            }
        }

        // analogue to source side
        private Point anchorPositionSink;
        public Point AnchorPositionSink
        {
            get { return anchorPositionSink; }
            set
            {
                if (anchorPositionSink != value)
                {
                    anchorPositionSink = value;
                    SendPropertyChanged("AnchorPositionSink");
                }
            }
        }
        // analogue to source side
        private double anchorAngleSink;
        public double AnchorAngleSink
        {
            get { return anchorAngleSink; }
            set
            {
                if (anchorAngleSink != value)
                {
                    anchorAngleSink = value;
                    SendPropertyChanged("AnchorAngleSink");
                }
            }
        }

        private ArrowSymbol sourceArrowSymbol = ArrowSymbol.None;
        public ArrowSymbol SourceArrowSymbol
        {
            get { return sourceArrowSymbol; }
            set
            {
                if (sourceArrowSymbol != value)
                {
                    sourceArrowSymbol = value;
                    SendPropertyChanged("SourceArrowSymbol");
                }
            }
        }

        public ArrowSymbol sinkArrowSymbol = ArrowSymbol.Arrow;
        public ArrowSymbol SinkArrowSymbol
        {
            get { return sinkArrowSymbol; }
            set
            {
                if (sinkArrowSymbol != value)
                {
                    sinkArrowSymbol = value;
                    SendPropertyChanged("SinkArrowSymbol");
                }
            }
        }

        // specifies a point at half path length
        private Point labelPosition;
        public Point LabelPosition
        {
            get { return labelPosition; }
            set
            {
                if (labelPosition != value)
                {
                    labelPosition = value;
                    SendPropertyChanged("LabelPosition");
                }
            }
        }

        // pattern of dashes and gaps that is used to outline the connection path
        private DoubleCollection strokeDashArray;
        public DoubleCollection StrokeDashArray
        {
            get
            {
                return strokeDashArray;
            }
            set
            {
                if (strokeDashArray != value)
                {
                    strokeDashArray = value;
                    SendPropertyChanged("StrokeDashArray");
                }
            }
        }

        //public static readonly DependencyProperty StartPointProperty =
        //    DependencyProperty.Register(
        //    "StartPoint",
        //    typeof(Point),
        //    typeof(TransitionShape),
        //    new FrameworkPropertyMetadata(new Point(0, 0),
        //        FrameworkPropertyMetadataOptions.AffectsMeasure));

        //public static readonly DependencyProperty EndPointProperty =
        //    DependencyProperty.Register(
        //    "EndPoint", typeof(Point),
        //    typeof(TransitionShape),
        //    new FrameworkPropertyMetadata(new Point(0, 0),
        //        FrameworkPropertyMetadataOptions.AffectsMeasure));

        private Point startPoint;
        public Point StartPoint
        {
            get { return startPoint; }
            set
            {
                startPoint = value; UpdatePathGeometry();
            }
        }

        private Point endPoint;
        public Point EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; UpdatePathGeometry(); }
        }
        //public Point StartPoint
        //{
        //    get { return (Point)this.GetValue(StartPointProperty); }
        //    set { this.SetValue(StartPointProperty, value); UpdatePathGeometry(); }
        //}

        //public Point EndPoint
        //{
        //    get { return (Point)this.GetValue(EndPointProperty); }
        //    set { this.SetValue(EndPointProperty, value); UpdatePathGeometry(); }
        //}

        public TransitionShape()
        {
            InitializeComponent();            
            Loaded += this.TransitionShape_Loaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var textBox = this.Template.FindName("PART_TextBox", this) as TextBox;
            if (textBox != null)
            {
                textBox.PreviewKeyUp += this.textBox_PreviewKeyUp;
                textBox.LostFocus += this.textBox_LostFocus;
            }
        }

        void textBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            //ViewModel.IsInEditMode = false;
            //TheCanvas.EditLabel(ViewModel);
        }

        private void textBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ViewModel.IsInEditMode = false;
                TheCanvas.EditLabel(ViewModel);
            }
            if (e.Key == Key.Escape)
            {
                ViewModel.IsInEditMode = false;
                ViewModel.Name = ViewModel.OldText;
            }
        }

        void TransitionShape_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel = DataContext as TransitionViewModel;

            if (ViewModel != null)
            {
                startPoint = ViewModel.StartPoint;
                endPoint = ViewModel.EndPoint;
                UpdateSourceAndTarget();
                ViewModel.PropertyChanged += this.viewModel_PropertyChanged;
                ViewModel.View = this;
                if (ViewModel.IsTemp)
                {
                    StrokeDashArray = new DoubleCollection(new double[] { 1, 2 });
                    IsHitTestVisible = false;
                }
                ViewModel.ViewLoaded();
            }
            this.LostFocus += TransitionShape_LostFocus;
            this.UpdatePathGeometry();
        }

        void TransitionShape_LostFocus(object sender, RoutedEventArgs e)
        {
            if (ViewModel.IsInEditMode)
            {
                ViewModel.IsInEditMode = false;
                if (TheCanvas != null)
                {
                    TheCanvas.EditLabel(ViewModel);                    
                }
            }
        }

        public void Delete()
        {
            TheCanvas.RemoveElement(ViewModel);
        }

        void viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Selected"))
            {
                if (ViewModel.Selected)
                {
                    this.ShowAdorner();
                }
                else
                {
                    this.HideAdorner();
                }
            }
            if (e.PropertyName.Equals("Source") || e.PropertyName.Equals("Target"))
            {
                UpdateSourceAndTarget();
                this.UpdatePathGeometry();
            }
            if (e.PropertyName.Equals("StartPoint"))
            {
                StartPoint = ViewModel.StartPoint;
                UpdatePathGeometry();
            }
            if (e.PropertyName.Equals("EndPoint"))
            {
                EndPoint = ViewModel.EndPoint;
                UpdatePathGeometry();
            }
        }

        private void UpdateAnchorPosition()
        {
            Point pathStartPoint, pathTangentAtStartPoint;
            Point pathEndPoint, pathTangentAtEndPoint;
            Point pathMidPoint, pathTangentAtMidPoint;

            // the PathGeometry.GetPointAtFractionLength method gets the point and a tangent vector 
            // on PathGeometry at the specified fraction of its length

            try
            {
                this.PathGeometry.GetPointAtFractionLength(0, out pathStartPoint, out pathTangentAtStartPoint);
                this.PathGeometry.GetPointAtFractionLength(1, out pathEndPoint, out pathTangentAtEndPoint);
                this.PathGeometry.GetPointAtFractionLength(0.5, out pathMidPoint, out pathTangentAtMidPoint);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return;
            }

            // get angle from tangent vector
            this.AnchorAngleSource = Math.Atan2(-pathTangentAtStartPoint.Y, -pathTangentAtStartPoint.X)
                                     * (180 / Math.PI);
            this.AnchorAngleSink = Math.Atan2(pathTangentAtEndPoint.Y, pathTangentAtEndPoint.X) * (180 / Math.PI);

            // add some margin on source and sink side for visual reasons only
            pathStartPoint.Offset(-pathTangentAtStartPoint.X * 5, -pathTangentAtStartPoint.Y * 5);
            pathEndPoint.Offset(pathTangentAtEndPoint.X * 5, pathTangentAtEndPoint.Y * 5);

            this.AnchorPositionSource = pathStartPoint;
            this.AnchorPositionSink = pathEndPoint;
            this.LabelPosition = pathMidPoint;
        }

        private void UpdatePathGeometry()
        {
            var geometry = new PathGeometry();
            var start = ViewModel.Source;
            var stop = ViewModel.Target;
            PathFigure figure;
            if (start == null || stop == null)
            {
                if (start != null)
                {
                    var startBounds = InterSectionHelper.GetRectWithMargin(start.Left, start.Top, start.Width, start.Height, 5);

                    if (IsSelfReference(startBounds))
                    {
                        // Self reference
                        var arcStart = new Point((startBounds.X + startBounds.Width / 2), startBounds.Y);
                        var arcStop = new Point(startBounds.X + startBounds.Width, (startBounds.Y + startBounds.Height / 2));
                        figure = new PathFigure { StartPoint = arcStart };
                        figure.Segments.Add(new ArcSegment(arcStop, new Size(50, 50), 270, true, SweepDirection.Clockwise, true));
                    }
                    else
                    {
                        figure = new PathFigure {StartPoint = this.StartPoint};
                        if (ViewModel.Transition != null && ViewModel.Transition.Tags.ContainsKey(Tags.Walked) && (bool)ViewModel.Transition.Tags[Tags.Walked])
                        {
                            figure.Segments.Add(new LineSegment(EndPoint, true));

                        }
                        else
                        {
                            Point midPoint = GetMidPoint(StartPoint, EndPoint, 0.4);
                            figure.Segments.Add(new BezierSegment(StartPoint, midPoint, EndPoint, true));
                        }
                    }
                }
                else
                {
                    figure = new PathFigure { StartPoint = this.StartPoint };
                    if (ViewModel.Transition != null && ViewModel.Transition.Tags.ContainsKey(Tags.Walked) && (bool)ViewModel.Transition.Tags[Tags.Walked])
                    {
                        figure.Segments.Add(new LineSegment(EndPoint, true));

                    }
                    else
                    {

                        Point midPoint = GetMidPoint(StartPoint, EndPoint, 0.4);
                        figure.Segments.Add(new BezierSegment(StartPoint, midPoint, EndPoint, true));
                    }
                }
                geometry.Figures.Add(figure);
                this.PathGeometry = geometry;
                return;
            }

            var rectSource = InterSectionHelper.GetRectWithMargin(start.Left, start.Top, start.Width, start.Height, 5);
            var rectTarget = InterSectionHelper.GetRectWithMargin(stop.Left, stop.Top, stop.Width, stop.Height, 5);

            var s1 = InterSectionHelper.IntersectionLineRectangle(StartPoint, EndPoint, rectSource);
            var s2 = InterSectionHelper.IntersectionLineRectangle(StartPoint, EndPoint, rectTarget);

            if (s1.Y == s2.Y || s1.X == s2.X)
            {

                if (start == stop)
                {
                    var arcStart = new Point((rectSource.X + rectSource.Width / 2), rectSource.Y);
                    var arcStop = new Point(rectSource.X + rectSource.Width, (rectSource.Y + rectSource.Height / 2));
                    figure = new PathFigure { StartPoint = arcStart };
                    figure.Segments.Add(new ArcSegment(arcStop, new Size(50, 50), 270, true, SweepDirection.Clockwise, true));
                }
                else
                {
                    figure = new PathFigure { StartPoint = this.StartPoint };
                    if (ViewModel.Transition != null && ViewModel.Transition.Tags.ContainsKey(Tags.Walked) && (bool)ViewModel.Transition.Tags[Tags.Walked])
                    {
                        figure.Segments.Add(new LineSegment(EndPoint, true));
                    }
                    else
                    {

                        Point midPoint = GetMidPoint(StartPoint, EndPoint, 0.4);
                        figure.Segments.Add(new BezierSegment(StartPoint, midPoint, EndPoint, true));

                    }
                }
                geometry.Figures.Add(figure);
                this.PathGeometry = geometry;
                return;
            }

            if (start == stop)
            {
                var arcStart = new Point((rectSource.X + rectSource.Width / 2), rectSource.Y);
                var arcStop = new Point(rectSource.X + rectSource.Width, (rectSource.Y + rectSource.Height / 2));
                figure = new PathFigure { StartPoint = arcStart };
                figure.Segments.Add(new ArcSegment(arcStop, new Size(50, 50), 270, true, SweepDirection.Clockwise, true));
            }
            else
            {
                figure = new PathFigure { StartPoint = s1 };
                if (ViewModel.Transition != null && ViewModel.Transition.Tags.ContainsKey(Tags.Walked) && (bool)ViewModel.Transition.Tags[Tags.Walked])
                {
                    figure.Segments.Add(new LineSegment(s2, true));

                }
                else
                {
                    Point midPoint = GetMidPoint(s1, s2, 0.4);
                    figure.Segments.Add(new BezierSegment(s1, midPoint, s2, true));

                }
            }
            geometry.Figures.Add(figure);
            this.PathGeometry = geometry;

        }

        private Point GetMidPoint(Point start, Point end, double magnitude)
        {
            var pM = new Point((start.X/2) + (end.X/2), (start.Y/2) + (end.Y/2));
            var pO = new Point(((magnitude*end.Y) - (magnitude*start.Y)), -((magnitude*end.X) - (magnitude*start.X)));
            return new Point(pM.X + pO.X, pM.Y + pO.Y);
        }

        private bool IsSelfReference(Rect startBounds)
        {
            return EndPoint.X < startBounds.Left + startBounds.Width && EndPoint.X > startBounds.Left &&
                   EndPoint.Y > startBounds.Top && EndPoint.Y < startBounds.Top + startBounds.Height;
        }

        private void UpdateSourceAndTarget()
        {
            var stateSource = ViewModel.Source;
            var stateTarget = ViewModel.Target;

            if (stateSource == null || stateTarget == null) return;

            StartPoint = new Point
            {
                X = stateSource.Left + stateSource.Width / 2,
                Y = stateSource.Top + stateSource.Height / 2
            };

            EndPoint = new Point
            {
                X = stateTarget.Left + stateTarget.Width / 2,
                Y = stateTarget.Top + stateTarget.Height / 2
            };
        }

        public void ShowAdorner()
        {
            // the ConnectionAdorner is created once for each Connection
            if (this.adorner == null)
            {
                var designer = VisualTreeHelperEx.GetParent<DiagramCanvas>(this) as DiagramCanvas;

                var adornerLayer = AdornerLayer.GetAdornerLayer(this);
                if (adornerLayer != null)
                {
                    this.adorner = new ConnectionAdorner(designer, this);
                    adornerLayer.Add(this.adorner);
                }
            }
            if (this.adorner != null)
            {
                this.adorner.Visibility = Visibility.Visible;
            }
        }

        public void HideAdorner()
        {
            if (this.adorner != null)
                this.adorner.Visibility = Visibility.Collapsed;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SendPropertyChanged(string propertyName)
        {
            var tmp = PropertyChanged;
            if (tmp != null)
            {
                tmp(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
