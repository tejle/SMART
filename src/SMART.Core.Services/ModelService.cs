using SMART.Core.DomainModel;

namespace SMART.Core.Services
{
    using System;

    using Interfaces.Services;

    using IOC;

    public class ModelService : IModelService
    {
        public Model CreateModel(string name)
        {
            var model = Resolver.Resolve<Model>();
            model.Name = name;
            return model;
        }

        public State AddState(Model model)
        {
            var state = Resolver.Resolve<State>();
            state.Id = Guid.NewGuid();
            state.Label = string.Format("State.{0}", model.States.Count);
            model.Add(state);
            return state;
        }

        public Transition AddTransition(Model model)
        {
            var transition = Resolver.Resolve<Transition>();
            transition.Id = Guid.NewGuid();
            transition.Label = string.Format("Transition.{0}", model.Transitions.Count);
            model.Add(transition);
            return transition;
        }

        public bool AddExistingState(Model model, State state)
        {
            return model.Add(state) != null;
        }

        public bool AddExistingTransition(Model model, Transition transition)
        {
            return model.Add(transition) != null;
        }

        public bool RemoveTransition(Model model, Transition transition)
        {
            return model.Remove(transition) != null;
        }

        public bool RemoveState(Model model, State state)
        {
            return model.Remove(state) != null;
        }

        public bool RenameState(State state, string value)
        {
            state.Label = value;
            return true;
        }

        public bool RenameTransition(Transition transition, string value)
        {
            transition.Label = value;
            return true;
        }

        //public bool MoveState(State state, Point location)
        //{
        //    state.Location = location;
        //    return true;
        //}
    }
}