using System.ComponentModel.Composition;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;

namespace SMART.Base.StopCriterias
{
    [Export]
    [Export(typeof(IStopCriteria))]
    [StopCriteria(Name = "Statistical")]
    public class StatisticalStopCriteria : IStopCriteria
    {
        [Config(Editor = ConfigEditor.Statistic)]
        public IStatistic Statistic { get; set; }
        
        [Config]
        public double StopLimit { get; set; }

        public bool Complete
        {
            get { return PercentComplete >= 1; }
        }

        public double PercentComplete
        {
            get { return StopLimit == 0 ? 1 : Statistic.Percent / StopLimit; }
        }

        public void Reset()
        {
            
        }
    }

}
