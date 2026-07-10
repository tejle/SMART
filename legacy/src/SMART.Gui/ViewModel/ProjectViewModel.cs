using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using SMART.Core;
using SMART.Core.DomainModel.Validation;
using SMART.Core.Interfaces;
using SMART.Gui.Commands;
using SMART.Gui.ViewModel.TestcaseExecution;
using SMART.IOC;
using SMART.Core.Interfaces.Services;
using SMART.Core.Events;

namespace SMART.Gui.ViewModel
{
    public class TaskStatus
    {
        public bool IsOk { get; set; }
        public string Msg { get; set; }
    }

    public class ProjectViewModel : ViewModelBase, IEditableViewModel
    {
        private bool isEditMode;
        private readonly IEventService eventService;
        private readonly IViewModelFactory viewModelFactory;
        private readonly ApplicationViewModel applicationViewModel;

        private readonly IProject project;
        private readonly IModelService modelService;
        private readonly IScenarioService testcaseService;

        private ObservableCollection<ProjectProjectViewModel> projects;
        private ObservableCollection<ProjectScenarioViewModel> scenarios;
        private ProjectProjectViewModel currentProject;
        private ProjectScenarioViewModel currentScenario;
        private ProjectModelViewModel currentModel;

        public TaskStatus AlgorithmStatus { get; set; }
        public TaskStatus AdapterStatus { get; set; }
        public TaskStatus GenerationStopCriteriaStatus { get; set; }
        public TaskStatus ExecutionStopCriteriaStatus { get; set; }
        public TaskStatus ExecutionStatus { get; set; }

        public RoutedActionCommand OpenModelCommand { get; set; }
        public RoutedActionCommand NewModelCommand { get; set; }
        public RoutedActionCommand DeleteModelCommand { get; set; }
        public RoutedActionCommand AddModelCommand { get; set; }
        public RoutedActionCommand RenameModelCommand { get; set; }

        public RoutedActionCommand NewScenarioCommand { get; set; }
        public RoutedActionCommand DeleteScenarioCommand { get; set; }
        public RoutedActionCommand RenameScenarioCommand { get; set; }

        public RoutedActionCommand SettingsCommand { get; set; }
        public RoutedActionCommand AlgorithmSettings { get; set; }
        public RoutedActionCommand GenerationStopCriteriaSettings { get; set; }
        public RoutedActionCommand AdapterSettings { get; set; }
        public RoutedActionCommand ExecutionStopCriteriaSettings { get; set; }

        public RoutedActionCommand CodeCommand { get; set; }
        public RoutedActionCommand GenerateCommand { get; set; }
        public RoutedActionCommand ExecuteCommand { get; set; }
        public RoutedActionCommand ReportsCommand { get; set; }

        public RoutedActionCommand ShowProjectModels { get; set; }
        public RoutedActionCommand RenameProject { get; set; }

        public bool IsEditMode
        {
            get { return isEditMode; }
            set { isEditMode = value; SendPropertyChanged("IsEditMode"); }
        }

        public bool StartInEditMode
        {
            get;
            set;
        }

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
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
                return base.Name;
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

        private IViewModel currentSetting;
        public IViewModel CurrentSetting
        {
            get { return currentSetting; }
            set { currentSetting = value; SendPropertyChanged("CurrentSetting"); }
        }

        public ObservableCollection<ProjectProjectViewModel> Projects
        {
            get
            {
                if (projects == null)
                {
                    projects = new ObservableCollection<ProjectProjectViewModel>();
                    projects.Add(new ProjectProjectViewModel(this.project));
                    currentProject = projects[0];
                }
                return projects;
            }
            set { projects = value; }
        }

        public ObservableCollection<ProjectScenarioViewModel> Scenarios
        {
            get
            {
                if (scenarios == null)
                {
                    scenarios = new ObservableCollection<ProjectScenarioViewModel>();
                    project.Testcases.ToList().ForEach(tc => scenarios.Add(new ProjectScenarioViewModel(project, tc)));
                }
                return scenarios;
            }
            set { scenarios = value; }
        }

        public ProjectProjectViewModel CurrentProject
        {
            get
            {
                return currentProject;
            }
            set
            {
                //if (value == null) return;
                currentProject = value;
                SendPropertyChanged("CurrentProject");
                currentScenario = null;
                SendPropertyChanged("CurrentScenario");
                currentSetting = null;
                SendPropertyChanged("CurrentSetting");
                if (currentProject != null)
                {
                    Models = projectModels;
                    currentModel = null;
                    SendPropertyChanged("CurrentModel");

                    InfoContent = new ProjectInfoViewModel(project);
                }
                
                eventService.GetEvent<MenuBarEvent>().Publish(this);
            }
        }

        public ProjectScenarioViewModel CurrentScenario
        {
            get
            {
                return currentScenario;
            }
            set
            {
                //if (value == null) return;
                currentScenario = value;
                currentProject = null;
                currentSetting = null;
                SendPropertyChanged("CurrentScenario");
                SendPropertyChanged("CurrentProject");
                SendPropertyChanged("CurrentSetting");
                if (currentScenario != null)
                {
                    models = currentScenario.Models;
                    SendPropertyChanged("Models");
                    currentModel = null;
                    SendPropertyChanged("CurrentModel");

                    InfoContent = new ScenarioInfoViewModel(currentScenario.Scenario, project);
                }
                
                eventService.GetEvent<MenuBarEvent>().Publish(currentScenario);
            }
        }

        public ProjectModelViewModel CurrentModel
        {
            get { return currentModel; }
            set
            {
                currentModel = value;

                if (currentModel != null)
                {
                    InfoContent = new ModelInfoViewModel(currentModel.Model);
                }
                SendPropertyChanged("CurrentModel");
            }
        }

        private ObservableCollection<ProjectModelViewModel> projectModels;

        private ObservableCollection<ProjectModelViewModel> models;

        public ObservableCollection<ProjectModelViewModel> Models
        {
            get { return models; }
            set { models = value; SendPropertyChanged("Models"); }
        }

        private IViewModel infoContent;
        public IViewModel InfoContent
        {
            get { return infoContent; }
            set { infoContent = value; SendPropertyChanged("InfoContent"); }
        }

        public ProjectViewModel(IProject project)
            : this(project,
                Resolver.Resolve<ApplicationViewModel>(),
                Resolver.Resolve<IModelService>(),
                Resolver.Resolve<IScenarioService>(),
                Resolver.Resolve<IEventService>(),
                Resolver.Resolve<IViewModelFactory>()
                )
        {
        }

        public ProjectViewModel(IProject project, ApplicationViewModel applicationViewModel, IModelService modelService, IScenarioService testcaseService, IEventService eventService, IViewModelFactory modelFactory)
            : base(project.Name)
        {
            this.applicationViewModel = applicationViewModel;
            this.project = project;
            this.modelService = modelService;
            this.testcaseService = testcaseService;
            this.eventService = eventService;
            viewModelFactory = modelFactory;

            project.PropertyChanged += ProjectPropertyChanged;
            project.CollectionChanged += project_CollectionChanged;

            CreateCommands();
            CreateCollection();

            AlgorithmStatus = new TaskStatus();
            AdapterStatus = new TaskStatus();
            GenerationStopCriteriaStatus = new TaskStatus();
            ExecutionStopCriteriaStatus = new TaskStatus();
            ExecutionStatus = new TaskStatus();

            CurrentProject = Projects[0];
            //SelectFirstScenario();
        }

        private void CreateCollection()
        {
            var list = new List<ProjectModelViewModel>();
            foreach (var model in project.Models)
            {
                var vm = new ProjectModelViewModel(model, project, null);
                list.Add(vm);
            }
            projectModels = new ObservableCollection<ProjectModelViewModel>(list);
        }

        void project_CollectionChanged(object sender, SmartNotifyCollectionChangedEventArgs e)
        {
            if (e.CollectionName.Equals("Testcases"))
            {
                if (e.Action == SmartNotifyCollectionChangedAction.Add)
                {
                    ScenarioAdded(e.NewItems);

                }
                else if (e.Action == SmartNotifyCollectionChangedAction.Remove)
                {
                    ScenarioRemoved(e.OldItems);
                }
            }
            else if (e.CollectionName.Equals("Models"))
            {

                if (e.Action == SmartNotifyCollectionChangedAction.Add)
                {
                    ModelAdded((IEnumerable<IModel>)e.NewItems);

                }
                else if (e.Action == SmartNotifyCollectionChangedAction.Remove)
                {
                    ModelRemoved(e.OldItems);

                }
            }
        }

        private void ModelRemoved(IList items)
        {
            foreach (IModel model in items)
            {
                ProjectModelViewModel viewModel = FindViewModelForModel(model);

                projectModels.Remove(viewModel);
                viewModel.Dispose();
            }
            SendPropertyChanged("Models");

        }

        private void ModelAdded(IEnumerable<IModel> items)
        {
            foreach (var model in items)
            {
                //viewModelFactory.CreateProjectModelViewModel()
                if (model == items.Last())
                {
                    projectModels.Add(new ProjectModelViewModel(model, project, null) { StartInEditMode = true });
                }
                else
                {
                    projectModels.Add(new ProjectModelViewModel(model, project, null) { StartInEditMode = false });
                }
            }

            if (CurrentScenario == null)
                CurrentModel = projectModels.Last();
            else
                CurrentModel = CurrentScenario.Models.Last();

            SendPropertyChanged("Models");

        }

        private void ScenarioRemoved(IList items)
        {
            foreach (ITestcase scenario in items)
            {
                ProjectScenarioViewModel viewModel = FindViewModelForScenario(scenario);
                Scenarios.Remove(viewModel);
            }
        }

        private void ScenarioAdded(IList items)
        {
            foreach (ITestcase scenario in items)
            {
                scenarios.Add(new ProjectScenarioViewModel(project, scenario) { StartInEditMode = true });
            }
            CurrentScenario = scenarios.Last();
        }

        public void SelectFirstScenario()
        {
            if (CurrentScenario == null)
                CurrentScenario = Scenarios.FirstOrDefault();
        }

        void ProjectPropertyChanged(object sender, SmartPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
            {
                Name = project.Name;
            }
            else
            {
                throw new ArgumentException("What property???", e.PropertyName);
            }
        }

        private ProjectModelViewModel FindViewModelForModel(IModel model)
        {
            var list = from m in projectModels where m.Id == model.Id select m;
            if (list.Count() == 0) throw new ArgumentException("model not found among viewmodels, internal error");
            if (list.Count() > 1)
                throw new ArgumentException("more than one model with the same id found in project");

            return list.First();
        }
        private ProjectScenarioViewModel FindViewModelForScenario(ITestcase scenario)
        {
            var list = from t in Scenarios where t.Id == scenario.Id select t;
            if (list.Count() == 0) throw new ArgumentException("testcase not found among viewmodels, internal error");
            if (list.Count() > 1)
                throw new ArgumentException("more than one testcase with the same id found in project");

            return list.First();

        }

        private void OnRenameScenario(object obj)
        {
            var scenario = obj as ProjectScenarioViewModel;
            if (scenario != null)
            {
                scenario.Name = string.Format("{0}-{1}", scenario.Name, DateTime.Now.Second);
            }
        }

        private void OnDeleteScenario(object obj)
        {
            var scenarioViewModel = obj as ProjectScenarioViewModel;
            if (scenarioViewModel != null)
            {
                project.RemoveTestCase(scenarioViewModel.Scenario);
                Scenarios.Remove(scenarioViewModel);
                if (scenarioViewModel == currentScenario)
                    currentScenario = Scenarios.FirstOrDefault();
            }
        }

        private void OnNewScenario(object obj)
        {
            var tc = testcaseService.CreateTestcase("new scenario");
            project.AddTestCase(tc);

        }

        private void OnRenameModel(object obj)
        {
            var model = obj as ProjectModelViewModel;
            if (model != null)
            {
                model.Name = string.Format("{0}-{1}", model.Name, DateTime.Now.Second);
            }
        }

        private void OnNewModel(object obj)
        {
            var model = modelService.CreateModel("new model");
            if (CurrentScenario == null)
            {
                project.AddModel(model);
            }
            else
            {
                project.AddModel(model, currentScenario.Scenario);

            }
        }

        private void OnAddModel(object obj)
        {
        }

        private void OnDeleteModel(object obj)
        {
            var model = obj as ProjectModelViewModel;
            if (model != null)
            {
                Models.Remove(model);
                CurrentScenario.Scenario.Remove(model.Model);

                if (CurrentModel == model)
                {
                    CurrentModel = Models.FirstOrDefault();
                }
                model.Dispose();
            }

        }

        private void OnOpenModel(object o)
        {
            IViewModel vm;

            if (currentScenario == null)
            {
                vm = viewModelFactory.CreateModelDesigner(project.Models.ToList(), CurrentModel.Model);
            }
            else
            {
                vm = viewModelFactory.CreateModelDesigner(currentScenario.Scenario.Models.ToList(), CurrentModel.Model);
            }

            applicationViewModel.SetMainContent(vm);
        }

        private void OnSettings(object obj)
        {
            var scenario = obj as ProjectScenarioViewModel;
            if (scenario != null)
            {

                var vm = viewModelFactory.CreateTestcaseConfiguration(scenario.Scenario);
                applicationViewModel.SetMainContent(vm);
            }
        }

        private void OnAlgorithmSettins(object obj)
        {
            var scenario = obj as ProjectScenarioViewModel;
            if (scenario != null)
            {
                var vm = new TestcaseConfiguration.AlgorithmCollectionViewModel(scenario.Scenario, this);
                CurrentSetting = vm;
                //applicationViewModel.SetMainContent(vm);
            }

        }

        private void OnGenerationStopCriteriaSettings(object obj)
        {
            var scenario = obj as ProjectScenarioViewModel;
            if (scenario != null)
            {
                var vm = new TestcaseConfiguration.GenerationStopCriteriaCollectionViewModel(scenario.Scenario, this);
                //applicationViewModel.SetMainContent(vm);
                CurrentSetting = vm;
            }

        }

        private void OnAdapterSettings(object obj)
        {
            var scenario = obj as ProjectScenarioViewModel;
            if (scenario != null)
            {
                var vm = new TestcaseConfiguration.AdapterCollectionViewModel(scenario.Scenario, this);
                //applicationViewModel.SetMainContent(vm);
                CurrentSetting = vm;
            }
        }

        private void OnExecutionStopCriteriaSettings(object obj)
        {
            var scenario = obj as ProjectScenarioViewModel;
            if (scenario != null)
            {
                var vm = new TestcaseConfiguration.ExecutionStopCriteriaCollectionViewModel(scenario.Scenario, this);
                CurrentSetting = vm;
                //applicationViewModel.SetMainContent(vm);
            }
        }

        private void OnCode(object obj)
        {
            var scenario = obj as ProjectScenarioViewModel;
            if (scenario != null)
            {
                var vm = viewModelFactory.CreateCodeGeneration(project, scenario.Scenario);
                applicationViewModel.SetMainContent(vm);

            }
        }

        private void OnGenerateAndExecute(object obj)
        {
            var scenario = obj as ProjectScenarioViewModel;
            if (scenario != null)
            {
                Debug.Assert(scenario == CurrentScenario);

                var vm = viewModelFactory.CreateGenerateAndExecute(project, scenario.Scenario) as TestcaseExecutionCompositeViewModel;
                vm.Reset();
                applicationViewModel.SetMainContent(vm);

            }
        }

        private void OnShowProjectModels(object obj)
        {
            currentScenario = null;
            SendPropertyChanged("Models");
            SendPropertyChanged("CurrentScenario");
            eventService.GetEvent<MenuBarEvent>().Publish(this);
        }

        private void OnProjectRename(object obj)
        {
            IsEditMode = true;
        }

        private void CreateCommands()
        {
            SmartCommands.DeleteModel.InputGestures.Add(new KeyGesture(Key.Delete));
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(SmartCommands.DeleteModel, OnDelete));

            OpenModelCommand = new RoutedActionCommand("OpenModelCommand", typeof(ProjectViewModel))
                                   {
                                       Description = "Open model",
                                       Text = "Open",
                                       OnCanExecute = o => true,
                                       OnExecute = OnOpenModel,
                                       Icon = Constants.MISSING_ICON_URL
                                   };
            NewModelCommand = new RoutedActionCommand("NewModelCommand", typeof(ProjectViewModel))
                                  {
                                      Description = "New model",
                                      Text = "New model",
                                      OnCanExecute = e => true,
                                      OnExecute = OnNewModel,
                                      Icon = Constants.MISSING_ICON_URL
                                  };
            DeleteModelCommand = new RoutedActionCommand("DeleteModelCommand", typeof(ProjectViewModel))
                                     {
                                         Description = "Delete Model",
                                         Text = "Delete Model",
                                         OnCanExecute = e => true,
                                         OnExecute = OnDeleteModel,
                                         Icon = Constants.MISSING_ICON_URL
                                     };

            AddModelCommand = new RoutedActionCommand("AddModelCommand", typeof(ProjectViewModel))
                                  {
                                      Description = "Add model",
                                      Text = "Add model",
                                      OnCanExecute = e => true,
                                      OnExecute = OnAddModel,
                                      Icon = Constants.MISSING_ICON_URL
                                  };
            RenameModelCommand = new RoutedActionCommand("RenameModelCommand", typeof(ProjectViewModel))
                                     {
                                         Description = "Rename model",
                                         Text = "Rename model",
                                         OnCanExecute = e => true,
                                         OnExecute = OnRenameModel,
                                         Icon = Constants.MISSING_ICON_URL
                                     };


            NewScenarioCommand = new RoutedActionCommand("NewScenarioCommand", typeof(ProjectViewModel))
                                     {
                                         Description = "New scenario",
                                         Text = "New scenario",
                                         OnCanExecute = e => true,
                                         OnExecute = OnNewScenario,
                                         Icon = Constants.TESTCASE_ADD_ICON_URL
                                     };
            DeleteScenarioCommand = new RoutedActionCommand("DeleteScenarioCommand", typeof(ProjectViewModel))
                                        {
                                            Description = "Delete scenario",
                                            Text = "Delete scenario",
                                            OnCanExecute = e => true,
                                            OnExecute = OnDeleteScenario,
                                            Icon = Constants.MISSING_ICON_URL
                                        };

            RenameScenarioCommand = new RoutedActionCommand("RenameScenarioCommand", typeof(ProjectViewModel))
                                        {
                                            Description = "Rename scenario",
                                            Text = "Rename scenario",
                                            OnCanExecute = e => true,
                                            OnExecute = OnRenameScenario,
                                            Icon = Constants.MISSING_ICON_URL
                                        };

            SettingsCommand = new RoutedActionCommand("SettingsCommand", typeof(ProjectViewModel))
                    {
                        Description = "Settings for testcase",
                        Text = "Settings",
                        OnCanExecute = e => false,
                        OnExecute = OnSettings,
                        Icon = Constants.MISSING_ICON_URL
                    };

            AlgorithmSettings = new RoutedActionCommand("AlgorithmSettings", typeof(ProjectViewModel))
                                    {
                                        Description = "AlgorithmSettings",
                                        Text = "Algorithm",
                                        OnCanExecute = OnCanExecuteAlgorithm,
                                        OnExecute = OnAlgorithmSettins,
                                        Icon = Constants.MISSING_ICON_URL
                                    };
            GenerationStopCriteriaSettings = new RoutedActionCommand("GenerationStopCriteriaSettings", typeof(ProjectViewModel))
                                                 {
                                                     Description = "GenerationStopCriteriaSettings",
                                                     Text = "GenerationStopCriteria",
                                                     OnCanExecute = OnCanExecuteGenerationStopCriteria,
                                                     OnExecute = OnGenerationStopCriteriaSettings,
                                                     Icon = Constants.MISSING_ICON_URL
                                                 };
            AdapterSettings = new RoutedActionCommand("AdapterSettings", typeof(ProjectViewModel))
                                  {
                                      Description = "AdapterSettings",
                                      Text = "Adapter",
                                      OnCanExecute = OnCanExecuteAdapter,
                                      OnExecute = OnAdapterSettings,
                                      Icon = Constants.MISSING_ICON_URL
                                  };

            ExecutionStopCriteriaSettings = new RoutedActionCommand("ExecutionStopCriteriaSettings", typeof(ProjectViewModel))
                                                {
                                                    Description = "ExecutionStopCriteriaSettings",
                                                    Text = "ExecutionStopCriteria",
                                                    OnCanExecute = OnCanExecuteExecutionStopCriteria,
                                                    OnExecute = OnExecutionStopCriteriaSettings,
                                                    Icon = Constants.MISSING_ICON_URL
                                                };
            CodeCommand = new RoutedActionCommand("CodeCommand", typeof(ProjectViewModel))
                    {
                        Description = "View adapter code",
                        Text = "Code",
                        OnCanExecute = e => OnCanExecuteCodeGenerateExecute(),
                        OnExecute = OnCode,
                        Icon = Constants.MISSING_ICON_URL
                    };

            GenerateCommand = new RoutedActionCommand("GenerateCommand", typeof(ProjectViewModel))
                    {
                        Description = "Generate testcase",
                        Text = "Generate",
                        OnCanExecute = OnCanExecute,
                        OnExecute = OnGenerateAndExecute,
                        Icon = Constants.MISSING_ICON_URL
                    };

            ExecuteCommand = new RoutedActionCommand("ExecuteCommand", typeof(ProjectViewModel))
                    {
                        Description = "Execute scenario",
                        Text = "Execute",
                        OnCanExecute = OnCanExecute,
                        OnExecute = OnGenerateAndExecute,
                        Icon = Constants.MISSING_ICON_URL
                    };

            ReportsCommand = new RoutedActionCommand("ReportsCommand", typeof(ProjectViewModel))
            {
                Description = "Show reports",
                Text = "Reports",
                OnCanExecute = o => true,
                OnExecute = OnShowReports,
                Icon = Constants.MISSING_ICON_URL
            };

            ShowProjectModels = new RoutedActionCommand("ShowProjectModels", typeof(ProjectViewModel))
                                    {
                                        Description = "Show project models",
                                        Text = "Show project models",
                                        OnCanExecute = e => true,
                                        OnExecute = OnShowProjectModels,
                                        Icon = Constants.MISSING_ICON_URL
                                    };

            RenameProject = new RoutedActionCommand("RenameProject", typeof(ProjectViewModel))
                                {
                                    Description = "Rename Project",
                                    Text = "Rename",
                                    OnCanExecute = e => true,
                                    OnExecute = OnProjectRename,
                                    Icon = Constants.RENAME_ICON_URL
                                };

        }

        private void OnShowReports(object obj)
        {
            ReportsViewModel vm;

            var scenario = obj as ProjectScenarioViewModel;
            if (scenario == null)
            {
                vm = new ReportsViewModel(null, this.project, this);
            }
            else
            {
                vm = new ReportsViewModel(scenario.Scenario, this.project, this);
            }
            CurrentSetting = vm;
        }

        private bool OnCanExecute(object obj)
        {
            var canExecute = OnCanExecuteCodeGenerateExecute();
            if (canExecute)
            {
                ExecutionStatus.IsOk =
                    AdapterStatus.IsOk &&
                    AlgorithmStatus.IsOk &&
                    ExecutionStopCriteriaStatus.IsOk &&
                    GenerationStopCriteriaStatus.IsOk;

                if (ExecutionStatus.IsOk)
                {
                    ExecutionStatus.Msg = "Execution ok!";
                }
                else
                {
                    ExecutionStatus.Msg = "Some settings are not ok";
                    canExecute = false;
                }
            }
            else
            {
                ExecutionStatus.IsOk = false;
                ExecutionStatus.Msg = "No scenario selected or scenario has no models";
            }
            SendPropertyChanged("ExecutionStatus");
            return canExecute;
        }

        private bool canExecuteExecutionStopCriteria;

        private bool OnCanExecuteExecutionStopCriteria(object obj)
        {
            var canExecute = CurrentScenario != null;
            if (canExecute)
            {
                var stopCriteriaCount = currentScenario.Scenario.ExecutionStopCriteriasas.Count();
                ExecutionStopCriteriaStatus.IsOk = stopCriteriaCount > 0;
                if (stopCriteriaCount == 0)
                {
                    ExecutionStopCriteriaStatus.Msg = "You must have at least one Execution Stop Criteria";
                }
                else
                {
                    ExecutionStopCriteriaStatus.Msg = "Execution Stop Criterias ok!";
                }
            }
            else
            {
                ExecutionStopCriteriaStatus.IsOk = false;
                ExecutionStopCriteriaStatus.Msg = "No scenario selected";
            }
            if (canExecuteExecutionStopCriteria != ExecutionStopCriteriaStatus.IsOk)
            {
                canExecuteExecutionStopCriteria = ExecutionStopCriteriaStatus.IsOk;
                SendPropertyChanged("ExecutionStopCriteriaStatus");
                CommandManager.InvalidateRequerySuggested();
            }
            return canExecute;
        }

        private bool canExecuteAdapter;

        private bool OnCanExecuteAdapter(object obj)
        {
            var canExecute = CurrentScenario != null;
            if (canExecute)
            {
                var adapterCount = currentScenario.Scenario.Adapters.Count();
                AdapterStatus.IsOk = adapterCount > 0;
              var valid = string.Empty;
              currentScenario.Scenario.Adapters.ForEach(a => valid += ValidationManager.Validate(string.Empty, a));
                if (adapterCount == 0)
                {
                    AdapterStatus.Msg = "You must have at least one adapter";
                }
                else
                {
                  if (valid.Equals(string.Empty))
                  {
                    AdapterStatus.Msg = "Adapters ok!";
                  }
                  else
                  {
                    AdapterStatus.IsOk = false;
                    AdapterStatus.Msg = valid;
                  }
                }
            }
            else
            {
                AdapterStatus.IsOk = false;
                AdapterStatus.Msg = "No scenario selected";
            }
            if (canExecuteAdapter != AdapterStatus.IsOk)
            {
                canExecuteAdapter = AdapterStatus.IsOk;
                SendPropertyChanged("AdapterStatus");
                CommandManager.InvalidateRequerySuggested();
            }
            return canExecute;
        }

        private bool canExecuteGenerationStopCriteria;

        private bool OnCanExecuteGenerationStopCriteria(object obj)
        {
            var canExecute = CurrentScenario != null;
            if (canExecute)
            {
                var stopCriteriaCount = currentScenario.Scenario.GenerationStopCriterias.Count();
                GenerationStopCriteriaStatus.IsOk = stopCriteriaCount > 0;
                if (stopCriteriaCount == 0)
                {
                    GenerationStopCriteriaStatus.Msg = "You must have at least one Generation Stop Criteria";
                }
                else
                {
                    GenerationStopCriteriaStatus.Msg = "Generation Stop Criterias ok!";
                }
            }
            else
            {
                GenerationStopCriteriaStatus.IsOk = false;
                GenerationStopCriteriaStatus.Msg = "No scenario selected";
            }
            if (GenerationStopCriteriaStatus.IsOk != canExecuteGenerationStopCriteria)
            {
                canExecuteGenerationStopCriteria = GenerationStopCriteriaStatus.IsOk;
                SendPropertyChanged("GenerationStopCriteriaStatus");
                CommandManager.InvalidateRequerySuggested();
            }
            return canExecute;
        }

        private bool canExecuteAlgorithm;

        private bool OnCanExecuteAlgorithm(object obj)
        {
            var canExecute = CurrentScenario != null;
            if (canExecute)
            {
                var algorithmCount = currentScenario.Scenario.Algorithms.Count();
                AlgorithmStatus.IsOk = algorithmCount > 0;
                if (algorithmCount == 0)
                {
                    AlgorithmStatus.Msg = "You must have at least one algorithm";
                }
                else
                {
                    AlgorithmStatus.Msg = "Algorithms ok!";
                }
            }
            else
            {
                AlgorithmStatus.IsOk = false;
                AlgorithmStatus.Msg = "No scenario selected";
            }
            if (AlgorithmStatus.IsOk != canExecuteAlgorithm)
            {
                canExecuteAlgorithm = AlgorithmStatus.IsOk;
                SendPropertyChanged("AlgorithmStatus");
                CommandManager.InvalidateRequerySuggested();
            }
            return canExecute;
        }

        private bool OnCanExecuteCodeGenerateExecute()
        {
            return CurrentScenario != null && CurrentScenario.Models.Count > 0;
        }

        private void OnDelete(object sender, ExecutedRoutedEventArgs e)
        {

            var model = CurrentModel;//obj as ProjectModelViewModel;
            if (model != null)
            {
                if (currentScenario == null)
                {
                    this.project.RemoveModel(model.Model);
                }
                else
                {
                    Models.Remove(model);
                    CurrentScenario.Scenario.Remove(model.Model);
                }

                if (CurrentModel == model)
                {
                    CurrentModel = Models.FirstOrDefault();
                }
                model.Dispose();
            }
        }

        public override void ViewLoaded()
        {
            //if (currentProject != null)
            //    CurrentProject = currentProject;
            //if (currentScenario != null)
            //    CurrentScenario = currentScenario;            
        }
    }
}
