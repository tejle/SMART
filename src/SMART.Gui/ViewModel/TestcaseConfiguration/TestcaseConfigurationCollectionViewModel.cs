namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System;
    using System.Collections.ObjectModel;

    using Core.Interfaces;

    public class TestcaseConfigurationCollectionViewModel : ViewModelBase
    {
        private readonly ITestcase Testcase;

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override string Name { get { return "Testcase"; } }

        public override Guid Id
        {
            get { return Testcase.Id; }
            set { Testcase.Id = value; }
        }

        public ObservableCollection<TestcaseConfigurationViewModel> TestcaseSettings { get; set; }

        private TestcaseConfigurationViewModel currentTestcaseSetting;

        public TestcaseConfigurationViewModel CurrentTestcaseSetting
        {
            get { return this.currentTestcaseSetting; }
            set { this.currentTestcaseSetting = value; this.SendPropertyChanged("CurrentTestcaseSetting"); }
        }

        public TestcaseConfigurationCollectionViewModel(ITestcase testcase)
        {
            this.Testcase = testcase;
            this.TestcaseSettings = new ObservableCollection<TestcaseConfigurationViewModel>();
            this.TestcaseSettings.Add(new TestcaseConfigurationViewModel(this.Testcase));
            this.CurrentTestcaseSetting = this.TestcaseSettings[0];
        }
    }
}