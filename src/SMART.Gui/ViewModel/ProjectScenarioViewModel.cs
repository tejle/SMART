using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using SMART.Core.DomainModel;
using SMART.Core.Events;
using SMART.Core.Interfaces;
using SMART.Gui.Commands;
using SMART.IOC;

namespace SMART.Gui.ViewModel
{
    public interface IEditableViewModel
    {
        bool IsEditMode { get; set; }
        bool StartInEditMode { get; set; }
    }

    public class ProjectScenarioViewModel : ViewModelBase, IEditableViewModel
    {
        private readonly IProject project;
        private readonly ITestcase scenario;
        private bool isEditMode = false;

        public RoutedActionCommand Open { get; private set; }
        public RoutedActionCommand Remove { get; private set; }
        public RoutedActionCommand Rename { get; private set; }
        public RoutedActionCommand AddModel { get; private set; }

        public bool IsEditMode
        {
            get
            {
                return isEditMode;
            }
            set
            {
                isEditMode = value;
                StartInEditMode = false; 
                SendPropertyChanged("IsEditMode");

            }
        }

        public bool StartInEditMode
        {
            get; set;
        }

        public override string Icon
        {
            get { return Constants.TESTCASE_ICON_URL; }
        }
        public override string Name
        {
            get
            {
                return base.Name;
            }
            set
            {
                if (!base.Name.Equals(value))
                    base.Name = value;
                if (!scenario.Name.Equals(value))
                    scenario.Name = value;
            }
        }
        public override Guid Id
        {
            get { return scenario.Id; }
            set { scenario.Id = value; SendPropertyChanged("Id"); }
        }

        public ObservableCollection<ProjectModelViewModel> Models
        {
            get;
            private set;
        }

        public ITestcase Scenario
        {
            get
            {
                return scenario;
            }
        }

        public ProjectScenarioViewModel(IProject project, ITestcase testcase)
            : base(testcase.Name)
        {
            this.project = project;
            scenario = testcase;
            scenario.PropertyChanged += scenario_PropertyChanged;
            scenario.CollectionChanged += scenario_CollectionChanged;
            CreateCollection();
            CreateCommands();
        }

        void scenario_CollectionChanged(object sender, SmartNotifyCollectionChangedEventArgs e)
        {
            if (e.CollectionName.Equals("Models"))
            {
                if (e.Action == SmartNotifyCollectionChangedAction.Add)
                {
                    foreach (var model in e.NewItems.OfType<IModel>())
                    {
                        if (model == null)
                            return;

                        var g = new ProjectModelViewModel(model, project, scenario);

                        Models.Add(g);

                    }
                }
                else if (e.Action == SmartNotifyCollectionChangedAction.Remove)
                {
                    foreach (var model in e.OldItems.OfType<IModel>())
                    {
                        var modelViewModel = Models.Where(gvw => gvw.Id == model.Id);
                        if (modelViewModel.Count() == 0)
                            return;

                        Models.Remove(modelViewModel.First());
                    }
                }
                else if (e.Action == SmartNotifyCollectionChangedAction.Move)
                {
                    throw new ArgumentException("Supported method???");
                }
                else if (e.Action == SmartNotifyCollectionChangedAction.Replace)
                {
                    throw new ArgumentException("Supported method???");

                }
                else if (e.Action == SmartNotifyCollectionChangedAction.Reset)
                {
                    throw new ArgumentException("Supported method???");

                }

            }
        }

        void scenario_PropertyChanged(object sender, SmartPropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Name"))
            {
                Name = scenario.Name;
            }
        }

        private void CreateCommands()
        {
            this.Open = new RoutedActionCommand("Open", typeof(ProjectScenarioViewModel))
            {
                Description = "Open scenario",
                OnCanExecute = (o) => true,
                OnExecute = this.OnOpen,
                Text = "Open",
                Icon = Constants.OPEN_ICON_URL
            };
            this.Remove = new RoutedActionCommand("Remove", typeof(ProjectScenarioViewModel))
            {
                Description = "Remove scenario",
                OnCanExecute = (o) => true,
                OnExecute = this.OnRemove,
                Text = "Remove",
                Icon = Constants.DELETE_ICON_URL
            };
            this.Rename = new RoutedActionCommand("Rename", typeof(ProjectScenarioViewModel))
            {
                Description = "Rename scenario",
                OnCanExecute = (o) => true,
                OnExecute = this.OnRename,
                Text = "Rename",
                Icon = Constants.RENAME_ICON_URL
            };
            this.AddModel = new RoutedActionCommand("AddModel", typeof(ProjectScenarioViewModel))
            {
                Description = "Add model to scenario",
                OnCanExecute = (o) => true,
                OnExecute = this.OnAddModel,
                Text = "Add Model",
                Icon = Constants.MODEL_ADD_ICON_URL
            };

        }

        public RoutedEventHandler LostFocus = (s, e) => { MessageBox.Show("weeee"); };
        public RoutedEventHandler PreviewKeyUp = (s,e) => {};
        private void OnAddModel(object obj)
        {
            var model = Resolver.Resolve<Model>();
            model.Name = "new model";
            project.AddModel(model, scenario);

        }

        public void AddExistingModel(Guid id)
        {
            var model = (from m in project.Models where m.Id.Equals(id) select m).FirstOrDefault();
            if (model != null)
            {
                scenario.Add(model);
            }
        }

        private void OnRename(object obj)
        {
            IsEditMode = true;
        }

        private void OnRemove(object obj)
        {
            project.RemoveTestCase(scenario);
        }

        private void OnOpen(object obj)
        {

        }

        private void CreateCollection()
        {
            var list = from g in scenario.Models select new ProjectModelViewModel(g as Model, project, scenario);

            Models = new ObservableCollection<ProjectModelViewModel>();
            foreach (var t in list)
            {
                Models.Add(t);
            }
        }
    }
}