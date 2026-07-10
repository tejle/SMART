using SMART.Gui.ViewModel;

namespace SMART.Gui.Reports
{
    /// <summary>
    /// Interaction logic for BasicExecutionReportFlows.xaml
    /// </summary>
    public partial class BasicExecutionReportFlows
    {
        public BasicExecutionReportFlows() : this(null)
        {            
        }

        public BasicExecutionReportFlows(ReportViewModel report)
        {
            DataContext = report;
            InitializeComponent();
        }
    }
}
