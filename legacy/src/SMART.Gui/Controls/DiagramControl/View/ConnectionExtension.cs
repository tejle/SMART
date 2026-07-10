using System.ServiceModel;
using System.Windows;
using System.Windows.Input;
using SMART.Gui.Controls.DiagramControl.Helpers;
using SMART.Gui.ViewModel;

namespace SMART.Gui.Controls.DiagramControl.View
{
    public class ConnectionExtension : IExtension<DiagramCanvas>
    {
        private bool enabled = true;
        private bool isLinkStarted;
        private DiagramView itemHost;

        private IConnection link;
        private bool beenOutside;
        private Cursor oldCursor;
        private IConnectable sourceObject;
        private DiagramCanvas view;
        //private IUIElementStrategy strategy;

        public bool IsActive { get; set; }
        public bool IsSuspended { get; set; }

        public virtual bool CanActivate
        {
            get { return (enabled) ? !IsActive : false; }
        }

        public bool Enabled
        {
            get { return enabled; }
            set
            {
                // disable the tool first if it is active
                if (!value && IsActive)
                    Deactivate();

                enabled = true;
            }
        }

        #region IExtension<DiagramCanvas> Members

        public void Attach(DiagramCanvas owner)
        {
            view = owner;
            itemHost = VisualTreeHelperEx.GetParent<DiagramView>(view) as DiagramView;

            if (itemHost != null)
            {
                itemHost.PreviewMouseLeftButtonDown += DrawingSurface_MouseDown;
                itemHost.MouseMove += DrawingSurface_MouseMove;
                itemHost.PreviewMouseLeftButtonUp += DrawingSurface_MouseUp;
            }
        }

        public void Detach(DiagramCanvas owner)
        {
            itemHost.PreviewMouseLeftButtonDown -= DrawingSurface_MouseDown;
            itemHost.MouseMove -= DrawingSurface_MouseMove;
            itemHost.PreviewMouseLeftButtonUp -= DrawingSurface_MouseUp;
        }

        #endregion

        private void DrawingSurface_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsActive)
            {
                if (link == null)
                {
                    Deactivate();
                    return;
                }

                // We released the button on MyThumb object
                var itemPart = e.OriginalSource as FrameworkElement;
                if (itemPart == null)
                    return;

                var targetObject = itemPart.DataContext as IConnectable;

                if (targetObject != null && targetObject.CanConnect)
                {
                    if (e.GetPosition(view) != link.StartPoint)
                        AddConnection(sourceObject, targetObject);
                    view.SelectionService.SetSelectedComponents(null);
                }
                // exit link drawing mode                
                Deactivate();

                view.IsDragEnabled = true;
            }

            isLinkStarted = false;
            view.IsCreatingTransition = false;

            if (link != null)
            {
                // remove line from the canvas
                view.RemoveElementFromDesigner(link);
                // clear the link variable
                link = null;
            }
        }

        private void DrawingSurface_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsActive && !isLinkStarted)
            {
                if (sourceObject != null)
                {
                    var itemPart = e.OriginalSource as FrameworkElement;
                    if (itemPart == null)
                    {
                        return;
                    }

                    var currentObject = itemPart.DataContext as IConnectable;
                    if (currentObject == null || currentObject != sourceObject)
                    {
                        if (link == null || link.EndPoint != link.StartPoint)
                        {
                            view.IsDragEnabled = false;

                            Point position = e.GetPosition(view);
                            link = CreateElement(position, sourceObject) as IConnection;
                            view.InsertElement(view.Items.Count - 1, link);
                            isLinkStarted = true;
                            view.IsCreatingTransition = true;
                            //sourceObject = source;
                            e.Handled = true;
                        }
                    }
                }
            }

            if (IsActive && isLinkStarted)
            {
                // Set the new link end point to current mouse position                
                link.EndPoint = e.GetPosition(view);
                e.Handled = true;
            }
        }

        private void DrawingSurface_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (view.EditMode == DiagramCanvas.EditorMode.CreateStatesAndTransitions &&
                !Keyboard.IsKeyDown(Key.Space) && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                Activate();
            }
            else
            {
                return;
            }

            if (IsSuspended) return;

            if (IsActive)
            {
                var itemPart = e.OriginalSource as FrameworkElement;
                if (itemPart == null)
                    return;

                var source = itemPart.DataContext as IConnectable;

                if (source != null && source.CanConnect)
                {
                    sourceObject = source;

                    //if (link == null || link.EndPoint != link.StartPoint)
                    //{
                    //    view.IsDragEnabled = false;

                    //    Point position = e.GetPosition(view);
                    //    link = CreateElement(position, source) as IConnection;
                    //    view.InsertElement(1, link);
                    //    isLinkStarted = true;
                    //    view.IsCreatingTransition = true;
                    //    sourceObject = source;
                    //    e.Handled = true;
                    //}
                }
                else
                    Deactivate();
            }
        }

        public void AddConnection(IConnectable source, IConnectable target)
        {
            view.AddTransition(source, target);
        }

        public bool Activate()
        {
            if (Enabled && !IsActive)
            {
                oldCursor = Mouse.OverrideCursor;
                IsActive = true;
            }

            return IsActive;
        }

        public bool Deactivate()
        {
            if (IsActive)
            {
                IsActive = false;
                RestoreCursor();
                return true;
            }
            return false;
        }

        protected void RestoreCursor()
        {
            if (oldCursor != null)
            {
                Mouse.OverrideCursor = oldCursor;
                oldCursor = null;
            }
        }

        public ISelectable CreateElement(Point position, IConnectable source)
        {
            var connection = new TransitionViewModel();
            connection.EndPoint = connection.StartPoint = position;
            connection.Source = source;
            connection.IsTemp = true;
            return connection;
        }
    }
}