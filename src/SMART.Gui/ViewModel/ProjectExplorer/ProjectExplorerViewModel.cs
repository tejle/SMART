using System.Windows.Interop;

namespace SMART.Gui.ViewModel.ProjectExplorer
{
    using System.Collections.ObjectModel;
    using System.Windows;
    using System;
    using Core.Events;
    using Core.Interfaces;
    using Commands;
    using Core.Interfaces.Services;
    using Events;
    using IOC;
    using Microsoft.Win32;
    using System.IO;

    public class ProjectExplorerViewModel : ViewModelBase
    {
        ApplicationViewModel applicationViewModel;

        private readonly IProjectService projectService;
        private readonly IEventService eventService;
        private const string SmartProjectFileDefaultExt = ".smart";
        private readonly string smartProjectFileFilter = string.Format("SMART Project Files ({0})|*{0}", SmartProjectFileDefaultExt);

        public ObservableCollection<ProjectFolderViewModel> ProjectFolderViewModels { get; private set; }
        public RoutedActionCommand NewProject { get; private set; }
        public RoutedActionCommand OpenProject { get; private set; }
        public RoutedActionCommand SaveProject { get; private set; }
        public RoutedActionCommand SaveAsProject { get; private set; }

        public override string Icon { get { return Constants.MISSING_ICON_URL; } }
        public override Guid Id { get; set; }

        public IProject Project
        {
            get;
            private set;
        }

        public ProjectExplorerViewModel(ApplicationViewModel applicationViewModel, IProjectService projectService, IEventService eventService)
        {
            this.applicationViewModel = applicationViewModel;
            this.projectService = projectService;
            this.eventService = eventService;

            ProjectFolderViewModels = new ObservableCollection<ProjectFolderViewModel>();
            CreateCommands();
        }

        private void CreateCommands()
        {
            NewProject = new RoutedActionCommand("NewProject", typeof(ProjectExplorerViewModel))
                             {
                                 Description = "Creates a new project",
                                 OnCanExecute = o => true,
                                 OnExecute = OnNewProject,
                                 Text = "New project",
                                 Icon = Constants.NEW_PROJECT_ICON_URL
                             };

            OpenProject = new RoutedActionCommand("Open", typeof(ProjectExplorerViewModel))
                              {
                                  Text = "Open project",
                                  Description = "Open a project",
                                  OnCanExecute = o => true,
                                  OnExecute = OnOpenProject,
                                  Icon = Constants.OPEN32_ICON_URL
                              };

            SaveProject = new RoutedActionCommand("SaveProject", typeof(ProjectExplorerViewModel))
                              {
                                  Description = "Save the current project",
                                  OnCanExecute = OnCanSaveProject,
                                  OnExecute = OnSaveProject,
                                  Text = "Save project",
                                  Icon = Constants.SAVE32_ICON_URL
                              };

            SaveAsProject = new RoutedActionCommand("SaveAsProject", typeof(ProjectExplorerViewModel))
                                {
                                    Description = "Save the current project with a new filename",
                                    OnCanExecute = OnCanSaveProject,
                                    OnExecute = OnSaveAsProject,
                                    Text = "Save project as...",
                                    Icon = Constants.SAVEAS32_ICON_URL
                                };
        }

        private bool OnCanSaveProject(object obj)
        {
            //return Project != null;
            return ProjectFolderViewModels.Count == 1;
        }

        private void OnSaveAsProject(object obj)
        {
            var mainWindow = PresentationSource.FromVisual(Application.Current.MainWindow) as HwndSource;

            var fd = new System.Windows.Forms.SaveFileDialog
                         {
                             FileName = ProjectFolderViewModels[0].Name,
                             DefaultExt = SmartProjectFileDefaultExt,
                             Filter = smartProjectFileFilter
                         };

            if (!(fd.ShowDialog(new OldWindow(mainWindow.Handle)) == System.Windows.Forms.DialogResult.OK))
            {
                return;
            }

            SaveProjectToFile(fd.FileName);
            var projectFolderViewModel = ProjectFolderViewModels[0];
            projectFolderViewModel.Path = fd.FileName;
        }

        private void OnSaveProject(object obj)
        {
            var projectFolderViewModel = ProjectFolderViewModels[0];

            if (string.IsNullOrEmpty(projectFolderViewModel.Path))
            {
                OnSaveAsProject(obj);
            }
            else
            {
                SaveProjectToFile(projectFolderViewModel.Path);
            }
        }

        private void OnNewProject(object obj)
        {
            CreateProject();

            SetProjectOnApplication();
        }

        private void SetProjectOnApplication()
        {
            var vm = applicationViewModel.SetActiveProject(Project);
            eventService.GetEvent<MenuBarEvent>().Publish(vm);
        }

        private void OnOpenProject(object o)
        {
            string file;

            if (o != null && o is string)
            {
                file = o.ToString();
            }
            else
            {
                var mainWindow = PresentationSource.FromVisual(Application.Current.MainWindow) as HwndSource;
                var fd = new System.Windows.Forms.OpenFileDialog
                             {
                                 FileName = string.Empty,
                                 DefaultExt = SmartProjectFileDefaultExt,
                                 Filter = smartProjectFileFilter
                             };

                if (!(fd.ShowDialog(new OldWindow(mainWindow.Handle)) == System.Windows.Forms.DialogResult.OK))
                {
                    return;
                }
                file = fd.FileName;
            }


            OpenProjectFromFile(file);


        }

        private void CreateProject()
        {
            if (Project != null)
                Project.PropertyChanged -= ProjectPropertyChanged;

            Project = projectService.CreateProject(true);
            Project.PropertyChanged += ProjectPropertyChanged;

            var projectFolderViewModel = new ProjectFolderViewModel(Project);

            ProjectFolderViewModels.Clear();
            ProjectFolderViewModels.Add(projectFolderViewModel);
        }

        public bool OpenProjectFromFile(string file)
        {
            if (!File.Exists(file)) return false;

            if (Project != null)
                Project.PropertyChanged -= ProjectPropertyChanged;
            Project = null;
            ProjectFolderViewModels.Clear();


            Project = projectService.OpenProjectFromFile(file);
            Project.PropertyChanged += ProjectPropertyChanged;

            var projectFolderViewModel = new ProjectFolderViewModel(Project)
                                             {
                                                 Path = file
                                             };

            ProjectFolderViewModels.Add(projectFolderViewModel);

            eventService.GetEvent<OpenProjectEvent>().Publish(file);

            SetProjectOnApplication();
            return true;
        }

        private static void ProjectPropertyChanged(object sender, SmartPropertyChangedEventArgs e)
        {

        }

        private void SaveProjectToFile(string filename)
        {
            projectService.SaveProjectToFile(Project, filename);
            eventService.GetEvent<SaveProjectEvent>().Publish(filename);
        }

        private class OldWindow : System.Windows.Forms.IWin32Window
        {
            IntPtr _handle;
            public OldWindow(IntPtr handle)
            {
                _handle = handle;
            }
            #region IWin32Window Members
            IntPtr System.Windows.Forms.IWin32Window.Handle
            { get { return _handle; } }
            #endregion
        }
    }
}