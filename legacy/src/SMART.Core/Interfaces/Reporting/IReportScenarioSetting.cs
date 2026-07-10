using System.Collections.Generic;

namespace SMART.Core.Interfaces.Reporting
{
  public interface IReportScenarioSetting
  {
    string Name { get; set; }

    IEnumerable<IReportConfigSetting> Settings { get; set; }
  }
}