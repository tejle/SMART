using System;
using System.Linq;
using SMART.Core.Interfaces;

namespace SMART.Gui.ViewModel
{
    public class ScenarioInfoViewModel : ViewModelBase
    {
        private readonly ITestcase testcase;
        private readonly IProject project;

        public override string Icon { get { return Constants.MISSING_ICON_URL; } }
        public override Guid Id { get { return testcase.Id; } set { testcase.Id = value; } }

        public DateTime Created
        {
            get { return DateTime.Now.AddDays(-1).AddHours(-3).AddMinutes(-25); }
        }

        public DateTime Modified
        {
            get { return DateTime.Now; }
        }

        public int ModelCount
        {
            get { return testcase.Models.Count(); }
        }

        public DateTime LatestExecution
        {
            get { return (from r in project.Reports where r.Scenario.Id == this.Id orderby r.Created descending select r.Created).FirstOrDefault(); }
        }

        public bool? LatestExecutionStatus
        {
            get { return (from r in project.Reports where r.Scenario.Id == this.Id orderby r.Created descending select r.Scenario.Passed).First(); }
        }

        public int ExecutionCount
        {
            get { return (from r in project.Reports where r.Scenario.Id == this.Id select r).Count(); }
        }

        public bool NeverExecuted
        {
            get { return ExecutionCount == 0; }
        }

        public ScenarioInfoViewModel(ITestcase testcase, IProject project)
            : base(testcase.Name)
        {
            this.testcase = testcase;
            this.project = project;
        }
    }
}