using SMART.Core.Interfaces;

namespace SMART.Core.Interfaces
{
    public interface IBatchModel : IModel
    {
        void BeginBatch();
        void EndBatch();
        void CancelBatch();
    }
}