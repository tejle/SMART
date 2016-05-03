using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;


namespace SMART.Base.Statistics
{
    //[Export]
    //[Export(typeof(IStatistic))]
    [Statistic(Name = "State Coverage")]
    public class StateCoverageStatistic : IStatistic
    {
        private int total;
        private int current;

        public void AfterVisit(IModelElement element)
        {
            if (element is State && element.VisitCount == 1)
                current++;
        }

        public void BeforeVisit(IModelElement element)
        {
        }

        public void Reset(IModel executableModel)
        {
            if (executableModel != null)
            {
                total = executableModel.States.Count();
            }
            current = 0;
        }

        public double Percent
        {
            get { return (double)current / total; }
        }

        public void OnDefectDetected(IModelElement element)
        {

        }

        public double Calculate(IModel model)
        {
            IEnumerable<State> states = model.States.Where(s => (s.GetType() !=typeof(StartState)) && (s.GetType() != typeof(StopState)));
            var visited = states.Where(s => s.VisitCount > 0).Count();
            int c =  states.Count();
            return (double) visited/c;
        }
    }
}
