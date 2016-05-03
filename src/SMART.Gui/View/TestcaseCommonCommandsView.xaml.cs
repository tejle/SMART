namespace SMART.Gui.View
{
    using ViewModel;

    /// <summary>
    /// Interaction logic for TestcaseCommonCommandsView.xaml
    /// </summary>
    public partial class TestcaseCommonCommandsView
    {
        private TestcaseCommonCommandsViewModel viewModel;

        public TestcaseCommonCommandsView()
        {
            InitializeComponent();
            Loaded += this.TestcaseCommonCommandsView_Loaded;
        }

        void TestcaseCommonCommandsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            viewModel = DataContext as TestcaseCommonCommandsViewModel;
            if (viewModel != null)
            {
                viewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(viewModel_PropertyChanged);
            }
        }

        void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("ConfigButtonChecked"))
            {
                configButton.IsChecked = viewModel.ConfigButtonChecked;
            }
            if (e.PropertyName.Equals("CodeButtonChecked"))
            {
                codeButton.IsChecked = viewModel.CodeButtonChecked;
            }
            if (e.PropertyName.Equals("ExecuteButtonChecked"))
            {
                executeButton.IsChecked = viewModel.ExecuteButtonChecked;
            }
        }
    }
}
