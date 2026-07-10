using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces.Services;

using System;
using System.Collections;

namespace SMART.Core.Services
{
    public class ModelMergeService : IModelMergeService
    {
        private readonly ModelService modelService;

        public ModelMergeService(ModelService modelService)
        {
            this.modelService = modelService;
        }

        public Model Merge(Model modelOne, Model modelTwo)
        {
            var model = modelService.CreateModel(modelOne.Name);

            //model.Add(modelOne.States);
            //model.Add(modelOne.Transitions);


            //model.Add(modelTwo.States.Except(
            //    new List<State> { modelTwo.StartState, modelTwo.StopState }).ToList());

            //model.Add(modelTwo.Transitions.Except(
            //    new List<Transition> { FindInTransitionToModel(modelTwo), FindOutTransitionToModel(modelTwo) }).ToList());

            //var refNode = FindGlobalReferenceInModel(modelOne);

            //var refInTransitions = model.States.Find(v => v.Id == refNode.Id).In;
            //refInTransitions.ForEach(e => e.Destination = FindInTransitionToModel(modelTwo).Destination);

            //var refOutTransitions = model.States.Find(v => v.Id == refNode.Id).Out;
            //refOutTransitions.ForEach(e => e.Source = FindOutTransitionToModel(modelTwo).Source);

            //var node = model.States.Find(v => v.Id == refNode.Id);
            //node.Transitions.Clear();
            //model.Remove(node);

            return model;


        }

        public IEnumerable<Model> FindModelsMatchingReference(State state, List<Model> models)
        {
            if (state.Type != StateType.GlobalReference) throw new ArgumentException("state is not a global reference");
            if (models == null || models.Count == 0) throw new ArgumentException("models is null or empty");

            return models.Where(g => Regex.Match(g.Name, state.Label).Success);
        }

        /// <summary>
        /// At this point a model can only contain one reference
        /// </summary>
        /// <param name="model">a model</param>
        /// <returns>a reference state</returns>
        public State FindGlobalReferenceInModel(Model model)
        {
            if (model == null) throw new ArgumentException("model is null");
            if (model.States == null || model.States.Count == 0) throw new ArgumentException("model.States is null or empty");

            var refs = model.States.Where(v => v.Type == StateType.GlobalReference);
            return refs.Count() == 0 ? null : refs.First();
        }

        public Transition FindInTransitionToModel(Model model)
        {
            return model.StartState.Transitions.First();
        }

        public Transition FindOutTransitionToModel(Model model)
        {
            return model.StopState.Transitions.First();
        }


    }
}