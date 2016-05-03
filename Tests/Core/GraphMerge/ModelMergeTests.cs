//using System;
//using System.Linq;
//using NUnit.Framework;
//using SMART.Core;
//using Moq;
//using SMART.Core.DomainModel;
//using SMART.Core.Interfaces;
//using SMART.Core.BusinessLayer;

//using System.Collections.Generic;

//namespace SMART.Test.Core.ModelMerge
//{
//    [TestFixture]
//    public class ModelMergeTests {

//        Mock<ISandbox> sandbox;
//        Mock<IStatisticsManager> statisticsManager;
//        ModelBuilderTestDouble modelBuilder;
//        [SetUp]
//        public void setup()
//        {
//            sandbox = new Mock<ISandbox>();
//            statisticsManager = new Mock<IStatisticsManager>();
//            modelBuilder = new ModelBuilderTestDouble(sandbox.Object, statisticsManager.Object);
//        }

//        [Test]
//        public void Normal_states_should_return_all_states_except_start_and_stop_state()
//        {
//            // Assign
//            Model model = new Model();
//            model.Add(new StartState());
//            model.Add(new StopState());
//            model.Add(new State());
        
//            // Act
//            var list = modelBuilder.NormalStates(model);
            
//            // Assert
//            Assert.AreEqual(1,list.Count());

//        }

//        [Test]
//        public void global_states_should_return_all_of_type_globalreference_and_not_start_stop()
//        {
//            // Assign
//            Model model = new Model();
//            model.Add(new StartState());
//            model.Add(new StopState());
//            model.Add(new State(){Type = StateType.Normal});
//            model.Add(new State() {Type = StateType.GlobalReference});
//            model.Add(new State() { Type = StateType.LocalReference});
//            // Act

//            var list = modelBuilder.GlobalRefereneceTypeStates(model);
//            // Assert
//            Assert.AreEqual(1, list.Count());
//        }

//        [Test]
//        public void given_a_globalreference_state_models_that_matches_should_be_returned()
//        {
//            // Assign
//            Model model = new Model(){Name = "testmodel"};
//            model.Add(new StartState());
//            model.Add(new StopState());
//            model.Add(new State() { Type = StateType.Normal });
//            model.Add(new State() { Type = StateType.GlobalReference, Label = "model2" });
//            model.Add(new State() { Type = StateType.LocalReference });
            
//            Model model2 = new Model(){Name = "model2"};
//            var allmodels = new List<Model>() {model, model2};

//            // Act
//            var list = modelBuilder.GetModelMatches(model.States.Where(v => v.Type == StateType.GlobalReference).First(), allmodels);
//            // Assert
//            Assert.AreEqual(1, list.Count());
//        }

//        [Test]
//        public void normal_transitions_should_return_all_valid_transitions()
//        {
//            // Assign
//            var stateA = new State() {Type = StateType.Normal};
//            var stateB = new State() {Type = StateType.GlobalReference, Label = "model2"};
//            var stateC = new State() {Type = StateType.LocalReference};
//            var transitionAB = new Transition() {Source = stateA, Destination = stateB};
//            var transitionBC = new Transition() {Source = stateB, Destination = stateC};

//            Model model = new Model() { Name = "testmodel" };
//            model.Add(new StartState());
//            model.Add(new StopState());
//            model.Add(stateA);
//            model.Add(stateB);
//            model.Add(stateC);
//            model.Add(transitionAB);
//            model.Add(transitionBC);

//            // Act
//            var transitions = modelBuilder.NormalTransitions(model, new List<State> {stateA, stateB, stateC});

//            // Assert
//            Assert.AreEqual(2,transitions.Count());

//        }

//        [Test]
//        public void rebind_global_reference_should_do_just_that_QUESTIONMARK()
//        {
//            // Assign
//            var allglobalmodels = new List<Model>();
//            var newtransitions = new List<Transition>();
//            var statemap = new Dictionary<State, State>();

//            var start = new StartState();
//            var stop = new StopState();
//            var stateA = new State() { Type = StateType.Normal };
//            var stateB = new State() { Type = StateType.GlobalReference, Label = "model2" };
//            var stateC = new State() { Type = StateType.LocalReference };
//            var transitionAB = new Transition() { Source = stateA, Destination = stateB, Label ="AB"};
//            var transitionBC = new Transition() { Source = stateB, Destination = stateC , Label = "BC"};
//            var transitionStart = new Transition() {Source = start, Destination = stateA, Label = "StartA"};
//            var transitionStop = new Transition() {Source = stateC, Destination = stop, Label = "CStop"};

//            Model model = new Model() { Name = "testmodel" };
//            model.Add(start);
//            model.Add(stop);
//            model.Add(stateA);
//            model.Add(stateB);
//            model.Add(stateC);
//            model.Add(transitionStart);
//            model.Add(transitionAB);
//            model.Add(transitionBC);
//            model.Add(transitionStop);

//            Model model2 = new Model(){Name = "model2"};
//            var start2 = new StartState();
//            var stateD = new State();
//            var stateE = new State();
//            var stop2 = new StopState();
//            var transitionStart2D = new Transition() {Source = start2, Destination = stateD, Label = "StartD"};
//            var transitionDE = new Transition() {Source = stateD, Destination = stateE, Label = "DE"};
//            var transitionEStop = new Transition() {Source = stateE, Destination = stop2, Label = "EStop"};
//            model2.Add(start2);
//            model2.Add(stateD);
//            model2.Add(stateE);
//            model2.Add(stop2);
//            model2.Add(transitionStart2D);
//            model2.Add(transitionDE);
//            model2.Add(transitionEStop);

//            var eg = modelBuilder.Merge(new List<Model> {model, model2}, model);

//            // Act

//            // Assert

//        }

        
//        private class ModelBuilderTestDouble : ModelBuilder
//        {
//            public ModelBuilderTestDouble(ISandbox sandbox, IStatisticsManager statisticManager) : base(sandbox, statisticManager)
//            {
//            }

//            public new IEnumerable<State> NormalStates(Model model)
//            {
//                return base(model);
//            }

//            public new IEnumerable<State> GlobalRefereneceTypeStates(Model model)
//            {
//                return base.GlobalRefereneceTypeStates(model);
//            }

//            public new void RecursiveGetGlobalModels(Model model, IEnumerable<Model> models, ICollection<Model> globalModels)
//            {
//                base.RecursiveGetGlobalModels(model, models, globalModels);
//            }

//            public new IEnumerable<Model> GetModelMatches(ModelElement modelElement, IEnumerable<Model> models)
//            {
//                return base.GetModelMatches(modelElement, models);

//            }

//            public new IEnumerable<Transition> NormalTransitions(Model model, IEnumerable<State> availableStates)
//            {
//                return base.NormalTransitions(model, availableStates);
//            }
//        }
//    }
//}
