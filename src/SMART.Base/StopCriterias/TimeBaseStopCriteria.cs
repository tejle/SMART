using System.ComponentModel.Composition;
using SMART.Core;
using System;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;

namespace SMART.Base.StopCriterias
{
    //[Export]
    //[Export(typeof(IExecutionStopCriteria))]
    [ExecutionStopCriteria(Name = "TimeBased")]
    public class TimeBasedStopCriteria : IExecutionStopCriteria
    {
        private DateTime? startTime;
        private DateTime StartTime {
            get {
                if (!startTime.HasValue)
                    startTime = SystemTime.Now();
                return startTime.Value;
            }
        }
        [Config]
        public TimeSpan Threshold { get; set; }

        public bool ShouldStop(IModel model)
        {
            var span = SystemTime.Now().Subtract(StartTime);
            return span >= Threshold;
        }

        public void Init()
        {
            startTime = SystemTime.Now();
        }


    	public TimeSpan Limit { get; set; }



        public void Reset() {
            startTime = SystemTime.Now(); //new DateTime?();
        }
    

        public bool Complete {
            get {
                var span = SystemTime.Now().Subtract(StartTime);
                return span >= Limit;
            }
        }

        public double PercentComplete {
            get {
                var span = SystemTime.Now().Subtract(StartTime);
                var percentComplete = (double)span.Ticks / Limit.Ticks;
                return percentComplete <= 1 ? percentComplete : 1;
            }
        }
    }

}