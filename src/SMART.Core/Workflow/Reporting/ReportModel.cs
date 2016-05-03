using SMART.Core.Interfaces.Reporting;
using SMART.Core.Metadata;

namespace SMART.Core.Workflow.Reporting
{
  public class ReportModel : IReportModel
  {
    [Config]
    public string Name { get; set; }
  }
}