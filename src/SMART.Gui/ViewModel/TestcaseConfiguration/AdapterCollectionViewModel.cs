using SMART.Core.Interfaces.Factories;

namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Commands;

    using Core.Events;
    using Core.Interfaces;
    using Core.Interfaces.Repository;

    using IOC;

    public class AdapterCollectionViewModel : ViewModelBase
    {
        private readonly ITestcase Testcase;
        private readonly ProjectViewModel _projectViewModel;

        public RoutedActionCommand AddAdapter { get; set; }
        public RoutedActionCommand RemoveAdapter { get; set; }
        public RoutedActionCommand Close { get; set; }

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override string Name { get { return "Adapters"; } }

        public override Guid Id
        {
            get;
            set;
        }

        public ObservableCollection<AdapterViewModel> Adapters { get; set; }

        private AdapterViewModel currentAdapter;

        public AdapterViewModel CurrentAdapter
        {
            get { return this.currentAdapter; }
            set { this.currentAdapter = value; this.SendPropertyChanged("CurrentAdapter"); }
        }

        private ObservableCollection<AdapterTypeViewModel> availableAdapters;
        public ObservableCollection<AdapterTypeViewModel> AvailableAdapters
        {
            get
            {
                if (this.availableAdapters == null)
                {
                    this.availableAdapters = new ObservableCollection<AdapterTypeViewModel>();
                    var adapterRepository = Resolver.Resolve<IAdapterRepository>();
                    var classDescriptions = adapterRepository.GetAll();

                    foreach (var a in classDescriptions)
                    {
                        this.availableAdapters.Add(new AdapterTypeViewModel(a));                        
                    }
                }
                return this.availableAdapters;
            }
        }

        private AdapterTypeViewModel currentSelectedAvailableAdapter;

        public AdapterTypeViewModel CurrentSelectedAvailableAdapter
        {
            get { return this.currentSelectedAvailableAdapter; }
            set { this.currentSelectedAvailableAdapter = value; this.SendPropertyChanged("CurrentSelectedAvailableAdapter"); }
        }

        public AdapterCollectionViewModel(ITestcase testcase, ProjectViewModel projectViewModel)
        {
            this.Testcase = testcase;
            _projectViewModel = projectViewModel;

            this.AddAdapter = new RoutedActionCommand("AddAdapter", typeof(AdapterCollectionViewModel))
                             {
                                     OnCanExecute = this.OnCanAddAdapter,
                                     OnExecute = this.OnAddAdapter
                             };
            this.RemoveAdapter = new RoutedActionCommand("RemoveAdapter", typeof(AdapterCollectionViewModel))
                                {
                                        OnCanExecute = this.OnCanRemoveAdapter,
                                        OnExecute = this.OnRemoveAdapter
                                };
            this.Close = new RoutedActionCommand("Close", typeof(AdapterCollectionViewModel))
                                {
                                        OnCanExecute = (o) => true,
                                        OnExecute = this.OnClose
                                };

            this.Adapters = new ObservableCollection<AdapterViewModel>();
            foreach (var adapter in testcase.Adapters)
            {
                this.Adapters.Add(new AdapterViewModel(adapter, this));
            }
            if (this.Adapters.Count > 0)
            {
                this.CurrentAdapter = this.Adapters[0];
            }


            this.Testcase.CollectionChanged += this.Testcase_CollectionChanged;
        }

        private void OnClose(object obj)
        {
            _projectViewModel.CurrentSetting = null;
        }

        private void OnRemoveAdapter(object obj)
        {
            var adapter = obj as AdapterViewModel;
            if (adapter != null)
            {
                this.Testcase.Remove(adapter.Adapter);
            }
        }

        private bool OnCanRemoveAdapter(object obj)
        {
            return true;
        }

        void Testcase_CollectionChanged(object sender, SmartNotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case SmartNotifyCollectionChangedAction.Add:
                    if (e.CollectionName.Equals("Adapters"))
                    {
                        foreach (var a in e.NewItems)
                        {
                            this.Adapters.Add(new AdapterViewModel(a as IAdapter, this));
                        }
                        this.CurrentAdapter = this.Adapters.Last();
                    }
                    break;     
                case SmartNotifyCollectionChangedAction.Remove:
                    if (e.CollectionName.Equals("Adapters"))
                    {
                        foreach (var a in e.OldItems)
                        {
                            var adapterVM = (from avm in this.Adapters where avm.Adapter == a select avm).FirstOrDefault();
                            if (adapterVM != null)
                            {
                                this.Adapters.Remove(adapterVM);
                            }
                        }
                    }
                    break;                    
            }
        }

        private bool OnCanAddAdapter(object obj)
        {
            return true;
        }

        private void OnAddAdapter(object obj)
        {
            var adapterFactory = Resolver.Resolve<IAdapterFactory>();
            var adapter = adapterFactory.Create(this.currentSelectedAvailableAdapter.Adapter.Type);
            this.Testcase.Add(adapter);            
        }
    }
}