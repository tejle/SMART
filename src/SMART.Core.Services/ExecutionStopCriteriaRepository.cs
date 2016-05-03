using SMART.Core.Interfaces.Repository;

namespace SMART.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Metadata;

    public class ExecutionStopCriteriaRepository : IExecutionStopCriteriaRepository{

        private readonly List<ClassDescription> list = new List<ClassDescription>()
                                                           {
                                                                   new ClassDescription()
                                                                       {
                                                                               Description = "Time based stop criteria",
                                                                               Name = "Time Based Criteria",
                                                                               Type = typeof(SMART.Base.StopCriterias.TimeBasedStopCriteria)
                                                                       }
                                                           };
        public IEnumerable<ClassDescription> GetAll()
        {
            return from s in this.list select s;
            
        }
    }
}