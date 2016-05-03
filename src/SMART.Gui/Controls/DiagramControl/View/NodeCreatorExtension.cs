namespace SMART.Gui.Controls.DiagramControl.View
{
    using System.ComponentModel.Design;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Input;

    using Helpers;

    using IOC;
    using Shapes;
    using ViewModel;

    public class NodeCreatorExtension : IExtension<DiagramCanvas>
    {
        private DiagramCanvas view;
        private DiagramView itemHost;
        private ISelectionService selectionService;

        public void Attach(DiagramCanvas owner)
        {
            this.view = owner;
            itemHost = VisualTreeHelperEx.GetParent<DiagramView>(view) as DiagramView;
            selectionService = Resolver.Resolve<ISelectionService>();
            itemHost.PreviewMouseLeftButtonDown += this.OnPreviewMouseLeftButtonDown;
        }

        void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (view.EditMode == DiagramCanvas.EditorMode.CreateStatesAndTransitions && 
                selectionService.GetSelectedComponents().Count == 0 &&
                !Keyboard.IsKeyDown(Key.Space) && Keyboard.Modifiers != ModifierKeys.Shift)
            {
                if (e.OriginalSource is FrameworkElement &&
                    (e.OriginalSource as FrameworkElement).DataContext is StateViewModel ||
                    (e.OriginalSource as FrameworkElement).DataContext is TransitionViewModel)
                {
                    return;
                }
                if (view.ViewModel.IsGrayed)
                {
                    return;
                }

                //var selectable = VisualTreeHelperEx.GetParent<ISelectable>((DependencyObject)e.OriginalSource) as ISelectable;
                //if (selectable == null) // Nothing was under the mouse, so go ahead
                {
                    view.AddState(e.GetPosition(this.view));
                }
            }
        }

        public void Detach(DiagramCanvas owner)
        {
            itemHost.PreviewMouseLeftButtonDown -= this.OnPreviewMouseLeftButtonDown;
        }

    }
}
