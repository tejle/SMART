namespace SMART.Gui.Controls.DiagramControl.View
{
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    using Helpers;

    using IOC;

    using Shapes;

    using ViewModel;

    public class DragSelectionExtension : IExtension<DiagramCanvas>
    {
        DiagramCanvas view;

        bool isDragging;
        Point startOffset;
        private Point elementStartPos;
        private Point currentPosition;
        IDraggable primaryObject;

        public class DragItemHolder
        {
            public IConnectable Item;
            public Point StartOffset;
            public Point OriginalPosition;
            public Point CurrentPosition;
        }

        private readonly List<DragItemHolder> dragItems = new List<DragItemHolder>();

        public ISelectionService SelectionService { get; set; }

        #region IExtension<DiagramView> Members

        public void Attach(DiagramCanvas owner)
        {
            this.view = owner;
            this.SelectionService = view.SelectionService;// Resolver.Resolve<ISelectionService>();
            owner.PreviewMouseLeftButtonDown += this.OnPreviewMouseLeftButtonDown;
            owner.PreviewMouseMove += this.OnPreviewMouseMove;
            owner.PreviewMouseLeftButtonUp += this.OnPreviewMouseLeftButtonUp;
        }

        public void Detach(DiagramCanvas owner)
        {
            owner.PreviewMouseLeftButtonDown -= this.OnPreviewMouseLeftButtonDown;
            owner.PreviewMouseMove -= this.OnPreviewMouseMove;
            owner.PreviewMouseLeftButtonUp -= this.OnPreviewMouseLeftButtonUp;
        }

        #endregion

        #region Input handlers
        /// <summary>
        /// Called when [preview mouse left button down].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Canvas clicked
            if (e.Source == this || (view.EditMode == DiagramCanvas.EditorMode.CreateStatesAndTransitions && !Keyboard.IsKeyDown(Key.Space)))
            {
                return;
            }

            var itemPart = e.OriginalSource as FrameworkElement;
            if (itemPart == null)
                return;
            var theState = VisualTreeHelperEx.GetParent<StateShape>(itemPart) as StateShape;
            if (theState == null)
                return;

            var draggable = theState.DataContext as IDraggable; 

            if (draggable != null && draggable.CanDrag)
            {
                isDragging = true;

                draggable.IsDragging = true;
                primaryObject = draggable;

                if (SelectionService.SelectionCount > 1)
                {
                    dragItems.Clear();
                    foreach (ISelectable item in SelectionService.GetSelectedComponents())
                    {
                        if (item is TransitionViewModel) continue;

                        if (item is IConnectable)
                        {
                          var i = item as IConnectable;
                          var point = e.GetPosition(((IViewModel) item).View);
                          
                            dragItems.Add(
                                    new DragItemHolder
                                        {
                                                Item = i,
                                                StartOffset = point,
                                                OriginalPosition = (item as IConnectable).Location
                                        });
                        }
                    }
                }
                else
                {
                    this.startOffset = e.GetPosition(theState);
                    if (primaryObject is IConnectable)
                    {
                        elementStartPos = (primaryObject as IConnectable).Location;
                        currentPosition = new Point();
                    }
                }

            }
        }

        /// <summary>
        /// Called when [preview mouse move].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (this.view.IsDragEnabled && this.isDragging)
            {
                view.IsDragging = true;
                var position = Mouse.GetPosition(this.view);

                if (dragItems.Count > 0)
                {
                    foreach (var item in dragItems)
                    {
                        if (item.Item != null)
                        {
                            var left = position.X - item.StartOffset.X;                            
                            item.Item.Left = left;

                            var top = position.Y - item.StartOffset.Y;                            
                            item.Item.Top = top;

                            item.CurrentPosition.X = left;
                            item.CurrentPosition.Y = top;
                          
                        }
                    }
                }
                else
                {
                    if (primaryObject is IConnectable)
                    {
                        var left = position.X - this.startOffset.X;                        
                        (this.primaryObject as IConnectable).Left = left;
                        
                        var top = position.Y - this.startOffset.Y;                        
                        (this.primaryObject as IConnectable).Top = top;

                        currentPosition.X = left;
                        currentPosition.Y = top;
                    }
                }
            }
        }

        /// <summary>
        /// Called when [preview mouse left button up].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.MouseButtonEventArgs"/> instance containing the event data.</param>
        protected virtual void OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.isDragging = false;
            view.IsDragging = false;
            if (this.primaryObject != null)
            {
                (this.primaryObject).IsDragging = false;

                if (dragItems.Count > 0)
                {
                    view.UpdateManyStatesPosition(dragItems);
                }
                else
                {
                    view.UpdateStatePosition(primaryObject, elementStartPos, currentPosition);
                }

                this.primaryObject = null;
                this.dragItems.Clear();
            }
        }
        #endregion
    }
}