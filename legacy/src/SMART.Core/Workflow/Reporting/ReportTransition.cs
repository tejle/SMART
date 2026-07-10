using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SMART.Core.Interfaces.Reporting;
using SMART.Core.Metadata;

namespace SMART.Core.Workflow.Reporting
{
  public class ReportTransition : IReportTransition
  {
    private List<string> parameters;

    public Guid Id { get; set; }
    [Config]
    public string Name { get; set; }
    [Config]
    public int VisitCount { get; set; }
    [Config]
    public bool IsDefect { get; set; }
    [Config(Default = "")]
    public string Parameter
    {
      get
      {
        return string.Join(";", parameters.ToArray());
      }
      set { parameters = new List<string>(value.Split(';')); }
    }
    public ReadOnlyCollection<string> Parameters
    {
      get
      {
        return new ReadOnlyCollection<string>(parameters);
      }
    }

    public ReportTransition()
    {
      Parameter = string.Empty;
    }
  }
}