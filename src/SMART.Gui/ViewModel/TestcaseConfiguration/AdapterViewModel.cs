namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System;
    using System.Collections.ObjectModel;

    using Commands;

    using Core;
    using Core.Interfaces;
    using Core.Metadata;

    public class AdapterViewModel: ViewModelBase
    {
        public readonly IAdapter Adapter;

        public RoutedActionCommand Remove { get; set; }        

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override string Name
        {
            get
            {
                return this.Adapter.MetadataView<AdapterAttribute>().Name;
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
                return this.Adapter.MetadataView<AdapterAttribute>().Description;
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
                    this.Adapter.GetConfig().ForEach(cs => this.configurations.Add(new ConfigSettingViewModel(cs.Value, this.Adapter)));
                }
                return this.configurations;
            }
            set { this.configurations = value; }
        }

        public AdapterViewModel(IAdapter adapter, AdapterCollectionViewModel viewModel)
        {
            this.Adapter = adapter;
            this.Remove = viewModel.RemoveAdapter;
        }
    }
}