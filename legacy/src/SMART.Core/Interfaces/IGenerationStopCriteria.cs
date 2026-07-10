namespace SMART.Core.Interfaces
{
    public interface IGenerationStopCriteria {
        bool ShouldStop(IModel model);
        IStatistic Statistic { get; }
        void InternalReset();
    }
}