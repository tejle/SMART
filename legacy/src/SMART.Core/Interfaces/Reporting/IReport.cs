using System;

namespace SMART.Core.Interfaces.Reporting
{
  public interface IReport
  {
    Guid Id { get; set; }
    string ProjectName { get; set; }
    DateTime Created { get; set; }        
    IReportScenario Scenario { get; set; }
    TimeSpan TotalElapsedTime { get; set; }
    string ResponsibleTester { get; set; }
    string TestSuiteId { get; set; }        
  }
}