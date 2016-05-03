using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using SMART.Core.BusinessLayer;
using SMART.Core.Model.Graph;
using SMART.Core.Model.ProjectStructure;
using SMART.Core.Model;
using Rhino.Mocks.Impl;
using System.Collections;

namespace SMART.Core.UnitTests
{
    [TestFixture]
    public class TestcaseUnitTests
    {
        FakeTestCaseExcecutor testcaseExecutor;
        ISandbox sandbox;
        IStatisticsManager statisticsManager;
        IGraphBuilder graphBuilder;
        ITestcaseExecutor mockedTestcaseExecutor;
            
        [SetUp]
        public void setup()
        {
            
            testcaseExecutor = new FakeTestCaseExcecutor();
            mockedTestcaseExecutor = MockRepository.GenerateMock<ITestcaseExecutor>();
            sandbox = MockRepository.GenerateStub<ISandbox>();
            statisticsManager = MockRepository.GenerateMock<IStatisticsManager>();
            graphBuilder = MockRepository.GenerateMock<IGraphBuilder>();
     
        }

        [Test]
        public void on_contruction_verify_event_hook_up_and_call_to_statistics_manager()
        {
            var tc = new TestCase(testcaseExecutor, graphBuilder, statisticsManager);
            tc.Graphs = new List<IEditableGraph> {new Graph()};
            statisticsManager.AssertWasCalled(s => s.GraphGetter = Arg<Func<IExecutableGraph>>.Is.NotNull);
            
            Assert.IsNotNull(tc);
        }

        [Test]
        public void testcaseComplete_is_called_correctly()
        {
            var tc = new TestCase(testcaseExecutor, graphBuilder, statisticsManager);
            
            var b = false;

            tc.TestCaseExecutionComplete += (sender, e) => b = true;
            testcaseExecutor.FireEvent();
            Assert.IsTrue(b);
        }

        [Test]
        public void start_calls_executor_correctly()
        {
            graphBuilder.Stub(g=>
                g.Merge(Arg<IEnumerable<IEditableGraph>>.Is.Anything,Arg<IEditableGraph>.Is.Anything)).IgnoreArguments()
                .Return(new ExecutableGraph(
                    new List<Edge>(),
                    new List<Vertex>(), statisticsManager,sandbox));

           mockedTestcaseExecutor.Expect(f=> f.Start(Arg<IExecutableGraph>.Is.Anything,Arg<IEnumerable<Tuple<IAlgorithm,IStopCriteria>>>.Is.Anything));
            var tc = new TestCase(mockedTestcaseExecutor, graphBuilder, statisticsManager);
            tc.StartExecution();

            mockedTestcaseExecutor.VerifyAllExpectations();
            
            graphBuilder.VerifyAllExpectations();
        }

        [Test]
        public void stop_calls_executor_correctly()
        {
            graphBuilder.Stub(g =>
                g.Merge(Arg<IEnumerable<IEditableGraph>>.Is.Anything, Arg<IEditableGraph>.Is.Anything)).IgnoreArguments()
                .Return(new ExecutableGraph(
                    new List<Edge>(),
                    new List<Vertex>(), statisticsManager, sandbox));

            mockedTestcaseExecutor.Expect(f => f.Stop());
            var tc = new TestCase(mockedTestcaseExecutor, graphBuilder, statisticsManager);
            tc.StopExecution();

            mockedTestcaseExecutor.VerifyAllExpectations();

            graphBuilder.VerifyAllExpectations();
        }

        [Test]
        public void pause_calls_executor_correctly()
        {
            graphBuilder.Stub(g =>
                g.Merge(Arg<IEnumerable<IEditableGraph>>.Is.Anything, Arg<IEditableGraph>.Is.Anything)).IgnoreArguments()
                .Return(new ExecutableGraph(
                    new List<Edge>(),
                    new List<Vertex>(), statisticsManager, sandbox));

            mockedTestcaseExecutor.Expect(f => f.Pause());
            var tc = new TestCase(mockedTestcaseExecutor, graphBuilder, statisticsManager);
            tc.PauseExecution();

            mockedTestcaseExecutor.VerifyAllExpectations();

            graphBuilder.VerifyAllExpectations();
        }
        [Test]
        public void resume_calls_executor_correctly()
        {
            graphBuilder.Stub(g =>
                g.Merge(Arg<IEnumerable<IEditableGraph>>.Is.Anything, Arg<IEditableGraph>.Is.Anything)).IgnoreArguments()
                .Return(new ExecutableGraph(
                    new List<Edge>(),
                    new List<Vertex>(), statisticsManager, sandbox));

            mockedTestcaseExecutor.Expect(f => f.Resume());
            var tc = new TestCase(mockedTestcaseExecutor, graphBuilder, statisticsManager);
            tc.ResumeExecution();

            mockedTestcaseExecutor.VerifyAllExpectations();

            graphBuilder.VerifyAllExpectations();
        }

        [Test]
        public void restart_calls_executor_correctly()
        {
            graphBuilder.Stub(g =>
                g.Merge(Arg<IEnumerable<IEditableGraph>>.Is.Anything, Arg<IEditableGraph>.Is.Anything)).IgnoreArguments()
                .Return(new ExecutableGraph(
                    new List<Edge>(),
                    new List<Vertex>(), statisticsManager, sandbox));

            mockedTestcaseExecutor.Expect(f => f.Start(Arg<IExecutableGraph>.Is.Anything, Arg<IEnumerable<Tuple<IAlgorithm, IStopCriteria>>>.Is.Anything));
            mockedTestcaseExecutor.Expect(f => f.Stop());
            
            var tc = new TestCase(mockedTestcaseExecutor, graphBuilder, statisticsManager);
            tc.RestartExecution();

            mockedTestcaseExecutor.VerifyAllExpectations();

            graphBuilder.VerifyAllExpectations();
        }
        [Test]
        public void add_a_new_graph_should_return_true_and_count_should_be_one()
        {
            var testcase = new TestCase(testcaseExecutor, graphBuilder, statisticsManager);
            var success = testcase.AddGraph(new Graph());
            Assert.IsTrue(success);
            Assert.AreEqual(1, testcase.Graphs.Count());
        }

        [Test]
        public void add_graph_and_remove_it_should_return_true_and_count_be_zero()
        {
            var testcase = new TestCase(testcaseExecutor, graphBuilder, statisticsManager);
            var graph = new Graph();

            testcase.AddGraph(graph);
            var success = testcase.RemoveGraph(graph);
            Assert.IsTrue(success);
            Assert.AreEqual(0, testcase.Graphs.Count());
            
        }

        [Test]
        public void adding_a_list_of_graphs_should_pick_the_first_one_to_be_the_startupgraph()
        {
            var testcase = new TestCase(testcaseExecutor, graphBuilder, statisticsManager);
            var graph1 = new Graph();
            var graph2 = new Graph();

            testcase.Graphs = new List<IEditableGraph>() {graph1, graph2};
            Assert.AreSame(graph1, testcase.StartUpGraph);
        }

        [Test]
        public void adding_the_same_graph_twice_should_return_false_and_count_be_one()
        {
            var testcase = new TestCase(testcaseExecutor, graphBuilder, statisticsManager);
            var graph = new Graph();

            testcase.AddGraph(graph);
            var success = testcase.AddGraph(graph);
            

            Assert.IsFalse(success);
            Assert.AreEqual(1, testcase.Graphs.Count());
            
        }

        [Test]
        public void removing_a_graph_that_doesnt_exist_should_return_false()
        {
            var testcase = new TestCase(testcaseExecutor, graphBuilder, statisticsManager);

            Assert.IsFalse(testcase.RemoveGraph(new Graph()));
        }

        [Test]
        public void adding_adapters_should_hookup_event_which_in_turn_should_call_StatisticsManager_DefectDetected()
        {
            var testcase = new TestCase(testcaseExecutor, graphBuilder, statisticsManager);

            statisticsManager.Expect(s => s.DefectDetected(Arg<IGraphElement>.Is.Anything)).IgnoreArguments();

            var adapter = new FakeAdapter();
            testcase.Adapters = new List<IAdapter> {adapter};
          
            adapter.FireEvent(null);
            statisticsManager.VerifyAllExpectations();
        }

        [Test]
        public void adapters_should_not_be_null()
        {
            var testcase = new TestCase(testcaseExecutor, graphBuilder, statisticsManager);
            Assert.IsNotNull(testcase.Adapters);            
        }

        [Test]
        public void DelayBetweenExectionSteps_should_relay_to_executor()
        {
            var executor = MockRepository.GenerateMock<ITestcaseExecutor>();
            executor.Expect(e => e.DelayBetweenExecutionSteps).Return(500);

            var testcase = new TestCase(executor, graphBuilder, statisticsManager);
            testcase.DelayBetweenExecutionSteps = 1000;
            executor.AssertWasCalled(e=> e.DelayBetweenExecutionSteps =1000);
            Assert.AreEqual(500, testcase.DelayBetweenExecutionSteps);
               
        }
    }

    public class FakeAdapter: IAdapter
    {
        public void Execute(string function, params string[] args)
        {
        }

        public event EventHandler<DefectEventArgs> DefectDetected;
        public void PreExecution()
        {
        }

        public void PostExection()
        {
        }

        public void FireEvent(IGraphElement element)
        {
            DefectDetected(this, new DefectEventArgs(element));
        }
    }

    public class FakeTestCaseExcecutor: TestcaseExecutor
    {
        public override event EventHandler TestCaseComplete;
        public void FireEvent()
        {
            TestCaseComplete(null, null);

        }
    }
}