using System.Collections.Generic;
using System.Technology.BDD.Nunit;
using NUnit.Framework;
using SMART.Core.DomainModel;
using SMART.Core;
using SMART.Core.Interfaces;

namespace SMART.Test.Core.ModelMerge
{
	[TestFixture]
	public class ModelCompiler_merge_two_models : ContextSpecification
	{
		private Model modelA;
		private Model modelB;
		private Model expectedResult;
		IModel actualResult;

		private SimpleModelCompiler compiler;
		public override void Given()
		{
			createModels();
			compiler = new SimpleModelCompiler();
		}

		public override void When()
		{
			actualResult = compiler.Compile(new List<IModel> {modelA, modelB});
		}

		[Test]
		public void transitions_should_equal_expected()
		{
			actualResult.Transitions.Count.should_be_equal_to(expectedResult.Transitions.Count);
		}

		[Test]
		public void states_should_equal_expected()
		{
			actualResult.States.Count.should_be_equal_to(expectedResult.States.Count);
		}

		private void createModels()
		{
			modelA = new Model("A");
			modelA
				.Add(new State("First"))
				.Add(new State("Second"))
				.Add(new State("B"){Type = StateType.GlobalReference})
				.Add(new State("Third"))
				.Add(new Transition("StartToFirst")
				     	{
				     		Source = modelA.StartState,
				     		Destination = modelA["First"] as State
				     	})
				.Add(new Transition("FristToSecond")
				     	{
				     		Source = modelA["First"] as State,
				     		Destination = modelA["Second"] as State
				     	})
				.Add(new Transition("SecondToB")
				     	{
				     		Source = modelA["Second"] as State,
				     		Destination = modelA["B"] as State
				     	})
				.Add(new Transition("BToThird")
				     	{
				     		Source = modelA["B"] as State,
				     		Destination = modelA["Third"] as State
				     	})
				.Add(new Transition("ThirdToStop")
				     	{
				     		Source = modelA["Third"] as State,
				     		Destination = modelA.StopState

				     	});

			modelB = new Model("B");
			modelB.Add(new State("Forth"))
				.Add(new State("Fifth"))
				.Add(new Transition("StartToForth")
				     	{
				     		Source = modelB.StartState,
				     		Destination = modelB["Forth"] as State
				     	})
				.Add(new Transition("ForthToFifth")
				     	{
				     		Source = modelB["Forth"] as State,
				     		Destination = modelB["Fifth"] as State

				     	})
				.Add(new Transition("FifthToStop")
				     	{
				     		Source = modelB["Fifth"] as State,
				     		Destination = modelB.StopState

				     	});

			expectedResult = new Model("Merged");

			var start = new Transition
			            	{
			            		Label = (modelA["StartToFirst"] as Transition).Label,
			            		Id = (modelA["StartToFirst"] as Transition).Id,
			            		Source = expectedResult.StartState,
			            		Destination = (modelA["StartToFirst"] as Transition).Destination
			            	};

			var stop = new Transition
			           	{
			           		Label = (modelA["ThirdToStop"] as Transition).Label,
			           		Id = (modelA["ThirdToStop"] as Transition).Id,
			           		Source = (modelA["ThirdToStop"] as Transition).Source,
			           		Destination = expectedResult.StopState
			           	};

			var stbTran = new Transition
			              	{
			              		Label = (modelA["SecondToB"] as Transition).Label,
			              		Id = (modelA["SecondToB"] as Transition).Id,
			              		Source = (modelA["SecondToB"] as Transition).Source,
			              		Destination = (modelB["Forth"] as State)
			              	};

			var bttTran =new Transition
			             	{
			             		Label = (modelA["BToThird"] as Transition).Label,
			             		Id = (modelA["BToThird"] as Transition).Id,
			             		Destination = (modelA["BToThird"] as Transition).Destination,
			             		Source = (modelB["Fifth"] as State)
			             	};


			expectedResult
				.Add(modelA["First"] as State)
				.Add(modelA["Second"] as State)
				.Add(modelA["Third"] as State)
				.Add(modelB["Forth"] as State)
				.Add(modelB["Fifth"] as State);
			expectedResult
				.Add(start)
				.Add(modelA["FristToSecond"] as Transition)
				.Add(stbTran)
				.Add(bttTran)
				.Add(stop)
				.Add(modelB["ForthToFifth"] as Transition);


		}
	}
}