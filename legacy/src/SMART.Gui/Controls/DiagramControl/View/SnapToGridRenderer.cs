using System.ComponentModel;
using System.Windows.Shapes;

namespace SMART.Gui.Controls.DiagramControl.View
{
  using System;
  using System.Windows;
  using System.Windows.Controls;
  using System.Windows.Media;

  using Helpers;

  public class SnapToGridRenderer : Shape, IDiagramItem
  {
    public static readonly DependencyProperty DotGridProperty = DependencyProperty.Register("DotGrid", typeof(bool), typeof(Grid), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty DotSizeProperty = DependencyProperty.Register("DotSize", typeof(double), typeof(Grid), new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty GridSizeXProperty = DependencyProperty.Register("GridSizeX", typeof(double), typeof(Grid), new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty GridSizeYProperty = DependencyProperty.Register("GridSizeY", typeof(double), typeof(Grid), new FrameworkPropertyMetadata(0.0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty GridStepXProperty = DependencyProperty.Register("GridStepX", typeof(double), typeof(Grid), new FrameworkPropertyMetadata(100.0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));
    public static readonly DependencyProperty GridStepYProperty = DependencyProperty.Register("GridStepY", typeof(double), typeof(Grid), new FrameworkPropertyMetadata(100.0d, FrameworkPropertyMetadataOptions.AffectsRender | FrameworkPropertyMetadataOptions.AffectsMeasure));

    [TypeConverter(typeof(LengthConverter))]
    public double DotSize
    {
      get
      {
        return (double)base.GetValue(DotSizeProperty);
      }
      set
      {
        base.SetValue(DotSizeProperty, value);
      }
    }

    [TypeConverter(typeof(LengthConverter))]
    public bool DotGrid
    {
      get
      {
        return (bool)base.GetValue(DotGridProperty);
      }
      set
      {
        base.SetValue(DotGridProperty, value);
      }
    }

    public double GridSizeX
    {
      get
      {
        return (double)base.GetValue(GridSizeXProperty);
      }
      set
      {
        base.SetValue(GridSizeXProperty, value);
      }
    }

    public double GridSizeY
    {
      get
      {
        return (double)base.GetValue(GridSizeYProperty);
      }
      set
      {
        base.SetValue(GridSizeYProperty, value);
      }
    }

    public double GridStepX
    {
      get
      {
        return (double)base.GetValue(GridStepXProperty);
      }
      set
      {
        base.SetValue(GridStepXProperty, value);
      }
    }

    public double GridStepY
    {
      get
      {
        return (double)base.GetValue(GridStepYProperty);
      }
      set
      {
        base.SetValue(GridStepYProperty, value);
      }
    }


    //private readonly DiagramCanvas canvas;

    //private readonly Brush brush = Brushes.Black;
    //private readonly Brush brushTen;
    //private readonly Brush brushFive;
    //private readonly Brush brushOne;

    //public DiagramCanvas TheCanvas
    //{
    //  get
    //  {
    //    return VisualTreeHelperEx.GetParent<DiagramCanvas>(this) as DiagramCanvas;
    //  }
    //}
    //private Vector originOffset;

    //public SnapToGridRenderer()
    //{
    //  brushOne = brush.Clone();
    //  brushOne.Opacity = 0.1;
    //  brushOne.Freeze();
    //  brushFive = brush.Clone();
    //  brushFive.Opacity = 0.25;
    //  brushFive.Freeze();
    //  brushTen = brush.Clone();
    //  brushTen.Opacity = 0.5;
    //  brushTen.Freeze();

    //  this.HorizontalAlignment = HorizontalAlignment.Stretch;
    //  this.VerticalAlignment = VerticalAlignment.Stretch;

    //  //base.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
    //  base.InvalidateVisual();
    //  Loaded += new RoutedEventHandler(SnapToGridRenderer_Loaded);
    //}

    //void SnapToGridRenderer_Loaded(object sender, RoutedEventArgs e)
    //{
    //  //this.TransformToAncestor(TheCanvas);
    //}

    //public SnapToGridRenderer(DiagramCanvas canvas)
    //{
    //  this.canvas = canvas;
    //  //base.LayoutUpdated += new EventHandler(this.OnLayoutUpdated);
    //  base.InvalidateVisual();

    //}

    protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
    {
      if (!DotGrid)
      {
        Brush brush = this.Fill;
        Brush brushTen;
        Brush brushFive;
        Brush brushOne;

        brushOne = brush.Clone();
        brushOne.Opacity = 0.1;
        brushOne.Freeze();
        brushFive = brush.Clone();
        brushFive.Opacity = 0.25;
        brushFive.Freeze();
        brushTen = brush.Clone();
        brushTen.Opacity = 0.5;
        brushTen.Freeze();

        int sizeX = (int)Math.Floor(GridSizeX / GridStepX) + 2;
        int sizeY = (int)Math.Floor(GridSizeY / GridStepY) + 2;

        for (int i = -10; i < sizeX + 10; i++)
        {
          brush = ((i % 10) == 0) ? brushTen : (((i % 5) == 0) ? brushFive : brushOne);
          drawingContext.DrawRectangle(brush, null, new Rect(i * GridStepX - DotSize, -GridStepY * 10, 2 * DotSize, GridSizeY + GridStepY * 20));
        }

        for (int i = -10; i < sizeY + 10; i++)
        {
          brush = ((i % 10) == 0) ? brushTen : (((i % 5) == 0) ? brushFive : brushOne);
          drawingContext.DrawRectangle(brush, null, new Rect(-GridStepX * 10, i * GridStepY - DotSize, GridSizeX + GridStepX * 20, 2 * DotSize));
        }
      }

      base.OnRender(drawingContext);

      //this.Height = TheCanvas.ActualHeight == 0 ? 10000 : TheCanvas.ActualHeight;
      //this.Width = TheCanvas.ActualWidth == 0 ? 10000 : TheCanvas.ActualWidth;

      //base.OnRender(drawingContext);
      //if (true)//(this.artboard != null) && (this.snappingEngine != null))
      //{
      //  //SnappingOptionsModel snappingOptions = this.snappingEngine.SnappingOptions;
      //  if (true)//snappingOptions.ShowGrid)
      //  {
      //    //double num = snappingOptions.GridSpacing * this.artboard.Zoom;
      //    double num = 10 * 1;
      //    while (num < 8.0)
      //    {
      //      num *= 5.0;
      //      if (num < 8.0)
      //      {
      //        num *= 2.0;
      //      }
      //    }
      //    var point = new Point(0.0, 0.0);// +this.originOffset;

      //    var transform = new TranslateTransform();
      //    point = transform.Transform(point);  //this.TransformFromContentToArtboard(point));
      //    var renderSize = this.RenderSize;
      //    var num2 = (int)Math.Floor((double)((0.0 - point.X) / num));
      //    var num3 = (int)Math.Ceiling((double)((renderSize.Width - point.X) / num));

      //    for (var i = num2; i <= num3; i++)
      //    {
      //      var brush = ((i % 10) == 0) ? brushTen : (((i % 5) == 0) ? brushFive : brushOne);
      //      var x = point.X + (i * num);
      //      drawingContext.DrawRectangle(brush, null, new Rect(x, 0.0, 1.0, renderSize.Height));
      //    }
      //    var num6 = (int)Math.Floor((double)((0.0 - point.Y) / num));
      //    var num7 = (int)Math.Ceiling((double)((renderSize.Height - point.Y) / num));
      //    for (var j = num6; j <= num7; j++)
      //    {
      //      var brush2 = ((j % 10) == 0) ? brushTen : (((j % 5) == 0) ? brushFive : brushOne);
      //      var y = point.Y + (j * num);
      //      drawingContext.DrawRectangle(brush2, null, new Rect(0.0, y, renderSize.Width, 1.0));
      //    }
      //  }
      //}
    }

    //private void OnLayoutUpdated(object sender, EventArgs e)
    //{
    //    if (true)//!this.IsOriginOffsetLocked)
    //    {
    //        Vector vector = new Vector();
    //        if (true)//this.artboard != null)
    //        {
    //            FrameworkElement editableContent = Helpers.VisualTreeHelperEx.GetParent<DiagramCanvas>(this) as DiagramCanvas ;//.EditableContent;
    //            ContentControl control = editableContent as ContentControl;
    //            if (control != null)
    //            {
    //                Visual content = control.Content as Visual;
    //                if ((content != null) && editableContent.IsAncestorOf(content))
    //                {
    //                    Point point = new Point();
    //                    vector = (Vector)(content.TransformToAncestor(editableContent).Transform(point) - point);
    //                }
    //            }
    //        }
    //        if (this.originOffset != vector)
    //        {
    //            this.originOffset = vector;
    //            base.InvalidateVisual();
    //        }
    //    }
    //}

    #region ISelectable Members

    public void Select()
    {
      //throw new NotImplementedException();
    }

    public void Unselect()
    {
      //throw new NotImplementedException();
    }

    #endregion

    public Control View
    {
      get;
      set;
    }

    public void Refresh()
    {
      //
    }

    protected override Geometry DefiningGeometry
    {
      get
      {
        // Create a StreamGeometry for describing the shape
        var geometry = new StreamGeometry { FillRule = FillRule.EvenOdd };

        using (var context = geometry.Open())
        {
          InternalDrawArrowGeometry(context);
        }

        // Freeze the geometry for performance benefits
        geometry.Freeze();

        return geometry;
      }
    }

    private void InternalDrawArrowGeometry(StreamGeometryContext context)
    {
      if (DotGrid)
      {
        int sizeX = (int)Math.Floor(GridSizeX / GridStepX) + 2;
        int sizeY = (int)Math.Floor(GridSizeY / GridStepY) + 2;

        for (int i = 0; i < sizeX; i++)
        {
          for (int j = 0; j < sizeY; j++)
          {

            context.BeginFigure(new Point(i * GridStepX + 0.5, j * GridStepY - DotSize), true, true);
            context.ArcTo(new Point(i * GridStepX - 0.5, j * GridStepY - DotSize),
                new Size(DotSize, DotSize), 360, true, SweepDirection.Clockwise, true, true);
          }
        }
      }
    }
  }
}