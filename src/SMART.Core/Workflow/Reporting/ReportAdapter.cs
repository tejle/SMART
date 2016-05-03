using System.Collections.Generic;
using SMART.Core.Interfaces.Reporting;
using SMART.Core.Metadata;

namespace SMART.Core.Workflow.Reporting
{
  public class ReportAdapter : IReportAdapter
  {
    [Config]
    public string Name { get; set; }

    public IEnumerable<IReportConfigSetting> Settings { get; set; }
  }
}