using SMART.Core.Events;
using SMART.Core.Interfaces;

namespace SMART.Gui.ViewModel.ProjectExplorer
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Commands;

    using IOC;

    public class RootTestcaseFolderViewModel : ViewModelBase, IFolderViewModel
    {
        private readonly IProject project;
        public ObservableCollection<TestcaseFolderViewModel> FolderViewModels { get; set; }
        public RoutedActionCommand AddTestcase
        {
            get;
            private set;
        }
       
        public RootTestcaseFolderViewModel(IProject project)
                : base("Testcases")
        {
            this.project = project;
            
            project.PropertyChanged += this.Project_PropertyChanged;
            this.CreateTestcaseFolders();
            this.CreateCommand();
        }

        private void CreateCommand()
        {
            this.AddTestcase = new RoutedActionCommand("AddTestcase", typeof(RootTestcaseFolderViewModel))
                              {
                                      Description = "Add a new testcase", 
                                      OnCanExecute = (o) => true,
                                      OnExecute = this.OnAddTestcase,
                                      Text = "Add testcase",
                                      Icon = Constants.TESTCASE_ADD_ICON_URL
                              };
        }

        protected virtual void OnAddTestcase(object obj)
        {
            var testcase = Resolver.Resolve<ITestcase>();
            testcase.Name = "new testcase";
            this.project.AddTestCase(testcase);
        }

        void Project_PropertyChanged(object sender, SmartPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Testcases")
            {
                var testcase = e.Item as ITestcase;
                if (testcase == null)
                    return;

                switch (e.Action)
                {
                    case SmartPropertyChangedAction.Add:
                    {
                        var t = new TestcaseFolderViewModel(testcase, this.project);
                        
                        
                        this.FolderViewModels.Add(t);
                        break;
                    }
                    case SmartPropertyChangedAction.Remove:
                    {
                        var testcaseviewmodel = this.FolderViewModels.Where(vm => vm.TestcaseId == testcase.Id);
                        if(testcaseviewmodel.Count() == 0) return;
                        
                        this.FolderViewModels.Remove(testcaseviewmodel.First());
                        break;
                    }
                    default:
                        break;
                }

            }
        }

        private void CreateTestcaseFolders()
        {

            var list = from t in this.project.Testcases select new TestcaseFolderViewModel(t, this.project);
            
            this.FolderViewModels = new ObservableCollection<TestcaseFolderViewModel>();
            
            foreach(var t in list)
            {
                this.FolderViewModels.Add(t);
                
            }
        }

        public override string Icon
        {
            get { return Constants.OPEN_ICON_URL; }
        }

        public override Guid Id
        {
            get;
            set;
        }
    }
}