using System.Collections.Generic;
using System.Linq;
using SMART.Core.Interfaces.Repository;
using SMART.Core.Metadata;

namespace SMART.Core.Services
{
    public class StatisticsRepository : IStatisticsRepository 
    {
        private readonly List<ClassDescription> list = new List<ClassDescription>()
                                                           {
                                                               new ClassDescription()
                                                                   {
                                                                       Description = "State Coverage of the model",
                                                                       Name = "State Coverage",
                                                                       Type = typeof(SMART.Base.Statistics.StateCoverageStatistic)
                                                                   },
                                                                   new ClassDescription()
                                                                   {
                                                                       Description = "Step count for the testcase",
                                                                       Name = "Step count",
                                                                       Type = typeof(SMART.Base.Statistics.StepCountStatistic)
                                                                   }
                                                           };

        public IEnumerable<ClassDescription> GetAll()
        {
            return from s in list select s;
        }
    }
}