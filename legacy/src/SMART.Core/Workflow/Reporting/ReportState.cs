using System;
using SMART.Core.Interfaces.Reporting;
using SMART.Core.Metadata;

namespace SMART.Core.Workflow.Reporting
{
  public class ReportState : IReportState
  {
    public Guid Id { get; set; }
    [Config]
    public string Name { get; set; }
    [Config]
    public int VisitCount { get; set; }
    [Config]
    public bool IsDefect { get; set; }
  }
}