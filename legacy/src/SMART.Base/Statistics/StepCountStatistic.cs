using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;
using System.ComponentModel.Composition;

namespace SMART.Base.Statistics
{
    //[Export]
    //[Export(typeof(IStatistic))]
    [Statistic(Name = "Step Count")]
    public class StepCountStatistic : IStatistic
    {
        private int internalCounter;
        public StepCountStatistic()
        {
            internalCounter = 0;
        }
        public void AfterVisit(IModelElement element)
        {
            
        }

        public void BeforeVisit(IModelElement element)
        {
            internalCounter++;
        }

        public void Reset(IModel executableModel)
        {
            internalCounter = 0;
        }

        public double Percent
        {
            get { return 0; }
        }

        public void OnDefectDetected(IModelElement element)
        {
            
        }

        public double Calculate(IModel model)
        {
            return internalCounter;
        }
    }
}