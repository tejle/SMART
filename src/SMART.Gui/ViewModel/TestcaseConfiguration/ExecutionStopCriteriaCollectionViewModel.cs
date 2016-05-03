using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMART.Core.Interfaces.Factories;
using SMART.Core.Interfaces.Repository;

namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System.Collections.ObjectModel;

    using Commands;

    using Core.Events;
    using Core.Interfaces;
    using SMART.IOC;

    public class ExecutionStopCriteriaCollectionViewModel : ViewModelBase
    {
        private readonly ITestcase Testcase;
        private readonly ProjectViewModel _projectViewModel;

        public RoutedActionCommand AddStopCriteria { get; set; }
        public RoutedActionCommand RemoveStopCriteria { get; set; }
        public RoutedActionCommand Close { get; set; }

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }

        public override string Name { get { return "ExecutionStopCriterias"; } }

        public override Guid Id
        {
            get;
            set;
        }

        public ObservableCollection<ExecutionStopCriteriaViewModel> ExecutionStopCriterias { get; set; }

        private ExecutionStopCriteriaViewModel currentExecutionStopCriteria;

        public ExecutionStopCriteriaViewModel CurrentExecutionStopCriteria
        {
            get { return this.currentExecutionStopCriteria; }
            set { this.currentExecutionStopCriteria = value; this.SendPropertyChanged("CurrentExecutionStopCriteria"); }
        }

        private ObservableCollection<ExecutionStopCriteriaTypeViewModel> availableExecutionStopCriterias;
        public ObservableCollection<ExecutionStopCriteriaTypeViewModel> AvailableExecutionStopCriterias
        {
            get
            {
                if (this.availableExecutionStopCriterias == null)
                {
                    this.availableExecutionStopCriterias = new ObservableCollection<ExecutionStopCriteriaTypeViewModel>();
                    var stopCriteriaRepository = Resolver.Resolve<IExecutionStopCriteriaRepository>();
                    var classDescriptions = stopCriteriaRepository.GetAll();

                    foreach (var a in classDescriptions)
                    {
                        this.availableExecutionStopCriterias.Add(new ExecutionStopCriteriaTypeViewModel(a));
                    }
                }
                return this.availableExecutionStopCriterias;
            }
        }

        private ExecutionStopCriteriaTypeViewModel currentSelectedAvailableExecutionStopCriteria;

        public ExecutionStopCriteriaTypeViewModel CurrentSelectedAvailableExecutionStopCriteria
        {
            get { return this.currentSelectedAvailableExecutionStopCriteria; }
            set { this.currentSelectedAvailableExecutionStopCriteria = value; this.SendPropertyChanged("CurrentSelectedAvailableExecutionStopCriteria"); }
        }

        public ExecutionStopCriteriaCollectionViewModel(ITestcase testcase, ProjectViewModel projectViewModel)
        {
            this.Testcase = testcase;
            _projectViewModel = projectViewModel;

            this.AddStopCriteria = new RoutedActionCommand("AddStopCriteria", typeof(GenerationStopCriteriaCollectionViewModel))
            {
                OnCanExecute = this.OnCanAddStopCriteria,
                OnExecute = this.OnAddStopCriteria
            };

            this.RemoveStopCriteria = new RoutedActionCommand("RemoveStopCriteria", typeof(GenerationStopCriteriaCollectionViewModel))
            {
                OnCanExecute = this.OnCanRemoveStopCriteria,
                OnExecute = this.OnRemoveStopCriteria
            };
            this.Close = new RoutedActionCommand("Close", typeof(GenerationStopCriteriaCollectionViewModel))
            {
                OnCanExecute = (o) => true,
                OnExecute = this.OnClose
            };

            this.ExecutionStopCriterias = new ObservableCollection<ExecutionStopCriteriaViewModel>();
            foreach (var criteria in testcase.ExecutionStopCriteriasas)
            {
                this.ExecutionStopCriterias.Add(new ExecutionStopCriteriaViewModel(criteria, this));
            }
            if (this.ExecutionStopCriterias.Count > 0)
            {
                this.CurrentExecutionStopCriteria = this.ExecutionStopCriterias[0];
            }

            this.Testcase.CollectionChanged += this.Testcase_CollectionChanged;
        }

        private void OnClose(object obj)
        {
            _projectViewModel.CurrentSetting = null;
        }

        private void Testcase_CollectionChanged(object sender, SmartNotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case SmartNotifyCollectionChangedAction.Add:
                    if (e.CollectionName.Equals("ExecutionStopCriterias"))
                    {
                        foreach (var c in e.NewItems)
                        {
                            this.ExecutionStopCriterias.Add(new ExecutionStopCriteriaViewModel(c as IExecutionStopCriteria, this));
                        }
                        this.CurrentExecutionStopCriteria = this.ExecutionStopCriterias.Last();
                    }
                    break;
                case SmartNotifyCollectionChangedAction.Remove:
                    if (e.CollectionName.Equals("ExecutionStopCriterias"))
                    {
                        foreach (var c in e.OldItems)
                        {
                            var criteriaVM = (from cvm in this.ExecutionStopCriterias where cvm.StopCriteria == c select cvm).FirstOrDefault();
                            if (criteriaVM != null)
                            {
                                this.ExecutionStopCriterias.Remove(criteriaVM);
                            }
                        }
                    }
                    break;
            }
        }

        private void OnRemoveStopCriteria(object obj)
        {
            var stopCriteria = obj as ExecutionStopCriteriaViewModel;
            if (stopCriteria != null)
            {
                this.Testcase.Remove(stopCriteria.StopCriteria);
            }
        }

        private bool OnCanRemoveStopCriteria(object obj)
        {
            return true;
        }

        private void OnAddStopCriteria(object obj)
        {
            var stopCriteriaFactory = Resolver.Resolve<IExecutionStopCriteriaFactory>();
            var stopCriteria = stopCriteriaFactory.Create(this.CurrentSelectedAvailableExecutionStopCriteria.StopCriteria.Type);
            this.Testcase.Add(stopCriteria);
        }

        private bool OnCanAddStopCriteria(object obj)
        {
            return true;
        }
    }
}