using System.Linq;
using SMART.Core.Interfaces.Factories;

namespace SMART.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Composition;

    using Interfaces;
    using Interfaces.Repository;

    using Metadata;
    using SMART.IOC;
    using SMART.Base.Adapters;

    public class AdapterRepository : IAdapterRepository
    {
     
        private List<ClassDescription> classes = new List<ClassDescription>()
                                                     {
                                                         new ClassDescription()
                                                             {
                                                                 Description =
                                                                     "Assembly adapter that executes against a .Net Assembly",
                                                                 Name = "Assembly Adapter",
                                                                 Type = typeof (SMART.Base.Adapters.AssemblyAdapter)
                                                             },
                                                             new ClassDescription()
                                                             {
                                                                 Description =
                                                                     "Console adapter prints the testsequence to the console",
                                                                 Name = "Console Adapter",
                                                                 Type = typeof (SMART.Base.Adapters.ConsoleAdapter)
                                                             },
                                                             new ClassDescription()
                                                             {
                                                                 Description =
                                                                     "FlatFile adapter prints the testsequence to a file",
                                                                 Name = "Flat file Adapter",
                                                                 Type = typeof (SMART.Base.Adapters.FlatFileAdapter)
                                                             }
                                                     };

        public IEnumerable<ClassDescription> GetAll()
        {
            return from c in classes select c;
        }
    }

    public class AdapterFactory : IAdapterFactory
    {
        public IAdapter Create(Type type)
        {
            return Resolver.ResolveType(type) as IAdapter;
        }
    }
}