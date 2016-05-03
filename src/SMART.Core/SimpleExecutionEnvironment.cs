using System.Collections.Generic;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

namespace SMART.Core
{
    public class SimpleExecutionEnvironment : IExecutionEnvironment
    {
        public IEnumerable<Transition> GetOutTransitions(State state)
        {
            return state.OutTransitions;
        }

        public bool CheckCriteria(IModel model, IEnumerable<IGenerationStopCriteria> criterias)
        {
            bool check = true;

            foreach (var c in criterias)
            {
                check &= c.ShouldStop(model);
            }

            return check;
        }
    }
}