using SMART.Gui.Providers;

namespace SMART.Gui.Models
{
    public class TestCaseModel //: DataModel
    {
        private ITestCaseProvider _testCaseProvider;
//        private ITestCase _testCase;

        public TestCaseModel(ITestCaseProvider testCaseProvider)
        {
            _testCaseProvider = testCaseProvider;
        }

        //protected override void OnActivated()
        //{
        //    base.OnActivated();

        //    FetchTestCase();
        //}

        private void FetchTestCase()
        {
           // _testCase = _testCaseProvider.GetTestCase();
        }

        public string Name { get;set;}// { return _testCase.Name; } set { _testCase.Name = value; } }
    }
}
