namespace SMART.Gui.ViewModel
{
    using System;

    using Commands;

    using Core.Interfaces;
    using Core.Interfaces.Services;

    using Events;
    using IOC;

    using TestcaseCodeGeneration;

    using TestcaseConfiguration;

    using TestcaseExecution;

    public class TestcaseCommonCommandsViewModel:ViewModelBase
    {
        private readonly IViewModel Parent;

        private readonly ITestcase Testcase;

        private readonly IEventService EventService;

        public RoutedActionCommand ConfigScenario { get; set; }
        public RoutedActionCommand GenerateCode { get; set; }
        public RoutedActionCommand ExecuteScenario { get; set; }
        public RoutedActionCommand CloseWindow { get; set; }

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override Guid Id
        {
            get { return Testcase.Id; }
            set { Testcase.Id = value; }
        }

        private bool configButtonChecked;

        public bool ConfigButtonChecked
        {
            get { return this.configButtonChecked; }
            set { this.configButtonChecked = value; this.SendPropertyChanged("ConfigButtonChecked"); }
        }

        private bool codeButtonChecked;

        public bool CodeButtonChecked
        {
            get { return this.codeButtonChecked; }
            set { this.codeButtonChecked = value; this.SendPropertyChanged("CodeButtonChecked"); }
        }

        private bool executeButtonChecked;

        public bool ExecuteButtonChecked
        {
            get { return this.executeButtonChecked; }
            set { this.executeButtonChecked = value; this.SendPropertyChanged("ExecuteButtonChecked"); }
        }

        public TestcaseCommonCommandsViewModel(IViewModel parent, ITestcase testcase) : this(parent, testcase, Resolver.Resolve<IEventService>())
        {}
        public TestcaseCommonCommandsViewModel(IViewModel parent, ITestcase testcase, IEventService eventService)
        {
            Parent = parent;
            Testcase = testcase;
            EventService = eventService;

            this.ConfigScenario = new RoutedActionCommand("ConfigScenario", typeof(TestcaseCommonCommandsViewModel))
            {
                Description = "Configure scenario",
                OnCanExecute = (o) => true,
                OnExecute = this.OnConfigScenario
            };

            this.GenerateCode = new RoutedActionCommand("GenerateCode", typeof(TestcaseCommonCommandsViewModel))
            {
                Description = "Generate code",
                OnCanExecute = (o) => true,
                OnExecute = this.OnGenerateCode
            };

            this.ExecuteScenario = new RoutedActionCommand("ExecuteScenario", typeof(TestcaseCommonCommandsViewModel))
            {
                Description = "Execute scenario",
                OnCanExecute = (o) => true,
                OnExecute = this.OnExecuteScenario
            };

            this.CloseWindow = new RoutedActionCommand("Close", typeof(TestcaseCommonCommandsViewModel))
            {
                Description = "Close",
                OnCanExecute = (o) => true,
                OnExecute = this.OnCloseWindow
            };
        }

        private void OnExecuteScenario(object obj)
        {
            this.EventService.GetEvent<OpenPopUpEvent>().Publish(new OpenPopUpEventArgs(typeof(TestcaseExecutionCompositeViewModel), Testcase.Id));
        }

        private void OnGenerateCode(object obj)
        {
            this.EventService.GetEvent<OpenPopUpEvent>().Publish(new OpenPopUpEventArgs(typeof(TestcaseCodeGenerationViewModel), Testcase.Id));            
        }

        private void OnConfigScenario(object obj)
        {
            this.EventService.GetEvent<OpenPopUpEvent>().Publish(new OpenPopUpEventArgs(typeof(TestcaseConfigurationCompositeViewModel), Testcase.Id));            
        }

        private void OnCloseWindow(object obj)
        {
            this.EventService.GetEvent<ClosePopUpEvent>().Publish(Parent);                        
        }
    }
}
