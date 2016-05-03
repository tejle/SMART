namespace SMART.Gui.Controls.DiagramControl.Services
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Shapes;

    using View;

    public class SelectionService : ISelectionService
    {
        private DiagramCanvas drawingSurface;
        private List<ISelectable> componentsSelected;

        private bool isMouseDown;
        bool isSelecting;
        Shape rubberband;
        Point initialPoint;
        Rect selectionRect; 

        public SelectionService(DiagramCanvas surface)
        {
            this.drawingSurface = surface;
            this.componentsSelected = new List<ISelectable>();

            //surface.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(surface_PreviewMouseLeftButtonDown);
            //surface.PreviewMouseMove += new MouseEventHandler(surface_PreviewMouseMove);
            //surface.PreviewMouseLeftButtonUp += new MouseButtonEventHandler(surface_PreviewMouseLeftButtonUp);
        }

        private void EnableMultiSelection()
        {
            this.drawingSurface.CaptureMouse();

            if (this.rubberband == null)
            {
                this.rubberband = new Rectangle();
                this.rubberband.Stroke = new SolidColorBrush(Colors.LightGray);
                this.rubberband.StrokeDashArray.Add(5D);
                this.rubberband.StrokeDashArray.Add(3D);
                this.drawingSurface.AddElement<Shape>(this.rubberband);
            }
        }

        private void ClearSelection(bool raiseChangingEvent, bool raiseChangedEvent)
        {
            if (this.componentsSelected.Count > 0)
            {
                if (raiseChangingEvent)
                    this.OnSelectionChanging();

                for (int i = 0; i < this.componentsSelected.Count; i++)
                {
                    if (componentsSelected[i] != null)
                        this.componentsSelected[i].Unselect();
                }

                this.componentsSelected.Clear();

                if (raiseChangedEvent)
                    this.OnSelectionChanged();
            }
        } 

        public event EventHandler SelectionChanged;
        public event EventHandler SelectionChanging;

        protected void OnSelectionChanged()
        {
            if (this.SelectionChanged != null)
                this.SelectionChanged(this, EventArgs.Empty);
        }

        protected void OnSelectionChanging()
        {
            if (this.SelectionChanging != null)
                this.SelectionChanging(this, EventArgs.Empty);
        }

        public bool GetComponentSelected(object component)
        {
            if (component is ISelectable)
                return this.componentsSelected.Contains((ISelectable)component);
            else
                return false;
        }

        public System.Collections.ICollection GetSelectedComponents()
        {
            return this.componentsSelected;
        }

        public object PrimarySelection
        {
            get
            {
                if (this.componentsSelected.Count > 0)
                    return this.componentsSelected[0];
                else
                    return null;
            }
        }

        public int SelectionCount
        {
            get { return this.componentsSelected.Count; }
        }

        public void SetSelectedComponents(ICollection components, SelectionTypes selectionType)
        {
            // Clear selection and raise Changing event
            this.ClearSelection(true, false);

            if (components == null || components.Count == 0)
            {
                this.OnSelectionChanged();
                return;
            }

            foreach (ISelectable selectable in components)
            {
                this.componentsSelected.Add(selectable);
                selectable.Select();
            }

            this.OnSelectionChanged();
        }

        public void SetSelectedComponents(ICollection components)
        {
            // Clear selection and raise Changing event
            this.ClearSelection(true, false);

            if (components == null || components.Count == 0)
            {
                this.OnSelectionChanged();
                return;
            }

            foreach (ISelectable selectable in components)
            {
                this.componentsSelected.Add(selectable);

                if (selectable != null) selectable.Select();
            }

            this.OnSelectionChanged();
        }
    }
}