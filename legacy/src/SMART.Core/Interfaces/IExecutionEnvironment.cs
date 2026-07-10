using System.Collections.Generic;
using SMART.Core.Interfaces;

namespace SMART.Core.DomainModel
{
    public interface IExecutionEnvironment {
        IEnumerable<Transition> GetOutTransitions(State state);
        bool CheckCriteria(IModel model, IEnumerable<IGenerationStopCriteria> criterias);
    }
}