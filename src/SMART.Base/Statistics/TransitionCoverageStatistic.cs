using System.ComponentModel.Composition;
using System.Linq;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;


namespace SMART.Base.Statistics
{
    //[Export]
    //[Export(typeof(IStatistic))]
    [Statistic(Name = "Transition Coverage")]
    public class TransitionCoverageStatistic : IStatistic
    {
        private int total;
        private int current;

        public void AfterVisit(IModelElement element)
        {
            if (element is Transition && element.VisitCount == 1)
                current++;
        }

        public void BeforeVisit(IModelElement element)
        {
        }

        public void Reset(IModel executableModel)
        {
            if (executableModel != null)
            {
                total = executableModel.Transitions.Count();
            }
            current = 0;
        }

        public double Percent
        {
            get { return (double)current /total; }
        }

        public void OnDefectDetected(IModelElement element)
        {

        }

        public double Calculate(IModel model)
        {
            var numberofvisited = model.Transitions.Where(t => t.VisitCount > 0).Count();
            var totalTrans = model.Transitions.Count;
            return (double) numberofvisited/totalTrans;

        }
    }
}
