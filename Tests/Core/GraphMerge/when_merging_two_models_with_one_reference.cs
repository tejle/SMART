using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SMART.Core.DomainModel;
using SMART.Core.Services;
using SMART.IOC;


namespace SMART.Test.Core.ModelMerge
{
    [TestFixture]
    public class when_merging_two_models_with_one_reference
    {
        private Model modelA;
        private Model modelB;
        private Model modelC;
        private ModelService modelService;
        private ModelMergeService modelMergeService;
        [SetUp]
        public void setup()
        {
            modelService = new ModelService();
            modelMergeService = new ModelMergeService(modelService);
            createModelA();
            createModelB();
            createModelC();

            Resolver.Configure();
        }

        private void createModelA()
        {
            modelA = new Model()
                         {
                             Name = "modelA",
                         };

            var startA = new StartState();
            var stopA = new StopState();
            var stateA = new State() {Label = "A"};
            var stateB = new State() {Label = "B", Type = StateType.GlobalReference};
            var stateC = new State() {Label = "C"};
            var transitionSA = new Transition() {Label = "SA", Source = startA, Destination = stateA};
            var transitionAB = new Transition() {Label = "AB", Source = stateA, Destination = stateB};
            var transitionBC = new Transition() {Label = "BC", Source = stateB, Destination = stateC};
            var transitionCS = new Transition() {Label = "CS", Source = stateC, Destination = stopA};

            modelA.Add(startA);
            modelA.Add(stateA);
            modelA.Add(stateB);
            modelA.Add(stateC);
            modelA.Add(stopA);
            modelA.Add(transitionSA);
            modelA.Add(transitionAB);
            modelA.Add(transitionBC);
            modelA.Add(transitionCS);
        }

        private void createModelB()
        {
            modelB = new Model()
            {
                Name = "modelB",
            };

            var startB = new StartState();
            var stopB = new StopState();
            var stateD = new State() { Label = "D" };
            var stateE = new State() { Label = "E" };
            var transitionSD = new Transition() { Label = "SD", Source = startB, Destination = stateD};
            var transitionDE = new Transition() { Label = "DE", Source = stateD, Destination = stateE };
            var transitionES = new Transition() { Label = "ES", Source = stateE, Destination = stopB };
            
            modelB.Add(startB);
            modelB.Add(stateD);
            modelB.Add(stateE);
            modelB.Add(stopB);
            modelB.Add(transitionSD);
            modelB.Add(transitionDE);
            modelB.Add(transitionES);
        }

        private void createModelC()
        {
            modelC = new Model();
            modelC.Name = "Test";

        }



        [Test][Ignore]
        public void should_final_model_contain_one_start_and_one_stop()
        {

            Model model = modelMergeService.Merge(modelA, modelB);

            Assert.AreEqual(1, model.States.OfType<StartState>().Count());
            Assert.AreEqual(1, model.States.OfType<StopState>().Count());

        }

        [Test]
        [Ignore]
        public void should_map_reference_state_to_models()
        {
            // Assign
            
            //// Act
            //var list = modelMergeService.FindModelsMatchingReference(modelA, new List<Model> { modelB, modelC });
            //// Assert
            //Assert.Contains(modelB,list);
            //Assert.AreEqual(1,list.Count);
        }

        [Test]
        [Ignore]
        public void should_result_in_one_connected_model()
        {
            // Assign
            
            // Act

            // Assert

        }

        [Test]
        [Ignore]
        public void all_transitions_in_to_the_reference_in_A_should_go_into_modelB()
        {
            // Assign
            
            // Act
            Model model = modelMergeService.Merge(modelA, modelB);

            // Assert

            Assert.AreEqual(6, model.States.Count, "number of states doesn't match");
            Assert.AreEqual(5, model.Transitions.Count, "number of transitions doesn't match");

        }
        
    }
}