using System;
using System.ComponentModel.Composition;
using SMART.Base.Statistics;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;

namespace SMART.Base.StopCriterias
{
    //[Export]
    //[Export(typeof(IGenerationStopCriteria))]
    [GenerationStopCriteria(Name = "Step Count Criteria")]
    public class StepCountGenerationStopCriteria : IGenerationStopCriteria {

        //[Config(Editor = ConfigEditor.Statistic)]
        //[Import]
        private IStatistic statistic;

        public IStatistic Statistic
        {
            get
            {
                if (statistic == null)
                    statistic = new StepCountStatistic();
                return statistic;
            }
            set { statistic = value; }
        }

        public void InternalReset()
        {
            statistic.Reset(null);
        }


        [Config]
        public double Threshold { get; set; }

        public bool ShouldStop(IModel model) {
            var val = Statistic.Calculate(model);
            return val >= Threshold;
        }
    }
}