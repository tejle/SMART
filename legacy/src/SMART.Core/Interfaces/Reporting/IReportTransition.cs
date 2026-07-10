using System.Collections.ObjectModel;

namespace SMART.Core.Interfaces.Reporting
{
  public interface IReportTransition : IReportElement
  {
    string Parameter { get; set; }
    ReadOnlyCollection<string> Parameters { get; }
  }
}