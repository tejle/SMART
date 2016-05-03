namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System;
    using System.Collections.ObjectModel;

    using Commands;

    using Core;
    using Core.Interfaces;
    using Core.Metadata;

    public class AlgorithmViewModel: ViewModelBase
    {
        public readonly IAlgorithm Algorithm;

        public RoutedActionCommand Remove { get; set; }

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override string Name
        {
            get
            {
                return this.Algorithm.MetadataView<AlgorithmAttribute>().Name;
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
                return this.Algorithm.MetadataView<AlgorithmAttribute>().Description;
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
                    this.Algorithm.GetConfig().ForEach(cs => this.configurations.Add(new ConfigSettingViewModel(cs.Value, this.Algorithm)));
                }
                return this.configurations;
            }
            set { this.configurations = value; }
        }

        public AlgorithmViewModel(IAlgorithm algorithm, AlgorithmCollectionViewModel viewModel)
        {
            this.Algorithm = algorithm;
            this.Remove = viewModel.RemoveAlgorithm;
        }
    }
}