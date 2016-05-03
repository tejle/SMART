using System.Technology.BDD.Nunit;
using Moq;
using NUnit.Framework;
using SMART.Core;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Services;
using System.Collections.Generic;
using SMART.Core.Workflow;

namespace SMART.Test.Core
{
	[TestFixture]
	public class Testcase_executing_a_sequence : ContextSpecification
	{
		Model model;
		List<Queue<IStep>> sequence;
		TestExecutionEngine engine;
		Mock<IAdapter> adapter;
		Mock<IModelElement> modelElement;
		Mock<IModel> exeModel = new Mock<IModel>();
		Mock<IEventService> eventService;
		Mock<ITestcase> testcase = new Mock<ITestcase>();
		Mock<IExecutionStopCriteria> executionStopCriteria = new Mock<IExecutionStopCriteria>();

		public override void Given()
		{
			CreateModel();
			modelElement = new Mock<IModelElement>();
			sequence = GetSequence();

			adapter = new Mock<IAdapter>();
			adapter.Setup(a => a.Execute(It.IsAny<string>(), It.IsAny<string[]>()))
				.Returns(true);

			exeModel = new Mock<IModel>();
			exeModel.SetupGet(e => e.Elements).Returns(new List<IModelElement>(model.Elements)).Verifiable();

			eventService = new Mock<IEventService>();
			eventService.Setup(e => e.GetEvent<TestExecutionDefectDetected>()).Returns(new TestExecutionDefectDetected());

			testcase.SetupGet(t => t.Adapters)
				.Returns(new List<IAdapter> { adapter.Object });
			testcase.SetupGet(t => t.ExecutionStopCriteriasas).
				Returns(new List<IExecutionStopCriteria> { executionStopCriteria.Object });

			executionStopCriteria.Setup(e => e.ShouldStop(It.IsAny<IModel>())).Returns(true);
			executionStopCriteria.Setup(e => e.Init()).Verifiable();
			engine = new TestExecutionEngine(eventService.Object);
		}

		public override void When()
		{
			// when execute is called
			engine.Execute(sequence, testcase.Object, exeModel.Object);
		}


		[Test]
		public void then_the_execution_should_stop_when_the_stopcriteria_is_fullfilled()
		{
			executionStopCriteria.Verify(e => e.ShouldStop(It.IsAny<IModel>()), Times.Exactly(1));
			executionStopCriteria.Verify(e => e.Init(), Times.Exactly(1));
		}







		private void CreateModel()
		{
			model = new Model("TestModel");
			model
				.Add(new State("StateA"))
				.Add(new State("StateB"))
				.Add(new State("StateC"))
				.Add(new Transition("StartToA")
				{
					Source = model.StartState,
					Destination = model["StateA"] as State,
					Parameter = "param1"
				})
				.Add(new Transition("AToB")
				{
					Source = model["StateA"] as State,
					Destination = model["StateB"] as State
				})
				.Add(new Transition("BToC")
				{
					Source = model["StateB"] as State,
					Destination = model["StateC"] as State
				})
				.Add(new Transition("CToStop")
				{
					Source = model["StateC"] as State,
					Destination = model.StopState
				});
		}

		private List<Queue<IStep>> GetSequence()
		{
		    var list = new List<Queue<IStep>>();
			var q = new Queue<IStep>();
			q.Enqueue(new BasicStep(model["StartToA"]));
			q.Enqueue(new BasicStep(model["StateA"]));
			q.Enqueue(new BasicStep(model["AToB"]));
			q.Enqueue(new BasicStep(model["StateB"]));
			q.Enqueue(new BasicStep(model["BToC"]));
			q.Enqueue(new BasicStep(model["StateC"]));
			q.Enqueue(new BasicStep(model["CToStop"]));
            list.Add(q);
			return list;
		}

	}
}