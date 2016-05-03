using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

using Rhino.Mocks;
using SMART.Core.BusinessLayer;

namespace SMART.Test.Core
{
    [TestFixture]
    public class ExecutableModelTests
    {
        [Test]
        [Ignore]
        public void create_step_should_return_a_correctly_formed_step()
        {

            //var repository = new MockRepository();
            //var statisticsManager = repository.StrictMock<IStatisticsManager>();
            //var sandbox = repository.StrictMock<ISandbox>();

            //var v1 = new State();
            //var v2 = new State();
            //var transition = new Transition();
            //transition.Label = "label";
            //transition.Parameter = "p1;p2";
            //transition.Source = v1;
            //transition.Destination = v2;

            //var model = new Model(new List<Transition> {transition},
            //                                new List<State> {v1, v2}, statisticsManager, sandbox);

            //var step = model.CreateStep(transition);
            //Assert.AreEqual("label",step.Function);
            //Assert.AreEqual("p1", step.Parameters[0]);
            //Assert.AreEqual("p2", step.Parameters[1]);
            //Assert.AreSame(transition, step.ModelElement);
        }

        [Test]
        [Ignore]
        public void when_retrieving_out_transitions_sandbox_should_be_called_to_verify_possible_transitions()
        {
            //var repository = new MockRepository();
            //var statisticsManager = repository.StrictMock<IStatisticsManager>();
            //var sandbox = repository.StrictMock<ISandbox>();

            //var v1 = new State();
            //var v2 = new State();
            //var transition = new Transition();
            //transition.Label = "label";
            //transition.Parameter = "p1;p2";
            //transition.Source = v1;
            //transition.Destination = v2;
            
            //Expect.Call(sandbox.CanExecute(transition)).IgnoreArguments().Return(true);
            //repository.ReplayAll();


            //var model = new ExecutableModel(new List<Transition> { transition },
            //                                new List<State> { v1, v2 }, statisticsManager, sandbox);


            //model.GetExecutableOutTransitions(v1).ToList();

            //repository.VerifyAll();
        }
    }
}