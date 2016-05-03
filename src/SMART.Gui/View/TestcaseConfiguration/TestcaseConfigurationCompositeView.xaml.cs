namespace SMART.Gui.View.TestcaseConfiguration
{
    using ViewModel;

    public partial class TestcaseConfigurationCompositeView
    {
        private IViewModel viewModel;

        public TestcaseConfigurationCompositeView()
        {
            this.InitializeComponent();
            Loaded += new System.Windows.RoutedEventHandler(TestcaseConfigurationCompositeView_Loaded);
        }

        void TestcaseConfigurationCompositeView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel = DataContext as IViewModel;

            if (this.viewModel != null)
            {
                this.viewModel.ViewLoaded();
            }
        }
    }
}