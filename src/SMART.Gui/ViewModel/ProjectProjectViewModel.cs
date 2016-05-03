using System;
using SMART.Core.Interfaces;
using SMART.Gui.Commands;

namespace SMART.Gui.ViewModel
{
    public class ProjectProjectViewModel : ViewModelBase, IEditableViewModel
    {
        private readonly IProject project;
        private bool isEditMode;

        public RoutedActionCommand Rename { get; private set; }

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public bool IsEditMode
        {
            get { return isEditMode; }
            set { isEditMode = value; SendPropertyChanged("IsEditMode"); }
        }

        public bool StartInEditMode
        {
            get; set;
        }

        public override Guid Id
        {
            get { return project.Id; }
            set { project.Id = value; }
        }

        public override string Name
        {
            get
            {
                return project.Name;
            }
            set
            {
                if (base.Name.Equals(value)) return;

                base.Name = value;
                if (!project.Name.Equals(value))
                    project.Name = value;
                SendPropertyChanged("Name");
            }
        }

        public ProjectProjectViewModel(IProject project)
        {
            this.project = project;

            this.Rename = new RoutedActionCommand("Rename", typeof(ProjectScenarioViewModel))
                              {
                                  Description = "Rename scenario",
                                  OnCanExecute = (o) => true,
                                  OnExecute = this.OnRename,
                                  Text = "Rename",
                                  Icon = Constants.RENAME_ICON_URL
                              };
        }

        private void OnRename(object obj)
        {
            IsEditMode = true;
        }
    }
}