//using System;
//using System.Collections.Generic;
//using System.Linq;
//using NUnit.Framework;
//using Rhino.Mocks;
//using Rhino.Mocks.Interfaces;
//using SMART.Core;
//using SMART.Core.BusinessLayer;
//using SMART.Core.DomainModel;
//using SMART.Core.Interfaces;

//namespace SMART.Test.Core
//{
//    [TestFixture]
//    public class TestcaseUnitTests
//    {
//        FakeTestCaseExcecutor testcaseExecutor;
//        ISandbox sandbox;
//        IStatisticsManager statisticsManager;
//        ModelBuilder modelBuilder;
//        ITestcaseExecutor mockedTestcaseExecutor;
            
//        [SetUp]
//        public void setup()
//        {
            
//            testcaseExecutor = new FakeTestCaseExcecutor();
//            mockedTestcaseExecutor = MockRepository.GenerateMock<ITestcaseExecutor>();
//            sandbox = MockRepository.GenerateStub<ISandbox>();
//            statisticsManager = MockRepository.GenerateMock<IStatisticsManager>();
//            modelBuilder = MockRepository.GenerateMock<ModelBuilder>();
     
//        }

//        [Test]
//        public void on_contruction_verify_event_hook_up_and_call_to_statistics_manager()
//        {
//            var tc = new Testcase(testcaseExecutor, modelBuilder, statisticsManager);
//            tc.Models = new List<Model> {new Model()};
//            statisticsManager.AssertWasCalled(s => s.ModelGetter = Arg<Func<Model>>.Is.NotNull);
            
//            Assert.IsNotNull(tc);
//        }

//        [Test]
//        public void testcaseComplete_is_called_correctly()
//        {
//            var tc = new Testcase(testcaseExecutor, modelBuilder, statisticsManager);
            
//            var b = false;

//            tc.TestCaseExecutionComplete += (sender, e) => b = true;
//            testcaseExecutor.FireEvent();
//            Assert.IsTrue(b);
//        }

//        [Test]
//        public void start_calls_executor_correctly()
//        {
//            modelBuilder.Stub(g=>
//                              g.Merge(Arg<IEnumerable<Model>>.Is.Anything,Arg<Model>.Is.Anything)).IgnoreArguments()
//                .Return(new Model(
//                            new List<Transition>(),
//                            new List<State>(), statisticsManager,sandbox));

//            mockedTestcaseExecutor.Expect(f=> f.Start(Arg<Model>.Is.Anything,Arg<IEnumerable<Tuple<IAlgorithm,IStopCriteria>>>.Is.Anything));
//            var tc = new Testcase(mockedTestcaseExecutor, modelBuilder, statisticsManager);
//            tc.StartExecution();

//            mockedTestcaseExecutor.VerifyAllExpectations();
            
//            modelBuilder.VerifyAllExpectations();
//        }

//        [Test]
//        public void stop_calls_executor_correctly()
//        {
//            modelBuilder.Stub(g =>
//                              g.Merge(Arg<IEnumerable<Model>>.Is.Anything, Arg<Model>.Is.Anything)).IgnoreArguments()
//                .Return(new Model(
//                            new List<Transition>(),
//                            new List<State>(), statisticsManager, sandbox));

//            mockedTestcaseExecutor.Expect(f => f.Stop());
//            var tc = new Testcase(mockedTestcaseExecutor, modelBuilder, statisticsManager);
//            tc.StopExecution();

//            mockedTestcaseExecutor.VerifyAllExpectations();

//            modelBuilder.VerifyAllExpectations();
//        }

//        [Test]
//        public void pause_calls_executor_correctly()
//        {
//            modelBuilder.Stub(g =>
//                              g.Merge(Arg<IEnumerable<Model>>.Is.Anything, Arg<Model>.Is.Anything)).IgnoreArguments()
//                .Return(new Model(
//                            new List<Transition>(),
//                            new List<State>(), statisticsManager, sandbox));

//            mockedTestcaseExecutor.Expect(f => f.Pause());
//            var tc = new Testcase(mockedTestcaseExecutor, modelBuilder, statisticsManager);
//            tc.PauseExecution();

//            mockedTestcaseExecutor.VerifyAllExpectations();

//            modelBuilder.VerifyAllExpectations();
//        }
//        [Test]
//        public void resume_calls_executor_correctly()
//        {
//            modelBuilder.Stub(g =>
//                              g.Merge(Arg<IEnumerable<Model>>.Is.Anything, Arg<Model>.Is.Anything)).IgnoreArguments()
//                .Return(new Model(
//                            new List<Transition>(),
//                            new List<State>(), statisticsManager, sandbox));

//            mockedTestcaseExecutor.Expect(f => f.Resume());
//            var tc = new Testcase(mockedTestcaseExecutor, modelBuilder, statisticsManager);
//            tc.ResumeExecution();

//            mockedTestcaseExecutor.VerifyAllExpectations();

//            modelBuilder.VerifyAllExpectations();
//        }

//        [Test]
//        public void restart_calls_executor_correctly()
//        {
//            modelBuilder.Stub(g =>
//                              g.Merge(Arg<IEnumerable<Model>>.Is.Anything, Arg<Model>.Is.Anything)).IgnoreArguments()
//                .Return(new Model(
//                            new List<Transition>(),
//                            new List<State>(), statisticsManager, sandbox));

//            mockedTestcaseExecutor.Expect(f => f.Start(Arg<Model>.Is.Anything, Arg<IEnumerable<Tuple<IAlgorithm, IStopCriteria>>>.Is.Anything));
//            mockedTestcaseExecutor.Expect(f => f.Stop());
            
//            var tc = new Testcase(mockedTestcaseExecutor, modelBuilder, statisticsManager);
//            tc.RestartExecution();

//            mockedTestcaseExecutor.VerifyAllExpectations();

//            modelBuilder.VerifyAllExpectations();
//        }
//        [Test]
//        public void add_a_new_model_should_return_true_and_count_should_be_one()
//        {
//            var testcase = new Testcase(testcaseExecutor, modelBuilder, statisticsManager);
//            var success = testcase.AddModel(new Model());
//            Assert.IsTrue(success);
//            Assert.AreEqual(1, testcase.Models.Count());
//        }

//        [Test]
//        public void add_model_and_remove_it_should_return_true_and_count_be_zero()
//        {
//            var testcase = new Testcase(testcaseExecutor, modelBuilder, statisticsManager);
//            var model = new Model();

//            testcase.AddModel(model);
//            var success = testcase.RemoveModel(model);
//            Assert.IsTrue(success);
//            Assert.AreEqual(0, testcase.Models.Count());
            
//        }

//        [Test]
//        public void adding_a_list_of_models_should_pick_the_first_one_to_be_the_startupmodel()
//        {
//            var testcase = new Testcase(testcaseExecutor, modelBuilder, statisticsManager);
//            var model1 = new Model();
//            var model2 = new Model();

//            testcase.Models = new List<Model>() {model1, model2};
//            Assert.AreSame(model1, testcase.StartUpModel);
//        }

//        [Test]
//        public void adding_the_same_model_twice_should_return_false_and_count_be_one()
//        {
//            var testcase = new Testcase(testcaseExecutor, modelBuilder, statisticsManager);
//            var model = new Model();

//            testcase.AddModel(model);
//            var success = testcase.AddModel(model);
            

//            Assert.IsFalse(success);
//            Assert.AreEqual(1, testcase.Models.Count());
            
//        }

//        [Test]
//        public void removing_a_model_that_doesnt_exist_should_return_false()
//        {
//            var testcase = new Testcase(testcaseExecutor, modelBuilder, statisticsManager);

//            Assert.IsFalse(testcase.RemoveModel(new Model()));
//        }

//        [Test]
//        public void adding_adapters_should_hookup_event_which_in_turn_should_call_StatisticsManager_DefectDetected()
//        {
//            var testcase = new Testcase(testcaseExecutor, modelBuilder, statisticsManager);

//            statisticsManager.Expect(s => s.DefectDetected(Arg<ModelElement>.Is.Anything)).IgnoreArguments();

//            var adapter = new FakeAdapter();
//            testcase.Adapters = new List<IAdapter> {adapter};
          
//            adapter.FireEvent(null);
//            statisticsManager.VerifyAllExpectations();
//        }

//        [Test]
//        public void adapters_should_not_be_null()
//        {
//            var testcase = new Testcase(testcaseExecutor, modelBuilder, statisticsManager);
//            Assert.IsNotNull(testcase.Adapters);            
//        }

//        [Test]
//        public void DelayBetweenExectionSteps_should_relay_to_executor()
//        {
//            var executor = MockRepository.GenerateMock<ITestcaseExecutor>();
//            executor.Expect(e => e.DelayBetweenExecutionSteps).Return(500);

//            var testcase = new Testcase(executor, modelBuilder, statisticsManager);
//            testcase.DelayBetweenExecutionSteps = 1000;
//            executor.AssertWasCalled(e=> e.DelayBetweenExecutionSteps =1000);
//            Assert.AreEqual(500, testcase.DelayBetweenExecutionSteps);
               
//        }
//    }

//    public class FakeAdapter: IAdapter
//    {
//        public void Execute(string function, params string[] args)
//        {
//        }

//        public event EventHandler<DefectEventArgs> DefectDetected;
//        public void PreExecution()
//        {
//        }

//        public void PostExection()
//        {
//        }

//        public void FireEvent(ModelElement element)
//        {
//            DefectDetected(this, new DefectEventArgs(element));
//        }
//    }

//    public class FakeTestCaseExcecutor: TestcaseExecutor
//    {
//        public override event EventHandler TestCaseComplete;
//        public void FireEvent()
//        {
//            TestCaseComplete(null, null);

//        }
//    }
//}