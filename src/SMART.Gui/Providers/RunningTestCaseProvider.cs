using System;

namespace SMART.Gui.Providers
{
    //public class RunningTestCaseProvider : IRunningTestCaseProvider
    //{
    //    private IGraphManager _graphManager;
    //    private ITestCase _testCase;
    //    private TestCaseRunnerManager _runner;

    //    public ITestCase GetTestCase()
    //    {
    //        if (_testCase == null)
    //        {
    //            _testCase = App.CurrentProject.TestCases.Find(tc => tc.ID == _id);
    //        }
    //        return _testCase;
    //    }

    //    public IGraphManager GetGraphManager()
    //    {
    //        if (_graphManager == null)
    //        {
    //            GetTestCase().GraphCollection.Merge();
    //            _graphManager = new GraphManager(GetTestCase().GraphCollection, true);
    //        }
    //        return _graphManager;
    //    }

    //    public TestCaseRunnerManager GetRunner()
    //    {
    //        if (_runner == null)
    //        {
    //            _runner = new TestCaseRunnerManager(GetTestCase());
    //        }
    //        return _runner;

    //        return null;
    //    }

    //    private Guid _id;

    //    public RunningTestCaseProvider(Guid id)
    //    {
    //        _id = id;
    //    }

    //}
}
