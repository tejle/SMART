using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Factories;
using SMART.Core.Interfaces.Repository;
using SMART.Core.Metadata;
using SMART.IOC;


namespace SMART.Core.DataLayer
{
    public class TestcaseReader : ReaderBase, ITestcaseReader
    {
        private readonly IAlgorithmRepository algorithmRepository;
        private readonly IAdapterRepository adapterRespository;
        
        public ITestcase Load(Stream stream)
        {
            var settings = new XmlReaderSettings { CloseInput = false, IgnoreWhitespace = true, IgnoreComments = true };
            using (var xmlReader = XmlReader.Create(stream, settings))
            {
                var xdoc2 = XDocument.Load(xmlReader);

                Testcase testcase = GetTestcase(xdoc2);

                return testcase;
            }
        }

        private static Testcase GetTestcase(XDocument xdoc2)
        {
            var testcase = Configured<Testcase>(xdoc2.Descendants("testcase").First());

            testcase.Models = 
                xdoc2.Descendants("model").Attributes("id").Select(
                    id => (IModel)Resources[new Guid(id.Value)]);
            
            var adapterList = Get(Resolver.Resolve<IAdapterRepository>(), Resolver.Resolve<IAdapterFactory>());
            var generationList = Get(Resolver.Resolve<IGenerationStopCriteriaRepository>(), Resolver.Resolve<IGenerationStopCriteriaFactory>());
            var executionList = Get(Resolver.Resolve<IExecutionStopCriteriaRepository>(),Resolver.Resolve<IExecutionStopCriteriaFactory>());
            var algorithmList = Get(Resolver.Resolve<IAlgorithmRepository>(),Resolver.Resolve<IAlgorithmFactory>());
            
            testcase.Adapters = ConfiguredImport(xdoc2.Descendants("adapter"), adapterList);
            testcase.GenerationStopCriterias =
                ConfiguredImport(xdoc2.Descendants("generationstopcriteria"), generationList);
            testcase.ExecutionStopCriteriasas =
                ConfiguredImport(xdoc2.Descendants("executionstopcritera"), executionList);
            testcase.Algorithms = ConfiguredImport(xdoc2.Descendants("algorithm"), algorithmList);
            
            return testcase;

            //Todo Add Sandbox in config
            //testcase.Sandbox = AvailableSandBoxes.First().GetExportedObject();
        }

        private static List<T> Get<T>(IRepository repository, IFactory<T> factory)
        {
            var retval = new List<T>();
            var list = repository.GetAll();
            foreach (var description in list)
            {
                retval.Add(factory.Create(description.Type));
            }

            return retval;
        }
    }

}