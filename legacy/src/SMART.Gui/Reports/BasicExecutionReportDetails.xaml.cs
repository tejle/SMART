using SMART.Gui.ViewModel;

namespace SMART.Gui.Reports
{
    /// <summary>
    /// Interaction logic for BasicExecutionReportDetails.xaml
    /// </summary>
    public partial class BasicExecutionReportDetails
    {
        public BasicExecutionReportDetails()
            : this(null)
        {
        }

        public BasicExecutionReportDetails(ReportViewModel report)
        {
            DataContext = report;
            InitializeComponent();
        }
    }
}
