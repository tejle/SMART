using System;

namespace SMART.Core.Interfaces.Reporting
{
  public interface IReportElement
  {
    Guid Id { get; set; }
    string Name { get; set; }
    int VisitCount { get; set; }
    bool IsDefect { get; set; }
  }
}