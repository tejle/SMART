namespace SMART.Gui.View.TestcaseExecution
{
    using System.Windows;

    using ViewModel;

    public partial class TestcaseExecutionCompositeView
    {
        private IViewModel viewModel;

        public TestcaseExecutionCompositeView()
        {
            this.InitializeComponent();
            Loaded += this.TestcaseExecutionCompositeView_Loaded;
        }

        void TestcaseExecutionCompositeView_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = DataContext as IViewModel;
            if (viewModel != null)
            {
                viewModel.ViewLoaded();
            }
        }

        private void ShowLogButton_Click(object sender, RoutedEventArgs e)
        {
            if (logGrid.Visibility == Visibility.Visible)
                logGrid.Visibility = Visibility.Collapsed;
            else
                logGrid.Visibility = Visibility.Visible;
        }
    }
}