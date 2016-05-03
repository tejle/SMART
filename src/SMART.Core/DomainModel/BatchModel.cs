using System;
using System.Collections.Generic;
using SMART.Core.Interfaces;

namespace SMART.Core.DomainModel
{
    public class BatchModel : ModelDecorator, IBatchModel
    {
        private bool isInBatchMode;
        private List<Action> batchAddStateActions;
        private List<Action> batchAddTransitionActions;
        private List<Action> batchRemoveStateActions;
        private List<Action> batchRemoveTransitionActions;
        
        public BatchModel(IModel model) : base(model)
        {
            isInBatchMode = false;
        }

        public void BeginBatch() {
            batchAddStateActions = new List<Action>();
            batchAddTransitionActions = new List<Action>();
            batchRemoveStateActions = new List<Action>();
            batchRemoveTransitionActions = new List<Action>();
            isInBatchMode = true;
        }

        public void EndBatch() {
            if (!isInBatchMode) return;

            isInBatchMode = false;

            batchAddStateActions.ForEach(a => a());
            batchAddTransitionActions.ForEach(a => a());
            batchRemoveTransitionActions.ForEach(a => a());
            batchRemoveStateActions.ForEach(a => a());
        }

        public void CancelBatch() {
            if (!isInBatchMode) return;

            batchAddStateActions.Clear();
            batchAddTransitionActions.Clear();
            batchRemoveStateActions.Clear();
            batchRemoveTransitionActions.Clear();
            isInBatchMode = false;
        }

        public override IModel Add(State state) {
            if (isInBatchMode) {
                batchAddStateActions.Add(() => Add(state));
                return Model;
            }
            
            return base.Add(state);
        }

        public override IModel Add(Transition transition) {
            if (isInBatchMode) {
                batchAddTransitionActions.Add(() => Add(transition));
                return Model;
            }
            return base.Add(transition);
        }

        public override IModel Remove(State state) {
            if (isInBatchMode) {
                batchRemoveStateActions.Add(() => Remove(state));
                return Model;
            }
            return base.Remove(state);
        }

        public override IModel Remove(Transition transition) {
            if (isInBatchMode) {
                batchRemoveTransitionActions.Add(() => Remove(transition));
                return Model;
            }
            return base.Remove(transition);
        }
    }
}