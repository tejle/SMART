
using System.Linq;
using SMART.Core.Interfaces.Factories;
using SMART.IOC;

namespace SMART.Core.Services
{
    using System.Collections.Generic;

    using Interfaces;
    using Interfaces.Repository;

    using Metadata;
    using System;

    public class AlgorithmRepository : IAlgorithmRepository
    {

        private List<ClassDescription> classes = new List<ClassDescription>()
                                                     {
                                                         new ClassDescription()
                                                             {
                                                                 Description = "Assisted Random Algorithm",
                                                                 Name = "Assisted Random",
                                                                 Type =
                                                                     typeof (
                                                                     SMART.Base.Algorithms.AssistedRandomAlgorithm)

                                                             },
                                                         new ClassDescription()
                                                             {
                                                                 Description = "Random Algorithm",
                                                                 Name = "Random",
                                                                 Type =
                                                                     typeof (
                                                                     SMART.Base.Algorithms.RandomAlgorithm)

                                                             }
                                                         //    ,
                                                         //new ClassDescription()
                                                         //    {
                                                         //        Description = "Breadth first Algorithm",
                                                         //        Name = "Breadth-first",
                                                         //        Type =
                                                         //            typeof (SMART.Base.Algorithms.BreadthFirstAlgorithm
                                                         //            )
                                                         //    }
                                                     };


        public IEnumerable<ClassDescription> GetAll()
        {

            return from c in classes select c;
        }

    }

    public class AlgorithmFactory : IAlgorithmFactory
    {
        
        public IAlgorithm Create(Type type)
        {
            return Resolver.ResolveType(type) as IAlgorithm;
        }
    }
}