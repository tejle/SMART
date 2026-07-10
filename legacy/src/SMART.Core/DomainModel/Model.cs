using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SMART.Core.Events;
using SMART.Core.Exceptions;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;
using Microsoft.Practices.Unity;

namespace SMART.Core.DomainModel
{
    public class Model : SmartEntityCollectionBase, ITaggable, IModel
    {
        private Guid id;
        private string name;
        private readonly List<IModelElement> elements;
        private StartState startState;
        private StopState stopState;
        
        [Config]
        public virtual string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public IModelElement this[string label] {
            get {
                var elem = elements.Find(s => s.Label == label);
                return elem;
            }
        }

        public virtual Guid Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged("Id");
            }
        }

        public Dictionary<string, object> Tags
        {
            get;
            set;
        }

        public StartState StartState
        {
            get { if (startState == null) startState = new StartState(); return startState; }
            set
            {
                startState = value;
                OnPropertyChanged("StartState", value);
            }
        }

        public StopState StopState
        {
            get { if (stopState == null) stopState = new StopState(); return stopState; }
            set
            {
                stopState = value;
                OnPropertyChanged("StopState", value);
            }
        }

        public List<Transition> Transitions
        {
            get { return (from e in elements where e is Transition select (Transition)e).ToList(); }
        }

        public List<State> States
        {
            get { return (from e in elements where e is State select (State)e).ToList(); }
        }

        public IEnumerable<IModelElement> Elements { get { return elements; } }

        [Config]
        public SmartImage Thumbnail
        {
            get; set;
        }

        [InjectionConstructor]
        public Model() : this(string.Empty) { }
        public Model(string name) : this(name, Guid.NewGuid()){}
        public Model(string name, Guid id)
        {
            Name = name;
            Id = id;
            elements = new List<IModelElement>();
            CreateInitialStates();
        }

        public IModel Add(State state)
        {
          
            if (States.Contains(state)) throw new ModelException("state was already found in model");

            SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "States", new[] { state });

            return AddModelItem(state);
        }

        public IModel Add(IList<State> states)
        {
            SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "States", (IList)states);

            return AddModelItems(states);
        }

        public IModel Add(Transition transition)
        {
            ConnectTransition(transition);
            SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "Transitions", new[] { transition });

            return AddModelItem(transition);
        }


        public IModel Add(IList<Transition> transitions)
        {
            foreach (var t in transitions)
            {
                ConnectTransition(t);
            }

            SendCollectionChanged(SmartNotifyCollectionChangedAction.Add, "Transitions", (IList)transitions);
            return AddModelItems(transitions);
        }


        public IModel Remove(State state)
        {
            if (!States.Contains(state)) throw new ModelException("state not found in model");

            var transitions = state.Transitions;
            foreach (var transition in transitions)
            {
                if (!Transitions.Contains(transition)) throw new ModelException("transition not found in model");

                if (transition.Source != state) transition.Source.Remove(transition);
                if (transition.Destination != state) transition.Destination.Remove(transition);

                RemoveModelItem(transition);
            }

            if (transitions.Count > 0) SendCollectionChanged(SmartNotifyCollectionChangedAction.Remove, "Transitions", transitions);

            SendCollectionChanged(SmartNotifyCollectionChangedAction.Remove, "States", new[] { state });

            return RemoveModelItem(state);
        }


        public IModel Remove(Transition transition)
        {
            RemoveTransitionFromStates(transition);

            SendCollectionChanged(SmartNotifyCollectionChangedAction.Remove, "Transitions", new[] { transition });

            return RemoveModelItem(transition);
        }

        public void ChangeTransitionSource(Transition transition, State newSource)
        {
            var oldSource = transition.Source;
            oldSource.Remove(transition);
            transition.Source = newSource;
            newSource.Add(transition);
        }

        public void ChangeTransitionDestination(Transition transition, State newDestination)
        {
            var oldDestination = transition.Destination;
            oldDestination.Remove(transition);
            transition.Destination = newDestination;
            newDestination.Add(transition);
        }

        private void CreateInitialStates()
        {
            Add(StartState)
                .Add(StopState);
        }

        private void ConnectTransition(Transition transition)
        {
            VerifyThat_TransitionDoesNotExistAndIsConfigured(transition);
                
            transition.Source.Add(transition);
            transition.Destination.Add(transition);
        }

        private void VerifyThat_TransitionDoesNotExistAndIsConfigured(Transition transition)
        {
            
            if (Transitions.Find(t=> t.Id == transition.Id) != null ) throw new ModelException("transition was already found in model");
            if (transition.Source == null) transition.Source = StartState;
            if (transition.Destination == null) transition.Destination = StopState;
            if (States.Find(s=>s.Id ==transition.Source.Id) == null) throw new ModelException("source not found in model");
            if (States.Find(s=> s.Id ==transition.Destination.Id) ==null) throw new ModelException("target not found in model");
            
        }

        private void RemoveTransitionFromStates(Transition transition)
        {
            if (!Transitions.Contains(transition)) throw new ModelException("transition not found in model");

            if(transition.Source != null)
                transition.Source.Remove(transition);
            if (transition.Destination != null)
            transition.Destination.Remove(transition);
        }

        private Model AddModelItem<T>(T element) where T : IModelElement
        {
            elements.Add(element);
            return this;
        }

        private Model AddModelItems<T>(IList<T> items) where T : IModelElement
        {
            elements.AddRange(items.Cast<IModelElement>());
            return this;
        }

        private Model RemoveModelItem(IModelElement element)
        {
            elements.Remove(element);
            return this;
        }
    }
}