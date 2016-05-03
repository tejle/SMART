using System;
using System.Linq;
using SMART.Core.Interfaces;

namespace SMART.Gui.ViewModel
{
    public class ProjectInfoViewModel : ViewModelBase
    {
        private readonly IProject project;

        public override string Icon { get { return Constants.MISSING_ICON_URL; } }
        public override Guid Id { get { return project.Id; } set { project.Id = value; } }

        public DateTime Created
        {
            get { return DateTime.Now.AddDays(-1).AddHours(-3); }
        }

        public DateTime Modified
        {
            get { return DateTime.Now; }
        }

        public int ScenarioCount
        {
            get { return project.Testcases.Count(); }
        }

        public int ModelCount
        {
            get { return project.Models.Count(); }
        }

        public ProjectInfoViewModel(IProject project) : base(project.Name)
        {
            this.project = project;
        }
    }
}