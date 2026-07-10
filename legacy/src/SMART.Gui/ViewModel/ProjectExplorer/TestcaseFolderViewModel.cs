using SMART.Core.DomainModel;
using SMART.Core.Events;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Services;
using SMART.Gui.Events;

namespace SMART.Gui.ViewModel.ProjectExplorer
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Commands;

    using IOC;

    using TestcaseCodeGeneration;

    using TestcaseConfiguration;

    using TestcaseExecution;

    public class TestcaseFolderViewModel : ViewModelBase
    {
        private readonly ITestcase testcase;
        public ITestcase Testcase { get { return testcase; } }

        public Guid TestcaseId { get { return this.testcase.Id; } }

        private readonly IProject project;

        private readonly IEventService EventService;

        public ObservableCollection<TestcaseModelFolderViewModel> FolderViewModels
        {
            get;
            private set;
        }

        public RoutedActionCommand Open { get; private set; }
        public RoutedActionCommand Remove { get; private set; }
        public RoutedActionCommand Rename { get; private set; }
        public RoutedActionCommand AddModel { get; private set; }

        public RoutedActionCommand Configure { get; private set; }
        public RoutedActionCommand Execute { get; private set; }
        public RoutedActionCommand GenerateCode { get; private set; }

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
                if (!this.testcase.Name.Equals(value))
                    this.testcase.Name = value;
            }
        }

        public override Guid Id
        {
            get { return testcase.Id; }
            set { testcase.Id = value; }
        }

        public TestcaseFolderViewModel(ITestcase testcase, IProject project)
            : this(testcase, project, Resolver.Resolve<IEventService>())
        {

        }

        public TestcaseFolderViewModel(ITestcase testcase, IProject project, IEventService eventService)
            : base(testcase.Name)
        {
            this.testcase = testcase;
            testcase.PropertyChanged += this.testcase_PropertyChanged;
            testcase.CollectionChanged += this.testcase_CollectionChanged;
            this.project = project;
            this.EventService = eventService;

            this.CreateCollection();
            this.CreateCommand();
        }

        void testcase_CollectionChanged(object sender, SmartNotifyCollectionChangedEventArgs e)
        {
            if (e.CollectionName.Equals("Models"))
            {
                if (e.Action == SmartNotifyCollectionChangedAction.Add)
                {
                    foreach (var model in e.NewItems.OfType<IModel>())
                    {
                        if (model == null)
                            return;

                        var g = new TestcaseModelFolderViewModel(model, this.testcase);

                        this.FolderViewModels.Add(g);
             
                    }
                }
                else if (e.Action == SmartNotifyCollectionChangedAction.Remove)
                {
                    foreach (var model in e.OldItems.OfType<IModel>())
                    {
                        var modelViewModel = this.FolderViewModels.Where(gvw => gvw.ModelId == model.Id);
                        if (modelViewModel.Count() == 0)
                            return;

                        this.FolderViewModels.Remove(modelViewModel.First());
                    }
                }
                else if(e.Action == SmartNotifyCollectionChangedAction.Move)
                {
                    throw new ArgumentException("Supported method???");
                }
                else if(e.Action == SmartNotifyCollectionChangedAction.Replace)
                {
                    throw new ArgumentException("Supported method???");
                    
                }
                else if(e.Action == SmartNotifyCollectionChangedAction.Reset)
                {
                    throw new ArgumentException("Supported method???");
                    
                }
            
            }
        }

        void testcase_PropertyChanged(object sender, SmartPropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Models"))
            {

            }
            if (e.PropertyName.Equals("Name"))
            {
                this.Name = this.testcase.Name;
            }
        }


        private void CreateCommand()
        {
            this.Open = new RoutedActionCommand("Open", typeof(TestcaseFolderViewModel))
                       {
                           Description = "Open testcase",
                           OnCanExecute = (o) => true,
                           OnExecute = this.OnOpen,
                           Text = "Open",
                           Icon = Constants.OPEN_ICON_URL
                       };
            this.Remove = new RoutedActionCommand("Remove", typeof(TestcaseFolderViewModel))
                         {
                             Description = "Remove testcase",
                             OnCanExecute = (o) => true,
                             OnExecute = this.OnRemove,
                             Text = "Remove",
                             Icon = Constants.DELETE_ICON_URL
                         };
            this.Rename = new RoutedActionCommand("Rename", typeof(TestcaseFolderViewModel))
                         {
                             Description = "Rename testcase",
                             OnCanExecute = (o) => true,
                             OnExecute = this.OnRename,
                             Text = "Rename",
                             Icon = Constants.RENAME_ICON_URL
                         };
            this.AddModel = new RoutedActionCommand("AddModel", typeof(TestcaseFolderViewModel))
                           {
                               Description = "Add model to testcase",
                               OnCanExecute = (o) => true,
                               OnExecute = this.OnAddModel,
                               Text = "Add Model",
                               Icon = Constants.MODEL_ADD_ICON_URL
                           };

            this.Configure = new RoutedActionCommand("Configure", typeof(TestcaseFolderViewModel))
                            {
                                Description = "Configure testcase",
                                OnCanExecute = (o) => true,
                                OnExecute = this.OnConfigure,
                                Text = "Configure",
                                Icon = Constants.TESTCASE_CONFIGURATION_ICON
                            };

            this.Execute = new RoutedActionCommand("Execute", typeof(TestcaseFolderViewModel))
                            {
                                Description = "Execute testcase",
                                OnCanExecute = (o) => true,
                                OnExecute = this.OnExecute,
                                Text = "Execute",
                                Icon = Constants.TESTCASE_EXECUTION_ICON
                            };
            this.GenerateCode = new RoutedActionCommand("GenerateCode", typeof(TestcaseFolderViewModel))
                            {
                                Description = "Generate adapter code",
                                OnCanExecute = (o) => true,
                                OnExecute = this.OnGenerateCode,
                                Text = "Generate Code",
                                Icon = Constants.TESTCASE_GENERATE_CODE_ICON
                            };
        }

        private void OnGenerateCode(object obj)
        {
            this.EventService.GetEvent<OpenPopUpEvent>().Publish(new OpenPopUpEventArgs(typeof(TestcaseCodeGenerationViewModel), testcase.Id));
        }

        private void OnExecute(object obj)
        {
            this.EventService.GetEvent<OpenPopUpEvent>().Publish(new OpenPopUpEventArgs(typeof(TestcaseExecutionCompositeViewModel), testcase.Id));
        }

        private void OnConfigure(object obj)
        {
            this.EventService.GetEvent<OpenPopUpEvent>().Publish(new OpenPopUpEventArgs(typeof(TestcaseConfigurationCompositeViewModel), testcase.Id));
        }

        protected virtual void OnAddModel(object obj)
        {
            var model = Resolver.Resolve<Model>();
            model.Name = "new model";
            this.project.AddModel(model, testcase);
            
        }

        protected virtual void OnRename(object obj)
        {
            string name = "new name";
            this.Name = name;
        }

        protected virtual void OnRemove(object obj)
        {
            var success = this.project.RemoveTestCase(this.testcase);
        }

        protected virtual void OnOpen(object obj)
        {
            var eventService = Resolver.Resolve<IEventService>();
            eventService.GetEvent<OpenTestcaseViewEvent>().Publish(this.testcase);
        }

        private void CreateCollection()
        {
            var list = from g in this.testcase.Models select new TestcaseModelFolderViewModel(g as Model, this.testcase);

            this.FolderViewModels = new ObservableCollection<TestcaseModelFolderViewModel>();
            foreach (var t in list)
            {
                this.FolderViewModels.Add(t);
            }
        }

        public override string Icon
        {
            get { return Constants.TESTCASE_ICON_URL; }
        }
    }
}