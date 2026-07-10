using System.Linq;
using System.ComponentModel.Composition;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;


namespace SMART.Base.Statistics
{
    [Export]
    [Export(typeof(IStatistic))]
    [Statistic(Name = "Defect Coverage")]
    public class DefectStatistic : IStatistic
    {
        int defects;
        int totalStates;
        
        public void AfterVisit(IModelElement element)
        {
        }

        public void BeforeVisit(IModelElement element)
        {
        }

        public void Reset(IModel executableModel)
        {
            if (executableModel != null)
            {
                totalStates = executableModel.Transitions.Count() + executableModel.States.Count();
            }
        }

        public double Percent
        {
            get { return (double) defects / totalStates; }
        }

        public void OnDefectDetected(IModelElement element)
        {
            defects++;
        }

        public double Calculate(IModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}
