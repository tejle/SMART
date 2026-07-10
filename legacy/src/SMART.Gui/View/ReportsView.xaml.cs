using System.Windows.Controls;
using SMART.Gui.ViewModel;

namespace SMART.Gui.View
{
    /// <summary>
    /// Interaction logic for ReportsView.xaml
    /// </summary>
    public partial class ReportsView
    {
        public ReportsView()
        {
            InitializeComponent();

            Loaded += new System.Windows.RoutedEventHandler(ReportsView_Loaded);
        }

        void ReportsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = DataContext as ReportsViewModel;
            if (viewModel != null)
            {
                if (!viewModel.ShowScenarioColumn)
                {
                    (reportList.View as GridView).Columns.Remove(scenarioColumn);
                }
                //scenarioColumn.Width = viewModel.ShowScenarioColumn ? 150 : 0;
            }
        }
    }
}
