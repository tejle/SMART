using SMART.Gui.Providers;
using System;

namespace SMART.Gui.Models
{
    //public class RunningTestCaseModel //: DataModel
    //{
    //    private IRunningTestCaseProvider _runningTestCaseProvider;
    //    private ITestCase _testCase;
    //    private IGraphManager _graphManager;
    //    private TestCaseRunnerManager _runnerManager;

    //    public RunningTestCaseModel(IRunningTestCaseProvider runningTestCaseProvider)
    //    {
    //        _runningTestCaseProvider = runningTestCaseProvider;

    //        FetchTestCase();
    //        CreateGraphManager();
    //        CreateRunnerManager();
    //    }

    //    protected void OnActivated()
    //    {
    //        base.OnActivated();
    //    }

    //    private void FetchTestCase()
    //    {
    //        _testCase = _runningTestCaseProvider.GetTestCase();
    //    }

    //    private void CreateGraphManager()
    //    {
    //        _graphManager = _runningTestCaseProvider.GetGraphManager();
    //    }

    //    private void CreateRunnerManager()
    //    {
    //        _runnerManager = _runningTestCaseProvider.GetRunner();
    //        _runnerManager.ElementProcessed += new System.EventHandler<SMART.Core.Adapter.ElementProcessedEventArgs>(_runnerManager_ElementProcessed);
    //    }

    //    void _runnerManager_ElementProcessed(object sender, EventArgs e)
    //    {
    //        _graphManager.UpdateMergedGraph(e.Element);
    //    }

    //    public IGraph Graph { get { return _graphManager.Graph; } }

    //    public SmartGraphControl GraphControl { get { return _graphManager.GraphControl; } }

    //    public IGraphManager GraphManager { get { return _graphManager; } }

    //    public ITestCase TestCase { get { return _testCase; } }

    //    public TestCaseRunnerManager Runner { get { return _runnerManager; } }

    //    public double PercentDone { get { return _runnerManager.PercentDone; } }

    //    public DateTime ElapsedTime { get { return _runnerManager.ElapsedTime; } }
    //}
}
