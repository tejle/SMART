using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SMART.Gui.Controls.DiagramControl.View
{
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using Helpers;

    using ViewModel;

    public class PanAndZoomExtension : IExtension<DiagramCanvas>
    {
        DiagramCanvas view;

        private DiagramView itemHost;
        private SnapToGridRenderer grid;


        private Color gridColor;
        private DiagramCanvas.GridType eGridType;
        private TranslateTransform translateGrid;
        private double gridStepX;
        private double gridStepY;

        private Point ptPos;
        private Point ptPrevPos;

        private double zoomFactor;

        public void Attach(DiagramCanvas owner)
        {
            view = owner;

            itemHost = view.DiagramViewControl;

            gridColor = Colors.Black;
            eGridType = DiagramCanvas.GridType.LineGrid;
            translateGrid = new TranslateTransform();
            gridStepX = 1;
            gridStepY = 1;

            ptPos = new Point(0, 0);
            ptPrevPos = new Point(0, 0);

            zoomFactor = 0.01;            

            view.Loaded += view_Loaded;

            itemHost.PreviewMouseDown += view_PreviewMouseDown;
            itemHost.PreviewMouseMove += view_PreviewMouseMove;
            itemHost.PreviewMouseUp += view_PreviewMouseUp;
            itemHost.PreviewMouseWheel += itemHost_PreviewMouseWheel;
            itemHost.PreviewMouseDoubleClick += itemHost_PreviewMouseDoubleClick;
        }

        void view_Loaded(object sender, RoutedEventArgs e)
        {
            view.RemoveElementFromDesigner(grid);
            //AddGrid();
        }

        void itemHost_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (view.EditMode != DiagramCanvas.EditorMode.CreateStatesAndTransitions)
            {
                view.AnimateZoom(view.ScaleView.ScaleX + 0.5, view.TranslateView.X, view.TranslateView.Y, 100);
            }
        }

        public void Detach(DiagramCanvas owner)
        {
            itemHost.PreviewMouseDown -= view_PreviewMouseDown;
            itemHost.PreviewMouseMove -= view_PreviewMouseMove;
            itemHost.PreviewMouseUp -= view_PreviewMouseUp;
            itemHost.PreviewMouseWheel -= itemHost_PreviewMouseWheel;
            itemHost.PreviewMouseDoubleClick -= itemHost_PreviewMouseDoubleClick;

        }

        void itemHost_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            //view.ScaleView.CenterX = view.DiagramViewControl.ActualWidth / 2;
            //view.ScaleView.CenterY = view.DiagramViewControl.ActualHeight / 2;

            if (e.Delta > 0)
            {
                view.AnimateZoom(view.ScaleView.ScaleX + 0.15, view.TranslateView.X, view.TranslateView.Y, 100);
                //view.ScaleView.ScaleX += 0.1;
                //view.ScaleView.ScaleY += 0.1;
            }
            else if (e.Delta < 0)
            {
                view.AnimateZoom(view.ScaleView.ScaleX - 0.15, view.TranslateView.X, view.TranslateView.Y, 100);
                //view.ScaleView.ScaleX -= 0.1;
                //view.ScaleView.ScaleY -= 0.1;
            }
        }

        void view_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.Captured == itemHost && (view.EditMode == DiagramCanvas.EditorMode.PanAndZoom || Keyboard.IsKeyDown(Key.Space)))
            {
                Mouse.Capture(null);
                itemHost.Cursor = Cursors.Arrow;
                //AddGrid();

                //if (view.Items() != null)
                //{
                //  var grid = view.Items().OfType<SnapToGridRenderer>().FirstOrDefault();
                //  if (grid != null)
                //  {
                //    Canvas.SetTop(grid, -view.TranslateView.Y);
                //    Canvas.SetLeft(grid, -view.TranslateView.X);
                //    grid.Width = itemHost.ActualWidth;
                //    grid.Height = itemHost.ActualHeight;
                //    grid.Visibility = Visibility.Visible;
                //  }
                //}
            }
        }

        void view_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            ptPrevPos = ptPos;
            ptPos = Mouse.GetPosition(itemHost);

            if (Mouse.Captured == itemHost && (view.EditMode == DiagramCanvas.EditorMode.PanAndZoom || Keyboard.IsKeyDown(Key.Space)))
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Pan();
                    itemHost.Cursor = Cursors.Hand;
                }
                else if (e.RightButton == MouseButtonState.Pressed)
                {
                    Zoom();
                    itemHost.Cursor = Cursors.Cross;
                }
            }
        }

        void view_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Shift || (e.OriginalSource is FrameworkElement &&
                (e.OriginalSource as FrameworkElement).DataContext is StateViewModel ||
                ((FrameworkElement) e.OriginalSource).DataContext is TransitionViewModel))
                return;

            ptPos = Mouse.GetPosition(itemHost);

            // Can also pan when holding down space
            if ((view.EditMode == DiagramCanvas.EditorMode.PanAndZoom || Keyboard.IsKeyDown(Key.Space))
                && (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
                && Mouse.Captured == null)
            {
                Mouse.Capture(itemHost);
                //view.RemoveElementFromDesigner(grid);

                //if (view.Items() != null)
                //{
                //  var grid = view.Items().OfType<SnapToGridRenderer>().FirstOrDefault();
                //  if (grid != null)
                //  {
                //    grid.Visibility = Visibility.Collapsed;
                //  }
                //}
            }

        }

        private void Pan()
        {
            double dX = ptPos.X - ptPrevPos.X;
            double dY = ptPos.Y - ptPrevPos.Y;

            view.TranslateView.X = view.TranslateView.X + dX / view.ScaleView.ScaleX;
            view.TranslateView.Y = view.TranslateView.Y + dY / view.ScaleView.ScaleY;

            view.UpdateViewModel();
        }


        private void Zoom()
        {
            //double dX = ptPos.X - ptPrevPos.X;
            double dY = ptPos.Y - ptPrevPos.Y;

            double scale = zoomFactor * view.ScaleView.ScaleY;

            view.ScaleView.CenterX = view.DiagramViewControl.ActualWidth / 2;
            view.ScaleView.CenterY = view.DiagramViewControl.ActualHeight / 2;

            view.ScaleView.ScaleX = view.ScaleView.ScaleX + dY * scale;
            view.ScaleView.ScaleY = view.ScaleView.ScaleY + dY * scale;

            if (view.ScaleView.ScaleX < 0 || view.ScaleView.ScaleY < 0)
            {
                view.ScaleView.ScaleX = 0.0d;
                view.ScaleView.ScaleY = 0.0d;
            }

            view.UpdateViewModel();

            //var grid = this.GetItems().OfType<SnapToGridRenderer>().FirstOrDefault();
            //if (grid != null)
            //{
            //    Canvas.SetTop(grid, -TranslateView.Y);
            //    Canvas.SetLeft(grid, -TranslateView.X);
            //}
        }

        private void SetGrid(double gridSizeX, double gridSizeY, double gridstepX, double gridstepY, double dotSize)
        {
            grid.GridSizeX = gridSizeX;
            grid.GridSizeY = gridSizeY;
            grid.GridStepX = gridstepX;
            grid.GridStepY = gridstepY;
            grid.DotSize = dotSize;
        }

        private void AddGrid()
        {


            //if (grid == null)
            {
                grid = new SnapToGridRenderer();
                grid.DotGrid = false;

                //grid.RenderTransform = translateGrid;
                //grid.Fill = new SolidColorBrush(gridColor);


            }

            var viewPoint = GetViewPosition();

            var gridstepX = gridStepX;
            var gridstepY = gridStepY;

            gridstepX = gridStepX * 10;
            gridstepY = gridStepY * 10;

            //if (eGridType == DiagramCanvas.GridType.LineGrid)
            //{
            //  grid.DotGrid = false;

            //  gridstepX = gridStepX * 10;
            //  gridstepY = gridStepY * 10;
            //}
            //else
            //{
            //  grid.DotGrid = true;
            //}

            grid.RenderTransform = translateGrid;
            grid.Fill = new SolidColorBrush(gridColor);
            SetGrid(itemHost.ActualWidth / view.ScaleView.ScaleX, itemHost.ActualHeight / view.ScaleView.ScaleX, gridstepX, gridstepY, 1 / view.ScaleView.ScaleX);
            view.RemoveElementFromDesigner(grid);
            view.InsertElement(0, grid);//.Children.Add(grid);


            translateGrid.X = viewPoint.X - Math.IEEERemainder(viewPoint.X, gridstepX);
            translateGrid.Y = viewPoint.Y - Math.IEEERemainder(viewPoint.Y, gridstepY);
        }

        private Point GetViewPosition()
        {
            var pt = new Point
                       {
                           X =
                             -view.TranslateView.X - (1.0d - view.ScaleView.ScaleX) * view.ScaleView.CenterX / view.ScaleView.ScaleX,
                           Y =
                             -view.TranslateView.Y - (1.0d - view.ScaleView.ScaleY) * view.ScaleView.CenterY / view.ScaleView.ScaleY
                       };
            return pt;
        }

    }
}
