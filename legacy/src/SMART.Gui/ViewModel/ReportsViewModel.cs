using System;
using System.Collections.ObjectModel;
using System.Linq;
using SMART.Core;
using SMART.Core.Interfaces;
using SMART.Gui.Commands;

namespace SMART.Gui.ViewModel
{
    public class ReportsViewModel : ViewModelBase
    {
        private readonly ITestcase scenario;
        private readonly IProject project;
        private readonly ProjectViewModel projectViewModel;

        public override string Icon { get { return Constants.MISSING_ICON_URL; } }

        public override Guid Id { get; set; }

        public RoutedActionCommand Close { get; set; }

        private ObservableCollection<ReportViewModel> reports;
        public ObservableCollection<ReportViewModel> Reports
        {
            get
            {
                if (reports.Count == 0)
                {
                    LoadReports();
                }
                return reports;
            }            
        }

        public bool ShowScenarioColumn { get; set; }

        private void LoadReports()
        {
            if (scenario == null)
            {
                project.Reports.OrderByDescending(r => r.Created).ForEach(r => reports.Add(new ReportViewModel(r)));
                ShowScenarioColumn = true;
            }
            else
            {
                project.Reports.Where(r => r.Scenario.Id.Equals(scenario.Id)).OrderByDescending(r => r.Created).ForEach(r => reports.Add(new ReportViewModel(r)));
                ShowScenarioColumn = false;
            }
        }

        public ReportsViewModel(ITestcase scenario, IProject project, ProjectViewModel projectViewModel)
        {
            this.scenario = scenario;
            this.project = project;
            this.projectViewModel = projectViewModel;

            reports = new ObservableCollection<ReportViewModel>();

            Close = new RoutedActionCommand("Close", typeof(ReportsViewModel))
            {
                OnCanExecute = (o) => true,
                OnExecute = OnClose
            };
        }

        private void OnClose(object obj)
        {
            projectViewModel.CurrentSetting = null;
        }
    }
}
