using System.Linq;
using SMART.Core.Interfaces.Factories;

namespace SMART.Gui.ViewModel.TestcaseConfiguration
{
    using System;
    using System.Collections.ObjectModel;

    using Commands;

    using Core.Events;
    using Core.Interfaces;
    using Core.Interfaces.Repository;

    using IOC;

    public class GenerationStopCriteriaCollectionViewModel : ViewModelBase
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

        public override string Name { get { return "GenerationStopCriterias"; } }

        public override Guid Id
        {
            get;
            set;
        }

        public ObservableCollection<GenerationStopCriteriaViewModel> GenerationStopCriterias { get; set; }

        private GenerationStopCriteriaViewModel currentGenerationStopCriteria;

        public GenerationStopCriteriaViewModel CurrentGenerationStopCriteria
        {
            get { return this.currentGenerationStopCriteria; }
            set { this.currentGenerationStopCriteria = value; this.SendPropertyChanged("CurrentGenerationStopCriteria"); }
        }

        private ObservableCollection<GenerationStopCriteriaTypeViewModel> availableGenerationStopCriterias;
        public ObservableCollection<GenerationStopCriteriaTypeViewModel> AvailableGenerationStopCriterias
        {
            get
            {
                if (this.availableGenerationStopCriterias == null)
                {
                    this.availableGenerationStopCriterias = new ObservableCollection<GenerationStopCriteriaTypeViewModel>();
                    var stopCriteriaRepository = Resolver.Resolve<IGenerationStopCriteriaRepository>();
                    var classDescriptions = stopCriteriaRepository.GetAll();

                    foreach (var a in classDescriptions)
                    {
                        this.availableGenerationStopCriterias.Add(new GenerationStopCriteriaTypeViewModel(a));
                    }
                }
                return this.availableGenerationStopCriterias;
            }
        }

        private GenerationStopCriteriaTypeViewModel currentSelectedAvailableGenerationStopCriteria;

        public GenerationStopCriteriaTypeViewModel CurrentSelectedAvailableGenerationStopCriteria
        {
            get { return this.currentSelectedAvailableGenerationStopCriteria; }
            set { this.currentSelectedAvailableGenerationStopCriteria = value; this.SendPropertyChanged("CurrentSelectedAvailableGenerationStopCriteria"); }
        }

        public GenerationStopCriteriaCollectionViewModel(ITestcase testcase, ProjectViewModel projectViewModel)
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

            this.GenerationStopCriterias = new ObservableCollection<GenerationStopCriteriaViewModel>();
            foreach (var criteria in testcase.GenerationStopCriterias)
            {
                this.GenerationStopCriterias.Add(new GenerationStopCriteriaViewModel(criteria, this));
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
                    if (e.CollectionName.Equals("GenerationStopCriterias"))
                    {
                        foreach (var c in e.NewItems)
                        {
                            this.GenerationStopCriterias.Add(new GenerationStopCriteriaViewModel(c as IGenerationStopCriteria, this));
                        }
                        this.CurrentGenerationStopCriteria = this.GenerationStopCriterias.Last();
                    }
                    break;
                case SmartNotifyCollectionChangedAction.Remove:
                    if (e.CollectionName.Equals("GenerationStopCriterias"))
                    {
                        foreach (var c in e.OldItems)
                        {
                            var criteriaVM = (from cvm in this.GenerationStopCriterias where cvm.StopCriteria == c select cvm).FirstOrDefault();
                            if (criteriaVM != null)
                            {
                                this.GenerationStopCriterias.Remove(criteriaVM);
                            }
                        }
                    }
                    break;
            }
        }

        private void OnRemoveStopCriteria(object obj)
        {
            var stopCriteria = obj as GenerationStopCriteriaViewModel;
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
            var stopCriteriaFactory = Resolver.Resolve<IGenerationStopCriteriaFactory>();
            var stopCriteria = stopCriteriaFactory.Create(this.CurrentSelectedAvailableGenerationStopCriteria.StopCriteria.Type);
            this.Testcase.Add(stopCriteria);  
        }

        private bool OnCanAddStopCriteria(object obj)
        {
            return true;
        }
    }
}