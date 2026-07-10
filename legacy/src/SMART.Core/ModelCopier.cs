using System;
using System.Collections.Generic;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

namespace SMART.Core
{
	public static class ModelCopier
	{
		public static Model MakeCopyOfModel(IModel model)
		{

			Model copy = new Model()
			             	{
			             		Id = model.Id,
			             		Name = model.Name,
			             	};

			List<State> states = CopyAllStates(model, copy);
			List<Transition> transitions = CopyAllTransitions(model, copy, states);

			copy.Add(states);
			copy.Add(transitions);
			return copy;
		}

		private static List<Transition> CopyAllTransitions(IModel model, Model copy, List<State> states)
		{
			var transitions = new List<Transition>();

			foreach (var transition in model.Transitions)
			{
				var tmp = CopyTransition(transition);

				if (transition.Source == model.StartState)
				{
					tmp.Source = copy.StartState;
				}
				else
				{
					tmp.Source = states.Find(s => s.Id == transition.Source.Id);

				}
				if (transition.Destination == model.StopState)
				{
					tmp.Destination = copy.StopState;
				}
				else
				{
					tmp.Destination = states.Find(s => s.Id == transition.Destination.Id);

				}

				transitions.Add(tmp);

			}
			return transitions;
		}

		private static List<State> CopyAllStates(IModel model, Model copy)
		{
			var states = new List<State>();

			foreach (var state in model.States)
			{
				if (state is StartState)
				{
					CopyStartState(state, copy);
				}
				else if (state is StopState)
				{
					CopyStopState(state, copy);
				}
				else
				{
					states.Add(CopyState(state));
				}

			}
			return states;
		}

		public static Transition CopyTransition(Transition transition)
		{
			return new Transition()
			       	{

			       		Id = transition.Id,
			       		Label = transition.Label,
			       		IsDefect = transition.IsDefect,
			       		IsCurrent = transition.IsCurrent,
			       		Action = transition.Action,
			       		Guard = transition.Guard,
			       		Parameter = transition.Parameter,
			       		VisitCount = transition.VisitCount,
			       		Tags = transition.Tags
			       	};
		}

		private static State CopyState(State state)
		{
			return new State()
			       	{
			       		Id = state.Id,
			       		Label = state.Label,
			       		Location = state.Location,
			       		Size = state.Size,
			       		IsDefect = state.IsDefect,
			       		IsCurrent = state.IsCurrent,
			       		Type = state.Type,
			       		VisitCount = state.VisitCount,
			       		Tags = state.Tags,
			       	};
		}

		private static void CopyStopState(State state, IModel copy)
		{
			if(!(state is StopState)) throw new ArgumentException("state was not a stop state");

			copy.StopState.Id = state.Id;
			copy.StopState.Label = state.Label;
			copy.StopState.Location = state.Location;
			copy.StopState.Size = state.Size;
			copy.StopState.IsDefect = state.IsDefect;
			copy.StopState.IsCurrent = state.IsCurrent;
			copy.StopState.Type = state.Type;
			copy.StopState.VisitCount = state.VisitCount;
			copy.StopState.Tags = state.Tags;
		}

		private static void CopyStartState(State state, IModel copy)
		{
			if (!(state is StartState))
				throw new ArgumentException("state was not a start state");

			copy.StartState.Id = state.Id;
			copy.StartState.Label = state.Label;
			copy.StartState.Location = state.Location;
			copy.StartState.Size = state.Size;
			copy.StartState.IsDefect = state.IsDefect;
			copy.StartState.IsCurrent = state.IsCurrent;
			copy.StartState.Type = state.Type;
			copy.StartState.VisitCount = state.VisitCount;
			copy.StartState.Tags = state.Tags;
		}
	}
}