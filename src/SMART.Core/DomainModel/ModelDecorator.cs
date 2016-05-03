using System;
using System.Collections.Generic;
using SMART.Core.Events;
using SMART.Core.Interfaces;

namespace SMART.Core.DomainModel
{
    public abstract class ModelDecorator : SmartEntityCollectionBase, IModel
    {
        private readonly IModel model;

        protected IModel Model
        {
            get { return model; }
        }

        protected ModelDecorator(IModel model)
        {
            this.model = model;
        }

        public Guid Id
        {
            get { return model.Id; }
            set { model.Id = value; }
        }

        public string Name
        {
            get { return model.Name; }
            set { model.Name = value; }
        }

        public StartState StartState
        {
            get { return model.StartState; }
            set { model.StartState = value; }
        }

        public StopState StopState
        {
            get { return model.StopState; }
            set { model.StopState = value;}
        }

        public List<Transition> Transitions
        {
            get { return model.Transitions; }
        }

        public List<State> States
        {
            get { return model.States; }
        }

        public IEnumerable<IModelElement> Elements
        {
            get { return model.Elements; }
        }

        public SmartImage Thumbnail
        {
            get { return model.Thumbnail; }
            set { model.Thumbnail = value; }
        }

        public virtual IModel Add(State state)
        {
            return model.Add(state);
        }

        public virtual IModel Add(IList<State> states)
        {
            return model.Add(states);
        }

        public virtual IModel Add(Transition transition)
        {
            return model.Add(transition);
        }

        public virtual IModel Add(IList<Transition> transitions)
        {
            return model.Add(transitions);
        }

        public virtual IModel Remove(State state)
        {
            return model.Remove(state);
        }

        public virtual IModel Remove(Transition transition)
        {
            return model.Remove(transition);
        }

        public virtual void ChangeTransitionSource(Transition transition, State newSource)
        {
            model.ChangeTransitionSource(transition, newSource);
        }

        public virtual void ChangeTransitionDestination(Transition transition, State newDestination)
        {
           model.ChangeTransitionDestination(transition, newDestination);
        }

    }

    
}