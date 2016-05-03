using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using System.Text;

namespace SMART.Core
{
	public class ModelCompiler : IModelCompiler
	{
		public IModel Compile(IEnumerable<IModel> models)
		{
			IModel startModel = FindStartModel(models);
			IModel resultModel = ModelCopier.MakeCopyOfModel(startModel);
            
			List<IModel> allModelsThatAreReferenced = GetAllModelsThatAreReferenced(resultModel, models);

			AddCopiesOfAllModelsToResult(allModelsThatAreReferenced, resultModel);

			RebindMSRToInAndOutTransitions(resultModel, startModel, allModelsThatAreReferenced, models);

			ClearStartAndStopStates(resultModel);

			return resultModel;
		}

		private void ClearStartAndStopStates(IModel resultModel)
		{
			var startStates = resultModel.States.OfType<StartState>().Where(s => s != resultModel.StartState);
			foreach (var state in startStates)
			{
				resultModel.Remove(state);
			}

			var stopStates = resultModel.States.OfType<StopState>().Where(s => s != resultModel.StopState);
			foreach (var state in stopStates)
			{
				resultModel.Remove(state);

			}
		}

		private void RebindMSRToInAndOutTransitions(IModel resultModel, IModel startModel, List<IModel> allModelsThatAreReferenced, IEnumerable<IModel> models)
		{
			var msrInResult = GetModelReferenceStates(resultModel);

            if(allModelsThatAreReferenced.Count == 0) return;
			foreach (var msr in msrInResult)
			{
				var refModels = GetModelsForReference(msr, allModelsThatAreReferenced);
				var inTransitions = refModels.SelectMany(m => m.StartState.Transitions);
				var outTransitions = refModels.SelectMany(m => m.StopState.Transitions);

			    MergeTransitions(msr.InTransitions, inTransitions, resultModel);
				MergeTransitions(msr.OutTransitions, outTransitions, resultModel);

			}

			foreach (var state in msrInResult)
			{
				state.ClearTransitions();
				resultModel.Remove(state);

			}
		}

		private void MergeTransitions(ReadOnlyCollection<Transition> msrTransition, IEnumerable<Transition> refTransitions, IModel resultModel)
		{
            List<Transition> transitionsToAdd = new List<Transition>();
			foreach (var transition in msrTransition)
			{
				foreach (var refTra	in refTransitions)
				{
					Transition t = resultModel.Transitions.Find(tr => tr.Id == refTra.Id);
					Transition copy = MergedTransition(transition, t, resultModel);
				    transitionsToAdd.Add(copy);
				}
			    resultModel.Remove(transition);
			}

            transitionsToAdd.ForEach(t=> t.Id = Guid.NewGuid());

            transitionsToAdd.ForEach(t=> resultModel.Add(t));
		}

		private Transition MergedTransition(Transition msrt, Transition reft, IModel resultModel)
		{
		    Transition copy = ModelCopier.CopyTransition(msrt);

		    copy.Label = MergeLabel(msrt, reft);
		    copy.Parameter = MergeParameters(msrt, reft);
            //Guards
            //Actions
			if(reft.Source is StartState)
			{
			    copy.Destination = resultModel.States.Find(s=> s.Id == reft.Destination.Id);
			    copy.Destination.Add(copy);
			    
                copy.Source = msrt.Source;
			    copy.Source.Add(copy);
			    
                msrt.Destination = null;
			}

			if(reft.Destination is StopState)
			{
			    copy.Source = resultModel.States.Find(s=>s.Id == reft.Source.Id);
			    copy.Source.Add(copy);

			    copy.Destination = msrt.Destination;
			    if (copy.Destination != null) copy.Destination.Add(copy);

			    msrt.Source = null;
			}

			return copy;
		}

	    private string MergeParameters(Transition msrt, Transition reft)
	    {
            if (msrt.Parameter.Equals(reft.Parameter))
                return msrt.Parameter;
	        return string.Format("{0} {1}", msrt.Parameter, reft.Parameter).Trim();
	    }

	    private string MergeLabel(Transition msrt, Transition reft)
	    {
            if (msrt.Label.Equals(reft.Label)) 
                return msrt.Label;
            return string.Format("{0}_{1}", msrt.Label, reft.Label).Trim('_');
	    }


	    private void AddCopiesOfAllModelsToResult(List<IModel> referenced, IModel result)
		{
			foreach (var model in referenced)
			{
                if(model.Id == result.Id) continue;

				var copy = ModelCopier.MakeCopyOfModel(model);
				AddCopyToResult(copy, result);
			}
		}

		private void AddCopyToResult(Model copy, IModel result)
		{
			result.Add(copy.States);
			result.Add(copy.Transitions);
        }

		private List<IModel> GetAllModelsThatAreReferenced(IModel resultModel, IEnumerable<IModel> models)
		{
			
			List<IModel> allModelsThatAreReferenced = new List<IModel>();

			foreach (var model in models)
			{
				var refs = GetModelReferenceStates(model);
				foreach (var state in refs)
				{
					var modelsWithRefs = GetModelsForReference(state, models);
					foreach (var withRef in modelsWithRefs)
					{
						if(!allModelsThatAreReferenced.Contains(withRef))
							allModelsThatAreReferenced.Add(withRef);
					}
				}
			}

			return allModelsThatAreReferenced;
		}

		private IEnumerable<IModel> GetModelsForReference(State msr, IEnumerable<IModel> models)
		{
			return from m in models where Regex.IsMatch(m.Name, msr.Label) select m;
		}

		private IEnumerable<State> GetModelReferenceStates(IModel model)
		{
			return from s in model.States where s.Type == StateType.GlobalReference select s;
		}

		private IModel FindStartModel(IEnumerable<IModel> models)
		{
			if(models == null || models.Count() == 0) throw new ArgumentException("models empty");
			return models.First();
		}

		public IExecutionEnvironment CreateSandbox(IModel model)
		{
			return new SimpleExecutionEnvironment();
		}

		public IStep CreateStep(IModelElement modelElement)
		{
			return new BasicStep(modelElement);
		}

	}


	public class ModelBuilder
	{

		private readonly Guid innerId = Guid.NewGuid();


		public IModel Merge(IEnumerable<IModel> models, IModel root)
		{
			if (!models.Contains(root))
				throw new ArgumentOutOfRangeException("root", "Root model must be included in models to be merged");

			// StateMap maps between old states and new cloned versions to be placed in executable model
			var StateMap = new Dictionary<State, State>();

			// newTransitions contains new cloned versions of transitions to be placed in executable model
			var newTransitions = new List<Transition>();

			var allGlobalModels = AllGlobalModels(root, models);

			allGlobalModels.ForEach(g => AddShallowModel(g, newTransitions, StateMap));

			RebindGlobalReferences(allGlobalModels, newTransitions, StateMap);

			CleanMergedModel(root, newTransitions, StateMap);

			return new Model();// new ExecutableModel(newTransitions, StateMap.Values, statisticManager, sandbox) { Id = innerId };
		}

		private static IEnumerable<IModel> AllGlobalModels(IModel model, IEnumerable<IModel> models)
		{
			var globalModels = new List<IModel>();

			RecursiveGetGlobalModels(model, models, globalModels);

			return globalModels;
		}

		private static void RecursiveGetGlobalModels(IModel model, IEnumerable<IModel> models, ICollection<IModel> globalModels)
		{
			if (globalModels.Contains(model))
				return;
			globalModels.Add(model);

			foreach (var state in GlobalRefereneceTypeModels(model))
			{
				GetModelMatches(state, models)
					.Except(globalModels)
					.ForEach(g => RecursiveGetGlobalModels(g, models, globalModels));
			}
		}

		private static IEnumerable<State> GlobalRefereneceTypeModels(IModel model)
		{
			return NormalModels(model).Where(v => v.Type == StateType.GlobalReference);
		}

		private static IEnumerable<State> NormalModels(IModel model)
		{
			return AllStates(model).Except(new State[] { model.StartState, model.StopState });
		}

		private static IEnumerable<IModel> GetModelMatches(IModelElement modelElement, IEnumerable<IModel> models)
		{
			var r = new Regex(modelElement.Label);
			return models.Where(g => r.IsMatch(g.Name));
		}

		private static void CleanMergedModel(IModel root, List<Transition> newTransitions, Dictionary<State, State> StateMap)
		{
			var superflousStarts = StateMap.Keys.OfType<StartState>().ToList();
			superflousStarts.Remove(root.StartState);
			var superflousStartTransitions = superflousStarts.SelectMany(v => StateMap[v].OutTransitions);
			newTransitions.RemoveAll(e => superflousStartTransitions.Contains(e));
			superflousStarts.ForEach(v => StateMap.Remove(v));

			var superflousStops = StateMap.Keys.OfType<StopState>().ToList();
			superflousStops.Remove(root.StopState);
			var superflousStopTransitions = superflousStarts.SelectMany(v => StateMap[v].InTransitions);
			newTransitions.RemoveAll(e => superflousStopTransitions.Contains(e));
			superflousStops.ForEach(v => StateMap.Remove(v));
		}

		private static void RebindGlobalReferences(IEnumerable<IModel> allGlobalModels, ICollection<Transition> newTransitions, Dictionary<State, State> StateMap)
		{
			var globalReferences = StateMap.Keys.Where(v => v.Type == StateType.GlobalReference).ToList();

			foreach (var reference in globalReferences)
			{
				var state = StateMap[reference];
				var inTransitions = newTransitions.Where(e => e.Destination.Equals(state)).ToList();
				var outTransitions = newTransitions.Where(e => e.Source.Equals(state)).ToList();

				RebindTransitions(allGlobalModels, inTransitions, state, outTransitions, StateMap);

				inTransitions.All(newTransitions.Remove);
				outTransitions.All(newTransitions.Remove);
				StateMap.Remove(reference);
			}
		}

		private static void RebindTransitions(IEnumerable<IModel> models, IEnumerable<Transition> inTransitions, IModelElement state, IEnumerable<Transition> outTransitions, IDictionary<State, State> StateMap)
		{
			// models that match the msr
			var modelMatches = GetModelMatches(state, models);

			//BUG Fix problem with self-referense states

			var innerInTransitions = modelMatches.SelectMany(g => g.StartState.OutTransitions);
			var incomming = MergeTransitions(inTransitions, innerInTransitions);
			// Translate non-cloned destination to cloned
			incomming.ForEach(e => e.Destination = StateMap[e.Destination]);

			var innerOutTransitions = modelMatches.SelectMany(g => g.StopState.InTransitions);
			var outgoing = MergeTransitions(innerOutTransitions, outTransitions);
			// Translate non-cloned source to cloned
			outgoing.ForEach(e => e.Source = StateMap[e.Source]);

		}

		private static IEnumerable<Transition> MergeTransitions(IEnumerable<Transition> transitionTails, IEnumerable<Transition> transitionHeads)
		{
			return
				from outerTransition in transitionTails
				from innerTransition in transitionHeads
				where Matches(outerTransition, innerTransition)
				select MergedTransition(outerTransition, innerTransition);
		}

		private static Transition MergedTransition(Transition outerTransition, Transition innerTransition)
		{
			var e = outerTransition.Clone();
			e.Source = outerTransition.Source;
			e.Destination = innerTransition.Destination;

			if (string.IsNullOrEmpty(e.Label))
				e.Label = innerTransition.Label;
			if (string.IsNullOrEmpty(e.Parameter))
				e.Parameter = innerTransition.Parameter;

			//TODO use sandbox to combine guards
			if (string.IsNullOrEmpty(e.Guard))
			{
				e.Guard = innerTransition.Guard;
			}
			else if (!string.IsNullOrEmpty(innerTransition.Guard))
			{
				e.Guard = string.Format("{0} && {1}", e.Guard, innerTransition.Guard);
			}

			//TODO use sandbox to combine actions
			if (string.IsNullOrEmpty(e.Action))
			{
				e.Action = innerTransition.Action;
			}
			else if (!string.IsNullOrEmpty(innerTransition.Action))
			{
				e.Action = string.Format("{0}; {1}", e.Action, innerTransition.Action);
			}

			return e;
		}

		private static bool Matches(Transition outerTransition, Transition innerTransition)
		{
			//TODO add rules for match based on label format
			// ex. var isExactMatch =outerTransition.Label.Equals(innerTransition.Label);
			return true;//|| isExactMatch;
		}

		private static void AddShallowModel(IModel model, ICollection<Transition> newTransitions, IDictionary<State, State> StateMap)
		{
			// Add all Models to map
			AllStates(model)
				.ForEach(m => AddClone(m, StateMap));

			// add all transitions referenced in allready mapped states
			NormalTransitions(model, StateMap.Keys)
				.ForEach(t => AddClone(t, newTransitions, StateMap));
		}

		private static Transition AddClone(Transition transition, ICollection<Transition> newTransitions, IDictionary<State, State> StateMap)
		{
			var e = transition.Clone();
			newTransitions.Add(e);

			// reroute transitionClone source and destination to already mapped states
			e.Destination = StateMap[e.Destination];
			e.Source = StateMap[e.Source];

			return e;
		}

		private static State AddClone(State state, IDictionary<State, State> StateMap)
		{
			var v = state.Clone();
			StateMap.Add(state, v);

			return v;
		}

		private static IEnumerable<State> AllStates(IModel model)
		{
			return model.States;
		}

		private static IEnumerable<Transition> NormalTransitions(IModel model, IEnumerable<State> availableModels)
		{
			return AllTransitions(model).Where(e => availableModels.Contains(e.Source) && availableModels.Contains(e.Destination));
		}

		private static IEnumerable<Transition> AllTransitions(IModel model)
		{
			return model.Transitions;
		}
	}

}













