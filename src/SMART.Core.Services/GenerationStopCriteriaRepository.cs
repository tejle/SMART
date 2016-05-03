namespace SMART.Core.Services
{
    using System.Linq;
    using System.Collections.Generic;

    using Base.StopCriterias;
    using Interfaces.Repository;

    using Metadata;

    public class GenerationStopCriteriaRepository : IGenerationStopCriteriaRepository
    {
     
        private readonly List<ClassDescription> classes = new List<ClassDescription>()
                                                     {
                                                         new ClassDescription()
                                                             {
                                                                 Description =
                                                                     "Stops when n % of the states has been covered",
                                                                 Name = "State Coverage Criteria",
                                                                 Type = typeof (SMART.Base.StopCriterias.StateCoverageGenerationStopCriteria)
                                                             },
                                                                new ClassDescription()
                                                             {
                                                                 Description =
                                                                     "Stops when n % of the transitions has been covered",
                                                                 Name = "Transition Coverage Criteria",
                                                                 Type = typeof (SMART.Base.StopCriterias.TransitionCoverageGenerationStopCriteria)
                                                             },
                                                                new ClassDescription()
                                                             {
                                                                 Description =
                                                                     "Stops when #n of steps has been made",
                                                                 Name = "Step Count Criteria",
                                                                 Type = typeof (SMART.Base.StopCriterias.StepCountGenerationStopCriteria)
                                                             }
                                                     };

        public IEnumerable<ClassDescription> GetAll()
        {
            return from c in classes select c;
        }
    }
}