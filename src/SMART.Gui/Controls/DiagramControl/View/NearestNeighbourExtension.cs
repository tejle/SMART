using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMART.Gui.Controls.DiagramControl.View
{
    using System.ServiceModel;
    using System.Windows;

    using Helpers;
    using System.Windows.Input;

    using Shapes;

    using ViewModel;

    class NearestNeighbourExtension : IExtension<DiagramCanvas>
    {
        private DiagramCanvas view;
        private DiagramView itemHost;

        private bool isActive;

        public void Attach(DiagramCanvas owner)
        {
            view = owner;
            itemHost = view.DiagramViewControl;
            itemHost.PreviewMouseMove += this.view_PreviewMouseMove;
        }

        public void Detach(DiagramCanvas owner)
        {
            itemHost.PreviewMouseMove -= this.view_PreviewMouseMove;
        }

        void view_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.N) || view.EditMode == DiagramCanvas.EditorMode.NearestNeighbour)
            {
                view.ViewModel.DiagramItems.OfType<ISelectable>().ToList().ForEach(s => s.IsDimmed = false);

                var dontDimList = new List<ISelectable>();

                var itemPart = e.OriginalSource as FrameworkElement;
                if (itemPart == null)
                    return;

                var item = itemPart.DataContext as ISelectable; 

                //var item = VisualTreeHelperEx.GetParent<ISelectable>((DependencyObject)e.Source) as ISelectable;
                if (item != null)
                {
                    if (item is StateViewModel)
                    {
                        var stateshape = item as StateViewModel;
                        dontDimList.Add(stateshape);

                        var transitions =
                                view.ViewModel.DiagramItems.OfType<TransitionViewModel>().Where(
                                        i => (i.Source == stateshape || i.Target == stateshape)).ToList();
                        transitions.ForEach(
                                t =>
                                {
                                    dontDimList.Add(t);
                                    dontDimList.Add(t.Source as ISelectable);
                                    dontDimList.Add(t.Target as ISelectable);
                                });                        
                    }
                    else if (item is TransitionViewModel)
                    {
                        var transition = item as TransitionViewModel;
                        dontDimList.Add(transition);
                        dontDimList.Add(transition.Source as ISelectable);
                        dontDimList.Add(transition.Target as ISelectable);
                    }

                    foreach (var di in view.ViewModel.DiagramItems.OfType<ISelectable>())
                    {
                        if (!dontDimList.Contains(di))
                        {
                            di.IsDimmed = true;
                        }
                    }

                    isActive = true;
                }
            }
            else
            {
                if (isActive)
                {
                    view.ViewModel.DiagramItems.OfType<ISelectable>().ToList().ForEach(s => s.IsDimmed = false);
                    isActive = false;
                }
            }
        }

    }
}
