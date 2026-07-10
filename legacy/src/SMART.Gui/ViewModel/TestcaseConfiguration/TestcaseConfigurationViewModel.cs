namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System;
    using System.Collections.ObjectModel;

    using Core;
    using Core.Interfaces;

    public class TestcaseConfigurationViewModel: ViewModelBase
    {
        private readonly ITestcase Testcase;

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override string Name { get { return "General settings"; } }

        public override Guid Id
        {
            get { return Testcase.Id; }
            set{ Testcase.Id = value;}
        }

        public string Description { get { return "General settings for the testcase"; } }

        public bool ShowDeleteButton { get { return false; } }

        private ObservableCollection<ConfigSettingViewModel> configurations;

        public ObservableCollection<ConfigSettingViewModel> Configurations
        {
            get
            {
                if (this.configurations == null)
                {
                    this.configurations = new ObservableCollection<ConfigSettingViewModel>();
                    this.Testcase.GetConfig().ForEach(cs => this.configurations.Add(new ConfigSettingViewModel(cs.Value, this.Testcase)));
                }
                return this.configurations;
            }
            set { this.configurations = value; }
        }

        public TestcaseConfigurationViewModel(ITestcase testcase)
        {
            this.Testcase = testcase;
        }
    }
}