using System;
using System.Collections.Generic;

namespace SMART.Core.Interfaces.Reporting
{
  public interface IReportScenario
  {
    string Name { get; set; }
    Guid Id { get; set; }

    IEnumerable<IReportTransition> CoveredTransitions { get; }
    IEnumerable<IReportTransition> DefectTransitions { get; }

    IEnumerable<IReportState> CoveredStates { get; }
    IEnumerable<IReportState> DefectStates { get; }

    bool Passed { get; }
    IEnumerable<IReportAlgorithm> Algorithms { get; set; }
    IEnumerable<IReportAdapter> Adapters { get; set; }
    IEnumerable<IReportGenerationStopCriteria> GenerationStopCriterias { get; set; }
    IEnumerable<IReportExecutionStopCriteria> ExecutionStopCriterias { get; set; }
    IEnumerable<IReportModel> Models { get; set; }
    IEnumerable<IReportState> States { get; set; }
    IEnumerable<IReportTransition> Transitions { get; set; }

    IEnumerable<Queue<Guid>> DefectFlows { get; set; }
  }
}