using SMART.Gui.ViewModel;

namespace SMART.Gui.Reports
{
    /// <summary>
    /// Interaction logic for BasicExecutionReport.xaml
    /// </summary>
    public partial class BasicExecutionReport
    {
        public BasicExecutionReport()
            : this(null)
        {
        }

        public BasicExecutionReport(ReportViewModel report)
        {
            DataContext = report;
            InitializeComponent();
        }
    }
}
