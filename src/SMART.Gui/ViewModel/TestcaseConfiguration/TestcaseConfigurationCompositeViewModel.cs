namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System;
    using System.Collections.ObjectModel;

    using Commands;

    using Core.Interfaces;
    using Core.Interfaces.Services;

    using Events;

    using IOC;

    public class TestcaseConfigurationCompositeViewModel : ViewModelBase
    {
        private readonly ITestcase Testcase;

        private readonly IEventService EventService;

        public override string Icon
        {
            get { return Constants.TESTCASE_CONFIGURATION_ICON; }
        }

        public override Guid Id
        {
            get { return Testcase.Id; }
            set { Testcase.Id = value; }
        }

        private TestcaseCommonCommandsViewModel commonCommands;

        public TestcaseCommonCommandsViewModel CommonCommands
        {
            get { return this.commonCommands; }
            set { this.commonCommands = value; this.SendPropertyChanged("CommonCommands"); }
        }

        public ObservableCollection<IViewModel> Tabs { get; set; }

        public TestcaseConfigurationCompositeViewModel(ITestcase testcase) : this(testcase, Resolver.Resolve<IEventService>())
        {            
        }

        public TestcaseConfigurationCompositeViewModel(ITestcase testcase, IEventService eventService): base(testcase.Name)
        {
            this.Testcase = testcase;
            this.EventService = eventService;

            CommonCommands = new TestcaseCommonCommandsViewModel(this, Testcase)
            {
                //ConfigButtonChecked = true
            };

            //this.Tabs = new ObservableCollection<IViewModel>
            //           {
            //                   new TestcaseConfigurationCollectionViewModel(this.Testcase),
            //                   new AdapterCollectionViewModel(this.Testcase),
            //                   new AlgorithmCollectionViewModel(this.Testcase),
            //                   new GenerationStopCriteriaCollectionViewModel(this.Testcase),
            //                   new ExecutionStopCriteriaCollectionViewModel(this.Testcase)
            //           };
        }

        public override void ViewLoaded()
        {
            commonCommands.CodeButtonChecked = false;
            commonCommands.ExecuteButtonChecked = false;
            commonCommands.ConfigButtonChecked = true;

        }
    }
}