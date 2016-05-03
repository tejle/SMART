namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System;
    using System.Collections.ObjectModel;

    using Commands;

    using Core;
    using Core.Interfaces;
    using Core.Metadata;

    public class ExecutionStopCriteriaViewModel: ViewModelBase
    {
        public readonly IExecutionStopCriteria StopCriteria;

        public RoutedActionCommand Remove { get; set; }   

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override string Name
        {
            get
            {
                return this.StopCriteria.MetadataView<ExecutionStopCriteriaAttribute>().Name;
            }
        }

        public override Guid Id
        {
            get;
            set;
        }

        public string Description
        {
            get
            {
                return this.StopCriteria.MetadataView<ExecutionStopCriteriaAttribute>().Description;
            }
        }

        public bool ShowDeleteButton { get { return true; } }

        private ObservableCollection<ConfigSettingViewModel> configurations;

        public ObservableCollection<ConfigSettingViewModel> Configurations
        {
            get
            {
                if (this.configurations == null)
                {
                    this.configurations = new ObservableCollection<ConfigSettingViewModel>();
                    this.StopCriteria.GetConfig().ForEach(cs => this.configurations.Add(new ConfigSettingViewModel(cs.Value, this.StopCriteria)));
                }
                return this.configurations;
            }
            set { this.configurations = value; }
        }

        public ExecutionStopCriteriaViewModel(IExecutionStopCriteria stopCriteria, ExecutionStopCriteriaCollectionViewModel viewModel)
        {
            this.StopCriteria = stopCriteria;
            Remove = viewModel.RemoveStopCriteria;
        }
    }
}