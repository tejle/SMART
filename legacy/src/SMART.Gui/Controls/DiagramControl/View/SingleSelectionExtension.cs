namespace SMART.Gui.Controls.DiagramControl.View
{
    using System.ComponentModel.Design;
    using System.ServiceModel;
    using System.Windows;
    using System.Windows.Input;

    using Helpers;

    using IOC;

    using ViewModel;

    public class SingleSelectionExtension : IExtension<DiagramCanvas>
    {
        //[Dependency]
        public ISelectionService SelectionService { get; set; }

        private DiagramCanvas view;
        private DiagramView itemHost;

        #region IExtension<DiagramView> Members

        public void Attach(DiagramCanvas owner)
        {
            this.view = owner;
            itemHost = view.DiagramViewControl;
            this.SelectionService = view.SelectionService; // Resolver.Resolve<ISelectionService>();
            itemHost.PreviewMouseLeftButtonDown += this.OnPreviewMouseButtonDown;
            itemHost.PreviewMouseRightButtonDown += this.OnPreviewMouseButtonDown;
       
        }

        public void Detach(DiagramCanvas owner)
        {
            itemHost.PreviewMouseLeftButtonDown -= this.OnPreviewMouseButtonDown;
            itemHost.PreviewMouseRightButtonDown -= this.OnPreviewMouseButtonDown;
        }

        #endregion

        #region Input handling
        protected virtual void OnPreviewMouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (view.IsDragging) return;

            if (e.LeftButton == MouseButtonState.Pressed || e.RightButton == MouseButtonState.Pressed)
            {
                var itemPart = e.OriginalSource as FrameworkElement;
                if (itemPart == null)
                    return;

                var selectable = itemPart.DataContext as ISelectable;                        

                if (selectable == null)
                {
                    this.SelectionService.SetSelectedComponents(null);
                    view.DiagramViewControl.Items.MoveCurrentTo(null);
                    view.DiagramViewControl.Focus();
                }                
                else
                {                                        
                    if (this.SelectionService.SelectionCount > 1 && this.SelectionService.GetComponentSelected(selectable))
                        return;
                    if (this.SelectionService.SelectionCount == 1 && this.SelectionService.GetComponentSelected(selectable))
                        return;

                    this.SelectionService.SetSelectedComponents(new [] {selectable});
                    view.ViewModel.CurrentItem = selectable as IDiagramItem;
                    //view.DiagramViewControl.Items.MoveCurrentTo(selectable);                    
                    // Must do this! Otherwise the keydown wont raise!
                    view.DiagramViewControl.Focus();
                }
            }
        }

       

        #endregion
    }
}