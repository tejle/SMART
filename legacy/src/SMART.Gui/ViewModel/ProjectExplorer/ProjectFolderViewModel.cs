using SMART.Core.Events;
using SMART.Core.Interfaces;

namespace SMART.Gui.ViewModel.ProjectExplorer
{
    using System;
    using System.Collections.ObjectModel;

    using Commands;

    using Core.Interfaces.Services;

    using IOC;

    public class ProjectFolderViewModel : ViewModelBase
    {
        private readonly IProject project;

        private readonly IScenarioService testcaseService;

        private readonly IModelService modelService;


        public ObservableCollection<IFolderViewModel> FolderViewModels { get; set; }

        public string Path { get; set; }

        public RoutedActionCommand Rename
        {
            get;
            private set;
        }

        public RoutedActionCommand AddModel
        {
            get;
            private set;
        }

        public RoutedActionCommand AddTestcase
        {
            get;
            private set;
        }

        public ProjectFolderViewModel(IProject project)
                : this(project, Resolver.Resolve<IScenarioService>(), Resolver.Resolve<IModelService>())
        {
        }

        public ProjectFolderViewModel(IProject project, IScenarioService testcaseService, IModelService modelService)
                : base(project.Name)
        {
            this.project = project;
            this.testcaseService = testcaseService;
            this.modelService = modelService;


            this.project.PropertyChanged += this.project_PropertyChanged;

            this.FolderViewModels = new ObservableCollection<IFolderViewModel>();
            this.CreateCollection();
            this.CreateCommands();
        }

        void project_PropertyChanged(object sender, SmartPropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Name"))
            {
                this.Name = this.project.Name;
            }
        }

        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                if (!base.Name.Equals(value))
                    base.Name = value;
                if (!this.project.Name.Equals(value))
                    this.project.Name = value;
            }
        }

        public override Guid Id
        {
            get { return project.Id; }
            set { project.Id = value; }
        }

        private void CreateCommands()
        {
            this.Rename = new RoutedActionCommand("Rename", typeof(ProjectFolderViewModel))
                         {
                                 Description = "Rename project",
                                 Text = "Rename",
                                 OnCanExecute = o => true,
                                 OnExecute = this.RenameProject,
                                 Icon = Constants.RENAME_ICON_URL
                         };

            this.AddModel = new RoutedActionCommand("AddModel", typeof(ProjectFolderViewModel))
                           {
                                   Description = "Add a model to the project",
                                   Text = "Add model",
                                   OnCanExecute = o => true,
                                   OnExecute = this.AddModelToProject,
                                   Icon = Constants.MODEL_ADD_ICON_URL
                           };

            this.AddTestcase = new RoutedActionCommand("AddTestCase", typeof(ProjectFolderViewModel))
                              {
                                      Description = "Add a testcase to the project",
                                      Text = "Add testcase",
                                      OnCanExecute = o => true,
                                      OnExecute = this.AddNewTestcaseToProject,
                                      Icon = Constants.TESTCASE_ADD_ICON_URL
                              };

        }

        protected virtual void AddNewTestcaseToProject(object obj)
        {
            this.project.AddTestCase(this.testcaseService.CreateTestcase("new testcase"));
        }

        protected virtual void AddModelToProject(object obj)
        {
            this.project.AddModel(this.modelService.CreateModel("new model"));
        }

        protected virtual void RenameProject(object obj)
        {
            string name = "nytt namn";
            this.Name = name;
        }

        private void CreateCollection()
        {
            var g = new RootModelFolderViewModel(this.project, this.modelService);

            var t = new RootTestcaseFolderViewModel(this.project);

            var r = new RootResourceFolderViewModel();
            this.FolderViewModels.Add(g);
            this.FolderViewModels.Add(t);
            this.FolderViewModels.Add(r);
        }


        public override string Icon
        {
            get { return Constants.PROJECT_ICON_URL; }

        }


    }
}