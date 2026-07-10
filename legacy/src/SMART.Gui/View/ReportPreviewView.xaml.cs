using SMART.Core.Workflow;
using SMART.Gui.Reports;
using SMART.Gui.ViewModel;

namespace SMART.Gui.View
{
  /// <summary>
  /// Interaction logic for ReportPreviewView.xaml
  /// </summary>
  public partial class ReportPreviewView
  {
    private readonly ReportViewModel report;

    public ReportPreviewView() : this(null)
    {
    }

    public ReportPreviewView(ReportViewModel report)
    {
      this.report = report;
      InitializeComponent();

      Loaded += ReportPreviewView_Loaded;
    }

    void ReportPreviewView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      if (report != null)
      {
        //documentViewer.Document = new BasicExecutionReportGenerator().CreateDocumentAsXps(report);
        documentViewer.Document = new BasicExecutionReportGenerator().CreateDocument(report);
        documentViewer.FitToWidth();
      }
    }
  }
}
