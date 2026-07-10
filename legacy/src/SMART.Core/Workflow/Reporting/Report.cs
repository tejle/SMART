using System;
using SMART.Core.Interfaces.Reporting;
using SMART.Core.Metadata;

namespace SMART.Core.Workflow.Reporting
{
  public class Report : IReport
  {
    public Guid Id { get; set; }

    [Config]
    public string ProjectName { get; set; }

    [Config]
    public DateTime Created { get; set; }

    public IReportScenario Scenario { get; set; }
        
    [Config]
    public TimeSpan TotalElapsedTime { get; set; }
        
    [Config]
    public string ResponsibleTester { get; set; }
        
    [Config]
    public string TestSuiteId{ get; set; }

  }
}