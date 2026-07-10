namespace SMART.Gui.Controls.DiagramControl.View
{
    using System.ComponentModel.Design;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;

    using Adorners;

    using IOC;

    public class RubberbandSelectionExtension : IExtension<DiagramCanvas>
    {
        public ISelectionService SelectionService { get; set; }

        private DiagramCanvas view;
        private DiagramView itemHost;
        private Point? rubberbandSelectionStartPoint = null;


        public void Attach(DiagramCanvas owner)
        {
            this.view = owner;
            itemHost = view.DiagramViewControl;
            this.SelectionService = view.SelectionService;// Resolver.Resolve<ISelectionService>();

            itemHost.PreviewMouseDown += this.view_PreviewMouseDown;
            itemHost.PreviewMouseMove += this.view_PreviewMouseMove;
            itemHost.PreviewMouseUp += this.itemHost_PreviewMouseUp;
        }

        void itemHost_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (view.EditMode == DiagramCanvas.EditorMode.ZoomRect)
            //{
            //    var pos = e.GetPosition(view.DiagramViewControl);
            //    var zoomRect = new Rect(rubberbandSelectionStartPoint.Value, pos);
            //    view.ZoomRect(zoomRect);
            //}
        }



        public void Detach(DiagramCanvas owner)
        {
            itemHost.PreviewMouseDown -= this.view_PreviewMouseDown;
            itemHost.PreviewMouseMove -= this.view_PreviewMouseMove;
            itemHost.PreviewMouseUp -= this.itemHost_PreviewMouseUp;
        }

        void view_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (view.EditMode == DiagramCanvas.EditorMode.ZoomRect || Keyboard.Modifiers == ModifierKeys.Shift)
            {
                var itemPart = e.OriginalSource as FrameworkElement;
                if (itemPart == null)
                    return;

                var selectable = itemPart.DataContext as ISelectable;
                if (selectable == null)
                {
                    this.rubberbandSelectionStartPoint = new Point?(e.GetPosition(view.DiagramViewControl));                    
                }

            }
        }

        void view_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (view.EditMode == DiagramCanvas.EditorMode.ZoomRect || Keyboard.Modifiers == ModifierKeys.Shift)
            {
                // if mouse button is not pressed we have no drag operation, ...
                if (e.LeftButton != MouseButtonState.Pressed)
                    this.rubberbandSelectionStartPoint = null;

                // ... but if mouse button is pressed and start
                // point value is set we do have one
                if (this.rubberbandSelectionStartPoint.HasValue)
                {
                    // create rubberband adorner
                    var adornerLayer = AdornerLayer.GetAdornerLayer(view.DiagramViewControl);
                    if (adornerLayer != null)
                    {
                        var adorner = new RubberbandAdorner(view, view.DiagramViewControl, SelectionService, rubberbandSelectionStartPoint);
                        adornerLayer.Add(adorner);

                    }
                }
            }

        }
    }
}
