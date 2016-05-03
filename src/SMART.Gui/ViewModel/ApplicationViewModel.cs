using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;

namespace SMART.Gui.ViewModel
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using Core.Interfaces.Services;
    using Events;
    using Commands;
    using ProjectExplorer;
    using IOC;
    using Core.Interfaces;
    using TestcaseCodeGeneration;
    using TestcaseConfiguration;
    using TestcaseExecution;

  

    public class ApplicationViewModel : ViewModelBase
    {
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public RoutedActionCommand Exit { get; set; }
        public RoutedActionCommand NewProject { get; set; }
        public RoutedActionCommand OpenProject { get; set; }
        public RoutedActionCommand SaveProject { get; set; }
        public RoutedActionCommand SaveAsProject { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global
// ReSharper restore MemberCanBePrivate.Global

        private readonly DuplicateKeyDictionary<Guid, IViewModel> popUpItems;

        public override string Icon { get { return Constants.MISSING_ICON_URL; } }
        public override Guid Id {get;set;}
        public string Status { get; set; }

        private IViewModel titleBar;
        private IViewModel menuBar;
        private IViewModel mainContent;
        private IViewModel popUpContent;
        private bool showPopUp;
        private readonly ProjectExplorerViewModel projectExplorerViewModel;
        private ViewModelFactory viewModelFactory;


        public IViewModel TitleBar
        {
            get { return titleBar; }
            set { titleBar = value; SendPropertyChanged("TitleBar"); }
        }

        public IViewModel MenuBar
        {
            get
            {
                return menuBar;
            }
            set
            {
                menuBar = value; SendPropertyChanged("MenuBar");
            }
        }

        public IViewModel MainContent
        {
            get { return mainContent; }
            private set { mainContent = value; SendPropertyChanged("MainContent"); }
        }

        public ProjectExplorerViewModel ProjectExplorerViewModel
        {
            get { return projectExplorerViewModel; }
        }


        public ApplicationViewModel(IEventService eventService)
            : base("SMART - System Verification AB")
        {
            Resolver.RegisterInstance<ApplicationViewModel>(this);
            viewModelFactory = ViewModelFactory.Create();
            Resolver.RegisterInstance<IViewModelFactory>(viewModelFactory);
            this.projectExplorerViewModel = Resolver.Resolve<ProjectExplorerViewModel>();

            popUpItems = new DuplicateKeyDictionary<Guid, IViewModel>();

            //eventService.GetEvent<OpenPopUpEvent>().Subscribe(OnOpenPopup);
            //eventService.GetEvent<ClosePopUpEvent>().Subscribe(OnClosePopUp);

            CreateCommand();
            RouteCommands();

            TitleBar = viewModelFactory.CreateTitleBar();
            MenuBar = viewModelFactory.CreateMenuBar();
            MainContent = viewModelFactory.CreateStartViewModel();
             
        }

        private void CreateCommand()
        {
            Exit = new RoutedActionCommand("Exit", typeof(ApplicationViewModel))
                       {
                           Description = "Exit SMART",
                           OnCanExecute = (o) => true,
                           OnExecute = OnExit,
                           Text = "Exit",
                           Icon = Constants.EXIT_ICON_URL
                       };
        }

        private void RouteCommands()
        {
            NewProject = projectExplorerViewModel.NewProject;
            OpenProject = projectExplorerViewModel.OpenProject;
            SaveProject = projectExplorerViewModel.SaveProject;
            SaveAsProject = projectExplorerViewModel.SaveAsProject;
        }

        private static void OnExit(object obj)
        {
            Application.Current.Shutdown();
        }

        public string VersionInfo
        {
            get
            {
                return string.Format("S.M.A.R.T v{0} © {1} System Verification AB",
                                     Assembly.GetExecutingAssembly().GetName().Version,
                                     DateTime.Now.Year
                    );
            }
        }



        public IViewModel SetActiveProject(IProject project)
        {
            viewModelFactory.ClearProjectViewModels();
            var vm = viewModelFactory.CreateProjectViewModel(project);
            MainContent = vm;
            return vm;
        }

        public IViewModel ShowHomeScreen()
        {
            var vm = viewModelFactory.CreateStartViewModel();
            MainContent = vm;
            return vm;
        }

        public void SetMainContent(IViewModel viewModel)
        {
            //if (viewModel is ProjectViewModel)
            //    ((ProjectViewModel) viewModel).SelectFirstScenario();
            MainContent = viewModel;
        }
    }
}


        //#region popup

        //private void OnClosePopUp(IViewModel viewModel)
        //{
        //    popUpContent = null;
        //    ShowPopUp = false;
        //}

        //private void OnOpenPopup(OpenPopUpEventArgs args)
        //{
        //    IViewModel viewModel = FindViewModelWithIdAndType(args);
        //    if (viewModel == null)
        //    {
        //        viewModel = CreateViewModel(args);
        //        popUpItems.Add(args.Id, viewModel);
        //    }
        //    PopUpContent = viewModel;
        //    ShowPopUp = true;
        //}

        //private IViewModel FindViewModelWithIdAndType(OpenPopUpEventArgs args)
        //{
        //    var itemsWithSameId = popUpItems.FindAll(args.Id);
        //    return itemsWithSameId.Find(i => i.GetType() == args.Type);
        //}

        //private IViewModel CreateViewModel(OpenPopUpEventArgs args)
        //{
        //    Guid id = args.Id;
        //    IViewModel viewModel = null;
        //    if (args.Type == typeof(TestcaseConfigurationCompositeViewModel))
        //    {
        //        viewModel = CreateConfigurationViewModel(id);
        //    }
        //    else if (args.Type == typeof(TestcaseCodeGenerationViewModel))
        //    {
        //        viewModel = CreateCodeGenerationViewModel(id);
        //    }
        //    else if (args.Type == typeof(TestcaseExecutionCompositeViewModel))
        //    {
        //        viewModel = CreateExecutionViewModel(id);
        //    }
        //    return viewModel;
        //}

        //private IViewModel CreateExecutionViewModel(Guid id)
        //{
        //    ITestcase testcase = GetTestcase(id);
        //    var viewModel = new TestcaseExecutionCompositeViewModel(testcase);
        //    return viewModel;
        //}

        //private IViewModel CreateCodeGenerationViewModel(Guid id)
        //{
        //    ITestcase testcase = GetTestcase(id);
        //    var viewModel = new TestcaseCodeGenerationViewModel(testcase);
        //    return viewModel;
        //}

        //private IViewModel CreateConfigurationViewModel(Guid id)
        //{
        //    ITestcase testcase = GetTestcase(id);
        //    var viewModel = new TestcaseConfigurationCompositeViewModel(testcase);
        //    return viewModel;
        //}

        //private ITestcase GetTestcase(Guid id)
        //{
        //    return projectExplorerViewModel.Project.Testcases
        //        .Where(tc => tc.Id.Equals(id)).FirstOrDefault();
        //}


        //public bool ShowPopUp
        //{
        //    get { return showPopUp; }
        //    set { showPopUp = value; SendPropertyChanged("ShowPopUp"); }
        //}

        //public IViewModel PopUpContent
        //{
        //    get { return popUpContent; }
        //    set { popUpContent = value; SendPropertyChanged("PopUpContent"); }
        //} 
        //#endregion