//using System;
//using System.Collections.Generic;
//using System.Technology.BDD.Nunit;
//using Moq;
//using NUnit.Framework;
//using SMART.Core.DomainModel;
//using SMART.Core.Interfaces;
//using SMART.Core;
//using SMART.Core.Interfaces.Services;
//using SMART.Core.Workflow;

//namespace SMART.Test.Core {
//    [TestFixture]
//    public class Testcase_execution_of_one_step : ContextSpecification {
//        Model model;
//        List<Queue<IStep>> sequence;
//        TestExecutionEngine engine;
//        Mock<IAdapter> adapter;
//        Mock<IModelElement> modelElement;
//        Mock<IModel> exeModel;
//        Mock<IEventService> eventService;
//        Mock<IStep> step;
//        Mock<ITestcase> testcase;
        
//        public override void Given()
//        {
//            testcase = new Mock<ITestcase>();
//            modelElement = new Mock<IModelElement>();
//            step = new Mock<IStep>();
//            sequence = new List<Queue<IStep>>();
//            var q = new Queue<IStep>();
//            q.Enqueue(step.Object);
//            sequence.Add(q);
//            adapter = new Mock<IAdapter>();
//            exeModel = new Mock<IModel>();
//            eventService = new Mock<IEventService>();
            
//            CreateModel();
            
//            step
//                .SetupGet(s => s.Function)
//                .Returns("StartToA");
//            step
//                .SetupGet(s => s.Parameters)
//                .Returns(new[]{ "param1"});
//            step
//                .SetupGet(s => s.ModelElement)
//                .Returns(modelElement.Object);

//            adapter
//                .Setup(a => a.Execute("StartToA", new string[] { "param1" }))
//                .Returns(true)
//                // should not really raise on true... =)
//                .Raises(a=>a.DefectDetected+=null, new DefectEventArgs(modelElement.Object))
//                .Verifiable();

//            exeModel
//                .SetupGet(e=> e.Elements)
//                .Returns(new List<IModelElement>(){modelElement.Object})
//                .Verifiable();

//            eventService
//                .Setup(e => e.GetEvent<TestExecutionDefectDetected>())
//                .Returns(new TestExecutionDefectDetected());

//            testcase
//                .SetupGet(t => t.Adapters)
//                .Returns(new List<IAdapter> {adapter.Object});
            
//            engine = new TestExecutionEngine(eventService.Object);
//        }

//        public override void When() {
//            // when execute is called
//            engine.Execute(sequence, testcase.Object, exeModel.Object);
//        }
//        // then the apdater is called with the correct function name and parameters
//        // then statistics are updated for the modelitem
//        // then adapter events are listened to
//        // then times are meassured for the execution of the step

//        [Test]
//        public void then_the_adpater_is_called_with_the_correct_function_name_and_parameters()
//        {
//            adapter.Verify(a=>a.Execute("StartToA", new string[]{"param1"}));
//        }

//        [Test]
//        public void the_visit_is_called_on_the_model_item()
//        {
//            exeModel.Verify(e=>e.Elements, Times.Exactly(1));
//            modelElement.Verify(m=>m.Visit(), Times.Exactly(1));
//        }

//        [Test]
//        public void the_adapter_events_are_listened_to()
//        {
//            eventService.Verify(e=>e.GetEvent<TestExecutionDefectDetected>());
//        }


//        private void CreateModel()
//        {
//            model = new Model("TestModel");
//            model
//                .Add(new State("StateA"))
//                .Add(new State("StateB"))
//                .Add(new State("StateC"))
//                .Add(new Transition("StartToA") { Source = model.StartState, Destination = model["StateA"] as State, Parameter = "param1" })
//                .Add(new Transition("AToB") { Source = model["StateA"] as State, Destination = model["StateB"] as State })
//                .Add(new Transition("BToC") { Source = model["StateB"] as State, Destination = model["StateC"] as State })
//                .Add(new Transition("CToStop") { Source = model["StateC"] as State, Destination = model.StopState });
//        }

      
//    }
//}