namespace SMART.Core.Interfaces.Reporting
{
  public interface IReportConfigSetting
  {
    string Name { get; set; }
    object Value { get; set; }
    string FormattedValue { get; set; }
  }
}