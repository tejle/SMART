//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using NUnit.Framework;
//using SMART.Core.DomainModel;

//namespace SMART.Test.Core
//{
//    [TestFixture]
//    public class ModelTests
//    {
//        Model model;

//        [SetUp]
//        public void setup()
//        {
//            model = new Model();
//            model.Add(new StartState() {Id = Guid.NewGuid(), Label = "Start"});
//            model.Add(new StopState() { Id = Guid.NewGuid(), Label = "Stop" });
//        }

//        [Test]
//        public void adding_a_state_to_the_model_should_result_in_state_count_be_equal_to_one()
//        {
//            model = new Model();
//            var state = new State();

//            var success = model.Add(state);
//            Assert.AreEqual(true, success, "Model.Add failed");
//            Assert.AreEqual(1, model.States.Count());
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void adding_a_null_state_to_a_model_should_throw_an_argumentnullexception()
//        {
//            model.Add((State)null);
//        }

//        [Test]
//        public void adding_the_same_state_twice_should_ignore_the_second_and_return_false()
//        {
//            model = new Model();
//            var state = new State();

//            model.Add(state);
//            Assert.AreEqual(false, model.Add(state));
//            Assert.AreEqual(1, model.States.Count());
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void adding_a_null_transition_to_a_model_should_throw_an_argumentnullexception()
//        {
//            model.Add((Transition)null);
//        }

//        [Test]
//        public void adding_a_transition_to_the_model_should_set_the_transition_to_the_states()
//        {
//            var state1 = new State();
//            var state2 = new State();
//            var transition = new Transition();
//            transition.Source = state1;
//            transition.Destination = state2;

//            model.Add(state1);
//            model.Add(state2);

//            var success = model.Add(transition);
//            Assert.IsTrue(success);

//        }

//        [Test]
//        public void outtransitions_should_return_transitions_that_have_this_as_source()
//        {
//            var state1 = new State();
//            var state2 = new State();
//            var transition = new Transition();
//            transition.Source = state1;
//            transition.Destination = state2;

//            model.Add(state1);
//            model.Add(state2);
//            model.Add(transition);

//            var transitions = model.OutTransitions(e => e.Source.Equals(state1));
//            Assert.Contains(transition, transitions.ToList());
//        }

//        [Test]
//        public void removing_a_state_should_return_true_if_success()
//        {
//            var state = new State();
//            model.Add(state);
//            Assert.IsTrue(model.Remove(state));
//        }

//        [Test]
//        public void removing_a_list_of_state_should_return_true_if_success()
//        {
//            var state = new List<State> { new State() };
//            model.AddRange(state);
//            Assert.IsTrue(model.RemoveRange(state));
//        }

//        [Test]
//        public void add_list_of_transitions_should_return_true()
//        {

//            var transition = new List<Transition>{new Transition()};
//            Assert.IsTrue(model.AddRange(transition));
//        }

//        [Test]
//        public void removing_a_transition_should_return_true_if_success()
//        {
//            var state1 = new State();
//            var state2 = new State();
//            var transition = new Transition();
//            transition.Source = state1;
//            transition.Destination = state2;

//            model.Add(state1);
//            model.Add(state2);
//            model.Add(transition);
//            Assert.IsTrue(model.Remove(transition));
//        }

//        [Test]
//        public void removing_a_list_of_transitions_should_return_true_if_success()
//        {
//            var state1 = new State();
//            var state2 = new State();
//            var transition = new Transition();
//            transition.Source = state1;
//            transition.Destination = state2;

//            model.Add(state1);
//            model.Add(state2);
//            model.Add(transition);

//            var range = new List<Transition> {transition};
//            Assert.IsTrue(model.RemoveRange(range));
//        }

//        [Test]
//        public void removing_a_transition_from_a_start_state_should_return_true()
//        {
//            var transition = new Transition();
            
//            model.Add(transition);

//            var start = model.StartState.Out.ToList();
//            Assert.IsTrue(model.RemoveRange(start));
//        }

//        [Test]
//        public void removing_a_transition_from_a_stop_state_should_return_true()
//        {
//            model = new Model();
//            var transition = new Transition();

//            model.Add(transition);

//            var stop = model.StopState.In.ToList();
//            Assert.IsTrue(model.RemoveRange(stop));
//        }

//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void removerange_null_transition_should_throw_argument_null_exception()
//        {
//            model.RemoveRange((List<Transition>) null);
//        }
//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void removerange_null_state_should_throw_argument_null_exception()
//        {
//            model.RemoveRange((List<State>)null);
//        }
//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void remove_null_transition_should_throw_argument_null_exception()
//        {
//            model.Remove((Transition)null);
//        }
//        [Test]
//        [ExpectedException(typeof(ArgumentNullException))]
//        public void remove_null_state_should_throw_argument_null_exception()
//        {
//            model.Remove((State) null);

//        }
//        [Test]
//        public void add_transition_should_return_false_when_transition_exists()
//        {
//            var state1 = new State();
//            var state2 = new State();
//            var transition = new Transition();
//            transition.Source = state1;
//            transition.Destination = state2;

//            model.Add(state1);
//            model.Add(state2);
//            model.Add(transition);

//            Assert.IsFalse(model.Add(transition));
//        }

//        [Test]
//        public void add_transition_should_return_false_when_source_does_not_exist_in_model()
//        {
//            var state1 = new State();
//            var state2 = new State();
//            var transition = new Transition();
//            transition.Source = state1;
//            transition.Destination = state2;

//            model.Add(state2);
//            Assert.IsFalse(model.Add(transition));

            
//        }


//        [Test]
//        public void add_transition_should_return_false_when_destination_does_not_exist_in_model()
//        {
//            var state1 = new State();
//            var state2 = new State();
//            var transition = new Transition();
//            transition.Source = state1;
//            transition.Destination = state2;

//            model.Add(state1);
//            Assert.IsFalse(model.Add(transition));


//        }
//    }
//}