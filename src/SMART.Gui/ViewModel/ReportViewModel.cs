using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Microsoft.Practices.Unity.Utility;
using SMART.Core;
using SMART.Core.Interfaces.Reporting;
using SMART.Gui.Commands;
using SMART.Gui.View;

namespace SMART.Gui.ViewModel
{
    public class ReportViewModel : ViewModelBase
    {
        private readonly IReport report;

        public RoutedActionCommand ShowReport { get; set; }

        public override string Icon { get { return Constants.MISSING_ICON_URL; } }

        public override Guid Id
        {
            get { return report.Id; }            
            set { report.Id = value; }   
        }

        public bool Passed { get { return report.Scenario.Passed; } }

        public string Created { get { return string.Format("{0} {1}", report.Created.ToShortDateString(), report.Created.ToShortTimeString()); } }

        public string ProjectName { get { return report.ProjectName; } }

        public string TotalElapsedTime
        {
            get
            {
                return string.Format("{0}h {1}min {2}sec", 
                                     report.TotalElapsedTime.Hours, report.TotalElapsedTime.Minutes, report.TotalElapsedTime.Seconds);
            }
        }

        public string ResponsibleTester { get { return report.ResponsibleTester; } }

        public string ScenarioName { get { return report.Scenario.Name; } }

        public IEnumerable<IReportModel> Models { get { return report.Scenario.Models; } }
        
        public IEnumerable<IReportAlgorithm> Algorithms { get { return report.Scenario.Algorithms; } }
        
        public IEnumerable<IReportAdapter> Adapters { get { return report.Scenario.Adapters; } }
        
        public IEnumerable<IReportGenerationStopCriteria> GenerationStopCriterias { get { return report.Scenario.GenerationStopCriterias; } }
        
        public IEnumerable<IReportExecutionStopCriteria> ExecutionStopCriterias { get { return report.Scenario.ExecutionStopCriterias; } }

        public IEnumerable<IReportState> States { get { return report.Scenario.States; } }
        
        public IEnumerable<IReportTransition> Transitions { get { return report.Scenario.Transitions; } }

        public int StateCount { get { return report.Scenario.States.Count(); } }

        public int CoveredStates { get { return report.Scenario.CoveredStates.Count(); } }
        
        public double CoveredStatesPercent
        {
            get { return (double)report.Scenario.CoveredStates.Count() / report.Scenario.States.Count(); }
        }

        public int DefectStates { get { return report.Scenario.DefectStates.Count(); } }

        public int TransitionCount { get { return report.Scenario.Transitions.Count(); } }

        public int CoveredTransitions { get { return report.Scenario.CoveredTransitions.Count(); } }
        
        public double CoveredTransitionsPercent
        {
            get { return (double)report.Scenario.CoveredTransitions.Count() / report.Scenario.Transitions.Count(); }
        }

        public int DefectTransitions { get { return report.Scenario.DefectTransitions.Count(); } }

        public ObservableCollection<Pair<string, double>> StateCoverage
        {
            get
            {
                return new ObservableCollection<Pair<string, double>>(new List<Pair<string, double>>
                           {
                               new Pair<string, double>("Covered", CoveredStatesPercent),
                               new Pair<string, double>("Not covered", 1 - CoveredStatesPercent)
                           });
            }
        }

        public ObservableCollection<Pair<string,double>> TransitionCoverage
        {
            get
            {
                return new ObservableCollection<Pair<string,double>>(new List<Pair<string, double>>
                           {
                               new Pair<string, double>("Covered", CoveredTransitionsPercent),
                               new Pair<string, double>("Not covered", 1 - CoveredTransitionsPercent)
                           });
            }
        }

        public IEnumerable<List<string>> DefectFlows
        {
            get
            {
                var result = new List<List<string>>();
                
                var flows = from f in report.Scenario.DefectFlows select new List<Guid>(f);                               

                var elements = 
                    (from e in report.Scenario.States.Cast<IReportElement>().Union(report.Scenario.Transitions.Cast<IReportElement>()) select e).ToList();
                
                flows.ForEach(flow =>
                                  {
                                      var list = flow.Map(g => elements.Find(e => e.Id == g))
                                         .Select((e, i) =>
                                                   {
                                                     if (e is IReportTransition)
                                                     {
                                                       return e.Name + " " + (e as IReportTransition).Parameter;
                                                     }
                                                     return e.Name;
                                                   }).ToList();
                                      //var joinedQueue = (from e in elements
                                      //                   join f in flow on e.Id equals f 
                                      //                   orderby flow.IndexOf(f)
                                      //                   select  e.Name).ToList();

                                      result.Add(list); 
                                  });                

                return result;
            }
        }

        public ReportViewModel(IReport report)
        {
            this.report = report;

            ShowReport = new RoutedActionCommand("ShowReport", typeof(ReportViewModel))
                           {
                             Text = "Show report", 
                             OnCanExecute = o => true, 
                             OnExecute = OnShowReport
                           };
        }

      private static void OnShowReport(object obj)
      {
        var r = obj as ReportViewModel;
        if (r != null)
        {
          var preview = new ReportPreviewView(r)
                            {
                                Owner = Application.Current.MainWindow
                            };
            preview.Show();
        }
      }
    }

    public static class EnumerableExtensions
    {
        public static IEnumerable<R> Map<T, R>(this IEnumerable<T> seq, Func<T, R> f)
        {
            foreach (var t in seq)
                yield return f(t);
        }
    }
}