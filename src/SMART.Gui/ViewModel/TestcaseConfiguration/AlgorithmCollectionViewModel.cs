using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMART.Core.Interfaces.Factories;

namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System.Collections.ObjectModel;

    using Commands;

    using Core.Events;
    using Core.Interfaces;
    using Core.Interfaces.Repository;

    using IOC;

    public class AlgorithmCollectionViewModel : ViewModelBase
    {
        private readonly ITestcase Testcase;
        private readonly ProjectViewModel _projectViewModel;

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override string Name { get { return "Algorithms"; } }

        public override Guid Id
        {
            get;
            set;
        }

        public RoutedActionCommand AddAlgorithm { get; set; }
        public RoutedActionCommand RemoveAlgorithm { get; set; }
        public RoutedActionCommand Close { get; set; }


        public ObservableCollection<AlgorithmViewModel> Algorithms { get; set; }

        private AlgorithmViewModel currentAlgorithm;

        public AlgorithmViewModel CurrentAlgorithm
        {
            get { return this.currentAlgorithm; }
            set { this.currentAlgorithm = value; this.SendPropertyChanged("CurrentAlgorithm"); }
        }

        private ObservableCollection<AlgorithmTypeViewModel> availableAlgorithms;
        public ObservableCollection<AlgorithmTypeViewModel> AvailableAlgorithms
        {
            get
            {
                if (this.availableAlgorithms == null)
                {
                    this.availableAlgorithms = new ObservableCollection<AlgorithmTypeViewModel>();
                    
                    var algorithmRepository = Resolver.Resolve<IAlgorithmRepository>();
                    
                    var descriptions = algorithmRepository.GetAll();
                    
                    foreach (var a in descriptions)
                    {
                        this.availableAlgorithms.Add(new AlgorithmTypeViewModel(a));
                    }
                }
                return this.availableAlgorithms;
            }
        }

        private AlgorithmTypeViewModel currentSelectedAvailableAlgorithm;

        public AlgorithmTypeViewModel CurrentSelectedAvailableAlgorithm
        {
            get { return this.currentSelectedAvailableAlgorithm; }
            set { this.currentSelectedAvailableAlgorithm = value; this.SendPropertyChanged("CurrentSelectedAvailableAlgorithm"); }
        }

        public AlgorithmCollectionViewModel(ITestcase testcase, ProjectViewModel projectViewModel)
        {
            this.Testcase = testcase;
            _projectViewModel = projectViewModel;

            this.AddAlgorithm = new RoutedActionCommand("AddAlgorithm", typeof(AlgorithmCollectionViewModel))
                               {
                                       OnCanExecute = this.OnCanAddAlgorithm,
                                       OnExecute = this.OnAddAlgorithm
                               };
            this.RemoveAlgorithm = new RoutedActionCommand("RemoveAlgorithm", typeof(AlgorithmCollectionViewModel))
                                  {
                                          OnCanExecute = this.OnCanRemoveAlgorithm,
                                          OnExecute = this.OnRemoveAlgorithm
                                  };
            this.Close = new RoutedActionCommand("Close", typeof(AlgorithmCollectionViewModel))
            {
                OnCanExecute = (o) => true,
                OnExecute = this.OnClose
            };

            this.Algorithms = new ObservableCollection<AlgorithmViewModel>();
            foreach (var algorithm in testcase.Algorithms)
            {
                this.Algorithms.Add(new AlgorithmViewModel(algorithm, this));
            }
            if (this.Algorithms.Count > 0)
            {
                this.CurrentAlgorithm = this.Algorithms[0];
            }

            this.Testcase.CollectionChanged += this.Testcase_CollectionChanged;
        }

        private void OnClose(object obj)
        {
            _projectViewModel.CurrentSetting = null;
        }

        void Testcase_CollectionChanged(object sender, SmartNotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case SmartNotifyCollectionChangedAction.Add:
                    if (e.CollectionName.Equals("Algorithms"))
                    {
                        foreach (var a in e.NewItems)
                        {
                            this.Algorithms.Add(new AlgorithmViewModel(a as IAlgorithm, this));
                        }
                        this.CurrentAlgorithm = this.Algorithms.Last();
                    }
                    break;
                case SmartNotifyCollectionChangedAction.Remove:
                    if (e.CollectionName.Equals("Algorithms"))
                    {
                        foreach (var a in e.OldItems)
                        {
                            var algorithmVM = (from avm in this.Algorithms where avm.Algorithm == a select avm).FirstOrDefault();
                            if (algorithmVM != null)
                            {
                                this.Algorithms.Remove(algorithmVM);
                            }
                        }
                    }
                    break;
            }
        }

        private void OnRemoveAlgorithm(object obj)
        {
            var algorithm = obj as AlgorithmViewModel;
            if (algorithm != null)
            {
                this.Testcase.Remove(algorithm.Algorithm);
            }
        }

        private bool OnCanRemoveAlgorithm(object obj)
        {
            return true;
        }

        private void OnAddAlgorithm(object obj)
        {
            var algorithmFactory = Resolver.Resolve<IAlgorithmFactory>();
            
            var algo = algorithmFactory.Create(this.currentSelectedAvailableAlgorithm.Algorithm.Type);

            this.Testcase.Add(algo);
        }

        private bool OnCanAddAlgorithm(object obj)
        {
            return true;
        }
    }
}