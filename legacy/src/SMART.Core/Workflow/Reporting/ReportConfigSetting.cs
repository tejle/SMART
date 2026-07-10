using SMART.Core.DataLayer.Interfaces;
using SMART.Core.Interfaces.Reporting;
using SMART.Core.Metadata;

namespace SMART.Core.Workflow.Reporting
{
  public class ReportConfigSetting : IReportConfigSetting
  {
    [Config]
    public string Name
    {
      get;
      set;
    }

    [Config]
    public object Value
    {
      get;
      set;
    }

    [Config]
    public string FormattedValue
    {
      get;
      set;
    }

    public static string FormatSetting(IConfigSetting setting)
    {
      if (setting.Value == null)
        return string.Empty;

      switch (setting.Config.Editor)
      {
        case ConfigEditor.Text:
          return setting.Value.ToString();
        case ConfigEditor.Percent:
          return ((double)setting.Value).ToString("P0");
        case ConfigEditor.Assembly:
          return StringHelper.ShortenPathname(setting.Value.ToString(), 50);
      }
      return setting.Value.ToString();
    }
  }
}