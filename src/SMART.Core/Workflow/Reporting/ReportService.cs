using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces.Reporting;
using SMART.Core.Metadata;

namespace SMART.Core.Workflow.Reporting
{
  public class ReportService
  {
    public IReport GenerateReport(UnitOfWork work)
    {            
      var user = WindowsIdentity.GetCurrent();
      var userName = user == null ? "Could not retrieve user" : user.Name;
      var report = new Report
                     {
                       Id = Guid.NewGuid(),
                       Created = DateTime.Now,
                       ProjectName = work.Project.Name,
                       ResponsibleTester = userName,
                       TestSuiteId = work.Testcase.Id.ToString()
                     };
                                     
      report.Scenario = new ReportScenario()
                          {
                            Name = work.Testcase.Name,
                            Id = work.Testcase.Id,
                            Transitions = from t in work.Model.Transitions
                                                          
                                          select new ReportTransition()
                                                   {
                                                     Id = t.Id,
                                                     IsDefect = t.IsDefect,
                                                     Name = t.Label,
                                                     VisitCount = t.VisitCount,
                                                     Parameter = t.Parameter
                                                   } as IReportTransition
                            ,
                            States = from s in work.Model.States.Except(new State[] { work.Model.StartState, work.Model.StopState })
                                                     
                                     select new ReportState()
                                              {
                                                Id = s.Id,
                                                IsDefect = s.IsDefect, 
                                                Name=s.Label, 
                                                VisitCount = s.VisitCount
                                              } as IReportState


                          };

      report.Scenario.DefectFlows = new List<Queue<Guid>>(from f in work.DefectFlows
                                                          let flow = from e in f select e.ModelElement.Id
                                                          select new Queue<Guid>(flow));

      report.Scenario.Algorithms =from a in work.Testcase.Algorithms
                                  select
                                    new ReportAlgorithm()
                                      {
                                        Name =
                                          a.MetadataView
                                          <AlgorithmAttribute>().Name,
                                        Settings = from s in a.GetConfig().Values
                                                   select new ReportConfigSetting()
                                                            {
                                                              Name = s.Name, 
                                                              Value=s.Value, 
                                                              FormattedValue = ReportConfigSetting.FormatSetting(s)
                                                            } as IReportConfigSetting
                                      } as IReportAlgorithm;

      report.Scenario.GenerationStopCriterias = from g in work.Testcase.GenerationStopCriterias
                                                select
                                                  new ReportGenerationStopCriteria()
                                                    {
                                                      Name =
                                                        g.MetadataView
                                                        <GenerationStopCriteriaAttribute>().Name,
                                                      Settings = from s in g.GetConfig().Values
                                                                 select new ReportConfigSetting()
                                                                          {
                                                                            Name = s.Name,
                                                                            Value = s.Value,
                                                                            FormattedValue = ReportConfigSetting.FormatSetting(s)
                                                                          } as IReportConfigSetting
                                                    } as IReportGenerationStopCriteria;

      report.Scenario.ExecutionStopCriterias = from e in work.Testcase.ExecutionStopCriteriasas
                                               select
                                                 new ReportExecutionStopCriteria()
                                                   {
                                                     Name =
                                                       e.MetadataView
                                                       <ExecutionStopCriteriaAttribute>().Name,
                                                     Settings = from s in e.GetConfig().Values
                                                                select new ReportConfigSetting()
                                                                         {
                                                                           Name = s.Name,
                                                                           Value = s.Value,
                                                                           FormattedValue = ReportConfigSetting.FormatSetting(s)
                                                                         } as IReportConfigSetting
                                                   } as IReportExecutionStopCriteria;

      report.Scenario.Adapters = from a in work.Testcase.Adapters
                                 select
                                   new ReportAdapter()
                                     {
                                       Name =
                                         a.MetadataView
                                         <AdapterAttribute>().Name,
                                       Settings = from s in a.GetConfig().Values
                                                  select new ReportConfigSetting()
                                                           {
                                                             Name = s.Name,
                                                             Value = s.Value,
                                                             FormattedValue = ReportConfigSetting.FormatSetting(s)
                                                           } as IReportConfigSetting
                                     } as IReportAdapter;

      report.Scenario.Models = from m in work.Testcase.Models
                               select new ReportModel()
                                        {
                                          Name = m.Name
                                        } as IReportModel;

      return report;
    }

    public void SaveReport(IReport report)
    {

    }

    public IReport LoadReport(Guid id)
    {
      return new Report();
    }
  }
}