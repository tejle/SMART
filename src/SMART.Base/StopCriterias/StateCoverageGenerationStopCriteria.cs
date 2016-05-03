using System;
using System.ComponentModel.Composition;
using SMART.Base.Statistics;
using SMART.Core;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;

namespace SMART.Base.StopCriterias
{
    //[Export]
    //[Export(typeof(IGenerationStopCriteria))]
    [GenerationStopCriteria(Name = "State Coverage Criteria")]
    public class StateCoverageGenerationStopCriteria : IGenerationStopCriteria
    {
        //[Config(Editor = ConfigEditor.Statistic)]
        //[Import]
        private IStatistic statistic;

        public IStatistic Statistic
        {
            get
            {
                if (statistic == null)
                    statistic = new StateCoverageStatistic();
                return this.statistic;
            }
            set { this.statistic = value; }
        }

        public void InternalReset()
        {
            
        }


        [Config(Editor = ConfigEditor.Percent)]
        public double Threshold { get; set;}

        public bool ShouldStop(IModel model)
        {
            var val = Statistic.Calculate(model);
            return val >= Threshold;
        }
    }
}