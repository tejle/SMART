using System.Collections.Generic;
using System.Technology.BDD.Nunit;
using NUnit.Framework;
using SMART.Core;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

namespace SMART.Test.Core.ModelMerge
{
	[TestFixture]
	public class ModelCompiler_merge_two_models_where_one_has_two_references : ContextSpecification
	{
		private Model modelA = null;
		private Model modelB = null;
		private IModel expected = null;
		SimpleModelCompiler compiler;

		public override void Given()
		{
			createModels();
			compiler = new SimpleModelCompiler();
		}

		public override void When()
		{
			expected = compiler.Compile(new List<IModel> {modelA, modelB});
		}

		[TearDown]
		public void teardown()
		{
			compiler = null;
			expected = null;
		}

		[Test]
		[Ignore]
		public void result_should_have_eight_states()
		{
			expected.States.Count.should_be_equal_to(8);

		}

		[Test]
		[Ignore]
		public void result_should_have_seven_transitions()
		{
			expected.Transitions.Count.should_be_equal_to(7);

		}

		private void createModels()
		{
			var b1 = new State("B") {Type = StateType.GlobalReference};
			var b2 = new State("B") {Type = StateType.GlobalReference};
			modelA = new Model("A");
			modelA.Add(new State("First"))
				.Add(b1)
				.Add(b2)
				.Add(new State("Second"))
				.Add(new Transition("StartToFirst")
				     	{
				     		Source = modelA.StartState,
				     		Destination = modelA["First"] as State
				     	})
				.Add(new Transition("FirstToB1")
				     	{
				     		Source = modelA["First"] as State,
				     		Destination = b1
				     	})
				.Add(new Transition("B1ToB2")
				     	{
				     		Source = b1,
				     		Destination = b2
				     	})
				.Add(new Transition("B2ToSecond")
				     	{
				     		Source = b2,
				     		Destination = modelA["Second"] as State
				     	})
				.Add(new Transition("SecondToStop")
				     	{
				     		Source = modelA["Second"] as State,
				     		Destination = modelA.StopState
				     	});

			modelB = new Model("B");
			modelB.Add(new State("Third"))
				.Add(new State("Forth"))
				.Add(new Transition("StartToThird")
				     	{
				     		Source = modelB.StartState,
				     		Destination = modelB["Third"] as State
				     	})

				.Add(new Transition("ThirdToForth")
				     	{
				     		Source = modelB["Third"] as State,
				     		Destination = modelB["Forth"] as State
				     	})
				.Add(new Transition("ForthToStop")
				     	{
				     		Source = modelB["Forth"] as State,
				     		Destination = modelB.StopState
				     	});
		}
	}
}