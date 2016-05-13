using NUnit.Framework;
using SMART.Core.DomainModel;
using SMART.Core.Events;
using SMART.Core.Exceptions;

namespace SMART.Test.Core
{
    [TestFixture]
    public class WorkingWithModel
    {
        private Model model;

        [SetUp]
        public void setup()
        {
            model = new Model();
        }
        [Test]
        public void adding_state_should_return_the_model_back()
        {
            State state = new State();

            var m = model.Add(state);
            Assert.AreSame(model, m);
            Assert.AreEqual(3, model.States.Count);
        }

        [Test]
        public void adding_transition_should_return_back_the_model()
        {
            Transition transition = new Transition() { Source = model.StartState, Destination = model.StopState };

            var m = model.Add(transition);
            Assert.AreSame(model, m);
            Assert.AreEqual(1, model.Transitions.Count);

        }

        [Test]
        public void adding_transition_with_null_source_target_should_set_start_and_stop()
        {
            // Assign
            Transition transition = new Transition() { Source = model.StartState, Destination = model.StopState };

            // Act
            model.Add(transition);

            // Assert
            Assert.AreSame(model.StartState, transition.Source);
            Assert.AreSame(model.StopState, transition.Destination);
        }

        [Test]
        public void should_override_initial_start_and_stop_state_if_set()
        {
            // Assign
            StartState startState = new StartState();
            StopState stopState = new StopState();
            bool firedStart = false;
            bool firedStop = false;
            model.PropertyChanged += (s, e) =>
                                         {
                                             if (e.PropertyName.Equals("StartState")) firedStart = true;
                                             if (e.PropertyName.Equals("StopState")) firedStop = true;
                                         };

            // Act
            model.StartState = startState;
            model.StopState = stopState;

            // Assert
            Assert.AreSame(startState, model.StartState);
            Assert.AreSame(stopState, model.StopState);
            Assert.True(firedStart & firedStop);
        }

        [Test]
        public void adding_transition_with_invalid_source_should_throw_exception()
        {
            // Assign
            Transition transition = new Transition()
                                        {
                                            Source = new State(),
                                            Destination = model.StopState
                                        };
            // Act
            Assert.Throws<ModelException>(()=>model.Add(transition));
        }


        [Test]
        public void adding_transition_with_invalid_target_should_throw_exception()
        {
            // Assign
            Transition transition = new Transition()
                                        {
                                            Source = model.StartState,
                                            Destination = new State()
                                        };
            // Act
            Assert.Throws<ModelException>(() => model.Add(transition));
        }

        [Test]
        public void removing_a_state_should_also_remove_any_connected_transitions()
        {
            // Assign
            State s1 = new State();
            State s2 = new State();
            Transition t1 = new Transition() { Source = model.StartState, Destination = s1 };
            Transition t2 = new Transition() { Source = s1, Destination = s2 };
            Transition t3 = new Transition() { Source = s2, Destination = model.StopState };

            model.Add(s1)
                .Add(s2)
                .Add(t1)
                .Add(t2)
                .Add(t3);

            // Act
            model.Remove(s2);
            // Assert
            Assert.False(model.Transitions.Contains(t2));
            Assert.False(model.Transitions.Contains(t3));
            Assert.False(model.States.Contains(s2));
            Assert.True(model.States.Contains(s1));
            Assert.True(model.Transitions.Contains(t1));
        }

        [Test]
        public void removing_a_transition_should_deconnect_it_from_states()
        {
            // Assign
            State s1 = new State();
            State s2 = new State();
            Transition t1 = new Transition() { Source = model.StartState, Destination = s1 };
            Transition t2 = new Transition() { Source = s1, Destination = s2 };
            Transition t3 = new Transition() { Source = s2, Destination = model.StopState };

            model.Add(s1)
                .Add(s2)
                .Add(t1)
                .Add(t2)
                .Add(t3);

            // Act
            model.Remove(t2);
            // Assert
            Assert.False(model.Transitions.Contains(t2));
            Assert.False(s1.Transitions.Contains(t2));
            Assert.False(s2.Transitions.Contains(t2));
            Assert.True(model.States.Contains(s1));
            Assert.True(model.States.Contains(s2));
            Assert.True(model.Transitions.Contains(t1));
        }

        [Test]
        public void adding_state_should_fire_notified_property_changed()
        {
            // Assign
            var state = new State();

            bool fired = false;
            model.CollectionChanged += (s, e) =>
                                           {
                                               fired = true;
                                               Assert.Contains(state, e.NewItems);
                                           };
            // Act
            model.Add(state);

            // Assert
            Assert.IsTrue(fired);
        }

        [Test]
        public void removing_state_should_fire_notified_property_changed()
        {
            // Assign
            var state = new State();

            bool fired = false;
            model.CollectionChanged += (s, e) =>
                                           {
                                               if (e.Action != SmartNotifyCollectionChangedAction.Remove) return;
                                               fired = true;
                                               Assert.Contains(state, e.OldItems);
                                           };
            // Act
            model.Add(state);
            model.Remove(state);

            // Assert
            Assert.IsTrue(fired);
        }

        [Test]
        public  void change_transition_source_should_update_old_and_new_state()
        {
            // Assign
            var transition = new Transition("transition");
            var sourceState = new State("source");
            var destinationState = new State("target");

            var newSourceState = new State("new_source");

            transition.Source = sourceState;
            transition.Destination = destinationState;

            sourceState.Add(transition);
            destinationState.Add(transition);
            
            model.Add(sourceState);
            model.Add(destinationState);
            model.Add(newSourceState);
            model.Add(transition);

            // Act            
            model.ChangeTransitionSource(transition, newSourceState);

            // Assert
            Assert.AreSame(newSourceState, transition.Source, "The source for the Transition is not refering to the new State");
            Assert.IsEmpty(sourceState.Transitions, "The Transition collection should be empty on the old State!");
        }

        [Test]
        public void change_transition_destination_should_update_old_and_new_state()
        {
            // Assign
            var transition = new Transition("transition");
            var sourceState = new State("source");
            var destinationState = new State("target");

            var newDestinationState = new State("new_destination");

            transition.Source = sourceState;
            transition.Destination = destinationState;

            sourceState.Add(transition);
            destinationState.Add(transition);

            model.Add(sourceState);
            model.Add(destinationState);
            model.Add(newDestinationState);
            model.Add(transition);

            // Act            
            model.ChangeTransitionDestination(transition, newDestinationState);

            // Assert
            Assert.AreSame(newDestinationState, transition.Destination, "The source for the Transition is not refering to the new State");
            Assert.IsEmpty(destinationState.Transitions, "The Transition collection should be empty on the old State!");
        }
    }
}