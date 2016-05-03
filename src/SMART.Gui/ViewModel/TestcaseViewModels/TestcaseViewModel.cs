using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMART.Core.Interfaces;

namespace SMART.Gui.ViewModel.TestcaseViewModels
{
    using System.Collections.ObjectModel;

    public class TestcaseViewModel:ViewModelBase, ITestcaseViewModel
    {
        public ObservableCollection<ITestcaseViewModel> ViewModels
        {
            get;
            private set;
        }

        //public ITestcase Testcase { get { return testcase; } }
        private readonly ITestcase testcase;

        public TestcaseViewModel(ITestcase testcase) : base(testcase.Name)
        {
            this.testcase = testcase;

            CreateViewModels();
        }

        private void CreateViewModels()
        {
            ViewModels = new ObservableCollection<ITestcaseViewModel>
                             {
                                     new TestcaseConfigViewModel(this.testcase), 
                                     new TestcaseExecuteViewModel(this.testcase),
                                     new TestcaseModelViewModel(this.testcase)
                             };
        }

        public override string Icon
        {
            get { return Constants.TESTCASE_ICON_URL; }
        }
    }
}
