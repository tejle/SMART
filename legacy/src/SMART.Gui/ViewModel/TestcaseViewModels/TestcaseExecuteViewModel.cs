using SMART.Core.Interfaces;

namespace SMART.Gui.ViewModel.TestcaseViewModels
{
    public class TestcaseExecuteViewModel : ViewModelBase, ITestcaseViewModel
    {
        private readonly ITestcase testcase;

        public TestcaseExecuteViewModel(ITestcase testcase) : base(testcase.Name)
        {
            this.testcase = testcase;
        }

        public override string Icon
        {
            get { return Constants.MISSING_ICON_URL; }
        }
    }
}
