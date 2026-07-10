

namespace SMART.Core.Interfaces
{
    public interface IStatistic
    {
        void AfterVisit(IModelElement element);
        void BeforeVisit(IModelElement element);
        
        void Reset(IModel model);        

        double Percent { get; }

        void OnDefectDetected(IModelElement element);
        double Calculate(IModel model);
    }
}