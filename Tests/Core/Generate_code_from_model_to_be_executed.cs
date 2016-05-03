using System.Collections.Generic;
using System.Technology.BDD.Nunit;
using NUnit.Framework;
using SMART.Core;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Base.Adapters;

namespace SMART.Test.Core
{
    [TestFixture]
    public class Generate_code_from_model_to_be_executed : ContextSpecification
    {
        Model model;
        InterfaceAdapter adapter;
        List<IStep> seq;

        public override void Given()
        {
            adapter = new InterfaceAdapter();
            adapter.InterfaceName = "TestInterface";
            
            CreateModel();
            seq = GetSequence();
        }

        public override void When()
        {
            adapter.PreExecution();
            foreach (var step in seq)
            {
                adapter.Execute(step.Function, step.Parameters);
            }            
            adapter.PostExection();
        }

        [Test]
        public void some_test_method_to_be_discarded()
        {
            Assert.IsTrue(true);    

        }
        private void CreateModel() {
            model = new Model("TestModel");
            model
                .Add(new State("StateA"))
                .Add(new State("StateB"))
                .Add(new State("StateC"))
                .Add(new Transition("StartToA") { Source = model.StartState, Destination = model["StateA"] as State, Parameter = "param1" })
                .Add(new Transition("AToB") { Source = model["StateA"] as State, Destination = model["StateB"] as State })
                .Add(new Transition("BToC") { Source = model["StateB"] as State, Destination = model["StateC"] as State })
                .Add(new Transition("CToStop") { Source = model["StateC"] as State, Destination = model.StopState });
        }

        private List<IStep> GetSequence() {
            return new List<IStep>()
                       {
                           new BasicStep(model["StartToA"]),
                           new BasicStep(model["StateA"]),
                           new BasicStep(model["AToB"]),
                           new BasicStep(model["StateB"]),
                           new BasicStep(model["BToC"]),
                           new BasicStep(model["StateC"]),
                           new BasicStep(model["CToStop"])
                              
                       };
        }

    }
}