namespace SMART.Gui.Controls.DiagramControl.View
{
    using System;
    using System.Windows;

  /// <summary>
    /// Interaction logic for DiagramViewer.xaml
    /// </summary>
    public partial class DiagramViewer
    {
        public DiagramViewer()
        {
            InitializeComponent();
            //AllowNodeCreation = true;
            //this.Zoom = 1;

            Unloaded += this.DiagramViewer_Unloaded;
        }

        void DiagramViewer_Unloaded(object sender, RoutedEventArgs e)
        {
            
        }

        //public static DependencyProperty AllowNodeCreationProperty =
        //        DependencyProperty.RegisterAttached("AllowNodeCreationProperty", typeof(bool), typeof(DiagramViewer));

        //public bool AllowNodeCreation
        //{
        //    get
        //    {
        //        return (bool)this.GetValue(AllowNodeCreationProperty);
        //    }
        //    set
        //    {
        //        SetValue(AllowNodeCreationProperty, value);
        //        (VisualTreeHelperEx.GetChild<DiagramCanvas>(diagramView)).AllowNodeCreation = value;
        //    }
        //}

        protected override void OnInitialized(EventArgs e)
        {
//            ZoomSlider.ValueChanged += this.ZoomSlider_ValueChanged;
            //var eventService = Resolver.Resolve<IEventService>();
            //eventService.GetEvent<LayoutCompleteEvent>().Subscribe(OnLayoutComplete);
            base.OnInitialized(e);
  
        }

    
        
        //private void OnLayoutComplete(SmartPoint obj)
        //{
        //    //UpdateScrollSize();
        //    //scrollViewer.ScrollToHorizontalOffset(obj.X - (diagramView.ActualWidth / 2));
        //    //scrollViewer.ScrollToVerticalOffset(obj.Y - 50);
        //}

        void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //this.Zoom = e.NewValue;
        }
        private void UpdateScrollSize()
        {
            //// Nothing to do if the diagram is empty.
            //if (this.ActualWidth == 0 || this.ActualHeight == 0)
            //    return;

            //Size diagramSize = new Size(
            //        diagramView.ActualWidth * this.Zoom,
            //        diagramView.ActualHeight * this.Zoom);

            //// The grid contains the diagram, set the size of the grid so it's
            //// large enough to allow the diagram to scroll from transition to transition.
            //grid.Width = Math.Max(0, (this.ActualWidth * 2) + diagramSize.Width - 50);
            //grid.Height = Math.Max(0, (this.ActualHeight * 2) + diagramSize.Height - 50);

            
            //AutoScrollToCenter();

        }

        /// <summary>
        /// Center the diagram in the display area.
        /// </summary>
        private void AutoScrollToCenter()
        {
            // Adjust the offset so the diagram appears in the center of 
            // the display area. First get the top-left offset.
            //Point offset = this.GetTopLeftScrollOffset();

            //// Now adjust the offset so the diagram is centered.
            //offset.X += ((this.ActualWidth - (diagramView.ActualWidth * this.Zoom)) / 2);
            //offset.Y += ((this.ActualHeight - (diagramView.ActualHeight * this.Zoom)) / 2);

            //// Before auto scroll, determine the start and end 
            //// points so the scrolling can be animated.
            //Point startLocation = new Point(
            //        scrollViewer.HorizontalOffset,
            //        scrollViewer.VerticalOffset);

            //Point endLocation = new Point(
            //        grid.Width - offset.X - startLocation.X,
            //        grid.Height - offset.Y - startLocation.Y);

            //// Auto scroll the diagram.
            //scrollViewer.ScrollToHorizontalOffset(grid.Width - offset.X);
            //scrollViewer.ScrollToVerticalOffset(grid.Height - offset.Y);

        }
        /// <summary>
        /// Return the offset that positions the diagram in the top-left 
        /// corner, takes into account the zoom level.
        /// </summary>
        //private Point GetTopLeftScrollOffset()
        //{
        //    //// Offset that is returned.
        //    //Point offset = new Point();

        //    //// Empty offset if the diagram is empty.
        //    //if (diagramView.ActualWidth == 0 || diagramView.ActualHeight == 0)
        //    //    return offset;

        //    //// Get the size of the diagram.
        //    //Size diagramSize = new Size(
        //    //        diagramView.ActualWidth * this.Zoom,
        //    //        diagramView.ActualHeight * this.Zoom);

        //    //// Calcualte the offset that positions the diagram in the top-left corner.
        //    //offset.X = this.ActualWidth + diagramSize.Width - (25);
        //    //offset.Y = this.ActualHeight + diagramSize.Height - (25);

        //    //return offset;
        //}
        //public double Zoom
        //{
        //    //get { return ZoomSlider.Value; }
        //    //set
        //    //{
        //    //    if (value >= ZoomSlider.Minimum && value <= ZoomSlider.Maximum)
        //    //    {
        //    //        diagramView.Scale = value;
        //    //        ZoomSlider.Value = value;
        //    //        this.UpdateScrollSize();
        //    //    }
        //    //}
        //}

    }
}