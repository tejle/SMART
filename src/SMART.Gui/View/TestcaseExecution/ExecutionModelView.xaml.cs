using SMART.Gui.Interfaces;

namespace SMART.Gui.View.TestcaseExecution
{
    using ViewModel;

    /// <summary>
    /// Interaction logic for ExecutionModelView.xaml
    /// </summary>
    public partial class ExecutionModelView
    {
        private IDiagramViewModel viewModel;

        public ExecutionModelView()
        {
            InitializeComponent();
            Loaded += this.ExecutionModelView_Loaded;
        }

        void ExecutionModelView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel = DataContext as IDiagramViewModel;
            if (this.viewModel != null)
            {
                viewModel.DiagramCanvas = diagramViewer.diagramView.DiagramCanvas;
                ((IViewModel)viewModel).ViewLoaded();
                //viewModel.View = this;
                
                //this.viewModel.ViewLoaded();
            }
        }
    }
}
