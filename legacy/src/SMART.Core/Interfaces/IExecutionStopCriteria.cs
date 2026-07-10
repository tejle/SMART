using SMART.Core.DomainModel;

namespace SMART.Core.Interfaces
{
    public interface IExecutionStopCriteria {
        bool ShouldStop(IModel model);
        void Init();
		

    }
}