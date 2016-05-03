using System;
using System.Collections.Generic;
using System.Linq;
using SMART.Core.Interfaces.Reporting;
using SMART.Core.Metadata;

namespace SMART.Core.Workflow.Reporting
{
  public class ReportScenario : IReportScenario
  {
    public Guid Id { get; set; }

    [Config]
    public string Name { get; set; }

    public IEnumerable<IReportTransition> CoveredTransitions { get { return Transitions.Where(t => t.VisitCount > 0); } }

    public IEnumerable<IReportTransition> DefectTransitions { get { return Transitions.Where(t => t.IsDefect); } }

    public IEnumerable<IReportState> DefectStates { get { return States.Where(s => s.IsDefect); } }
        
    public IEnumerable<IReportState> CoveredStates { get { return States.Where(s => s.VisitCount > 0); } }
        
    public bool Passed { get { return DefectStates.Count() == 0 && DefectTransitions.Count() == 0; } }

    public IEnumerable<IReportAlgorithm> Algorithms { get; set; }

    public IEnumerable<IReportAdapter> Adapters { get; set; }

    public IEnumerable<IReportGenerationStopCriteria> GenerationStopCriterias { get; set; }

    public IEnumerable<IReportExecutionStopCriteria> ExecutionStopCriterias { get; set; }

    public IEnumerable<IReportModel> Models { get; set; }
        
    public IEnumerable<IReportState> States { get; set; }

    public IEnumerable<IReportTransition> Transitions { get; set; }

    public IEnumerable<Queue<Guid>> DefectFlows { get; set; }
  }
}