using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using SMART.Core;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.IOC;

namespace SMART.Core
{
	public class SimpleModelCompiler : IModelCompiler
	{

		public IModel Compile(IEnumerable<IModel> models)
		{
			Queue<IModel> modelQueue = new Queue<IModel>(models);
            if(modelQueue.Count == 0) return null;

			Model result = Resolver.Resolve<Model>();
			result.Name = "Merged";
			var mainModel = ModelCopier.MakeCopyOfModel(modelQueue.Dequeue() as Model);
            var referenceStates = GetReferenceStates(mainModel, modelQueue);

			RebindMainModel(result, mainModel);

			while (modelQueue.Count > 0)
			{
				var modelTwo = ModelCopier.MakeCopyOfModel(modelQueue.Dequeue() as Model); // models.ElementAt(1) 

				if (!ShouldMerge(modelTwo, referenceStates))
					continue;

				

				result.Add(GetNormalStates(modelTwo));

				result.Add(GetInternalTransitions(modelTwo));

				var refStates = referenceStates.Where(s => RegexMatchLabelToModel(modelTwo, s).Success); //FindGlobalReferenceInModel(mainModel);

				var refState = refStates.First();

				RemapInTransitionsToReference(
					result.States.Find(s => s.Id == refState.Id).InTransitions,
					FindInTransitionToModel(modelTwo).Destination
					);

				RemapOutTransitionsToReference(
					result.States.Find(v => v.Id == refState.Id).OutTransitions,
					FindOutTransitionToModel(modelTwo).Source
					);

				var state = result.States.Find(v => v.Id == refState.Id);

				SafeStateRemove(result, state);
			}
			return result;
		}

		private void RebindMainModel(Model result, Model mainModel)
		{
			result.Add(GetNormalStates(mainModel));

			var inClone = FindInTransitionToModel(mainModel);
			var outClone = FindOutTransitionToModel(mainModel);
			var tin = RemapInTransitions(inClone,
			                             result.StartState);

			var tout = RemapOutTransitions(outClone,
			                               result.StopState);

			result.Add(GetNormalTransitions(mainModel));
			
			if(tin !=null)
				result.Add(tin);
            
			if(tout != null)
				result.Add(tout);
		}

		/// <summary>
		/// Get all states except start and stop
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		private static List<State> GetNormalStates(IModel model)
		{

			var states =
				model.States.Except(new List<State> { model.StartState, model.StopState });
			return states.ToList();
		}

		

		private List<Transition> GetNormalTransitions(Model model)
		{
			var transitions =
				model.Transitions.Except(new List<Transition> { FindInTransitionToModel(model), FindOutTransitionToModel(model) });
			return transitions.ToList();
		}

		private Match RegexMatchLabelToModel(IModel model, State state)
		{
			return Regex.Match(model.Name, state.Label);
		}

		private bool ShouldMerge(Model model, IEnumerable<State> refernceStates)
		{
			return refernceStates.Where(s => RegexMatchLabelToModel(model, s).Success).Count() > 0;

		}

		private void SafeStateRemove(Model result, State state)
		{
			state.ClearTransitions();
			result.Remove(state);
		}

		private void RemapOutTransitionsToReference(ReadOnlyCollection<Transition> transitions, State source)
		{
			transitions.ForEach(t => t.Source = source);
		}

		private void RemapInTransitionsToReference(IEnumerable<Transition> transitions, State destination)
		{
			transitions.ForEach(t => t.Destination = destination);
		}

		private List<Transition> GetInternalTransitions(Model model)
		{
			return model.Transitions.Except(
				new List<Transition> { FindInTransitionToModel(model), FindOutTransitionToModel(model) }).ToList();
		}

		private Transition RemapOutTransitions(Transition transition, State state)
		{
			if(transition != null)
				transition.Destination = state;
			return transition;
		}

		private Transition RemapInTransitions(Transition transition, State state)
		{
            
			transition.Source = state;
			return transition;
		}

		/// <summary>
		/// Get Transitions that map to the startState of the model
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Transition FindInTransitionToModel(Model model)
		{
			return model.StartState.Transitions.First();
		}


		/// <summary>
		/// Get transitions that map t
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		public Transition FindOutTransitionToModel(Model model)
		{
			return model.StopState.Transitions.FirstOrDefault();
		}

		public IEnumerable<Model> FindModelsMatchingReference(State state, List<Model> models)
		{
			if (state.Type != StateType.GlobalReference)
				throw new ArgumentException("state is not a global reference");
			if (models == null || models.Count == 0)
				throw new ArgumentException("models is null or empty");

			return models.Where(m => RegexMatchLabelToModel(m, state).Success);
		}

		public State FindGlobalReferenceInModel(Model model)
		{
			if (model == null)
				throw new ArgumentException("model is null");
			if (model.States == null || model.States.Count == 0)
				throw new ArgumentException("model.States is null or empty");

			var refs = model.States.Where(v => v.Type == StateType.GlobalReference);
			return refs.Count() == 0 ? null : refs.First();
		}

		private List<State> GetReferenceStates(IModel model, IEnumerable<IModel> list)
		{
			var state = from s in model.States
						from m in list
						where RegexMatchLabelToModel(m, s).Success
						select s;

			return state.ToList();
		}

		public IExecutionEnvironment CreateSandbox(IModel model)
		{
			return new SimpleExecutionEnvironment();
		}

		public IStep CreateStep(IModelElement modelElement)
		{
			return new BasicStep(modelElement);
		}

		public IModel Compile2(IEnumerable<IModel> models)
		{
			IModel model;
			Queue<IModel> queue = new Queue<IModel>(models);

			model = queue.Dequeue();
			State state = GetReferenceStates(model, queue).First();

			while (queue.Count > 0)
			{
				IModel tmp = queue.Dequeue();

				var states = tmp.States.Where(s => !(s is StartState) && !(s is StopState));
				model.Add(new List<State>(states));

				var transitions = tmp.Transitions.Where(t => t.Destination != tmp.StopState && t.Source != tmp.StartState);
				model.Add(new List<Transition>(transitions));
				var transitionsToRemove = new List<Transition>();
				state.InTransitions.ForEach(t =>
												{
													t.Destination = tmp.StartState.Transitions.First().Destination;
													transitionsToRemove.Add(t);
												}
					);
				state.OutTransitions.ForEach(t =>
												{
													t.Source = tmp.StopState.Transitions.First().Source;
													transitionsToRemove.Add(t);
												}
					);

				transitionsToRemove.ForEach(state.Remove);
			}


			model.Remove(state);

			return model;//models.First();
		}
	}
}