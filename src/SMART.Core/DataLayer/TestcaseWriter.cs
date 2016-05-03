using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;

namespace SMART.Core.DataLayer {
    public class TestcaseWriter : ITestcaseWriter {
        public void Save(Stream stream, ITestcase testCase) {
            var settings = new XmlWriterSettings { Indent = true, CloseOutput = false };
            var xmlWriter = XmlWriter.Create(stream, settings);
            if (xmlWriter == null) return;

            XDocument doc = GetDoc(testCase);
            
            doc.Save(xmlWriter);
            xmlWriter.Close();
            
        }

        private static XDocument GetDoc(ITestcase testCase)
        {
            return new XDocument(new XDeclaration("1.0", "UTF-8", "no"),
                                 new XElement("smart",
                                              new XElement("testcase",
                                                           new XAttribute("id", testCase.Id.ToString()),
                                                           CreateXElementForModels(testCase),
                                                           CreateXElementForAdapters(testCase),
                                                           CreateXElementForAlgorithms(testCase),
                                                           CreateXElementForGenerationStopCriteria(testCase),
                                                           CreateXElementForExecutionStopCriteria(testCase),
                                                           testCase.GetXConfig()
                                                  )
                                     ));
        }

        private static XElement CreateXElementForExecutionStopCriteria(ITestcase testCase) {
            return new XElement("executionstopcriteras",
                                from e in testCase.ExecutionStopCriteriasas
                                select new XElement(
                                    "executionstopcritera",
                                    new XAttribute("type", e.GetType().FullName),
                                    e.GetXConfig()
                                    )
                );
        }

        private static XElement CreateXElementForGenerationStopCriteria(ITestcase testCase) {
            return new XElement("generationstopcriterias",
                                from t in testCase.GenerationStopCriterias
                                select new XElement(
                                    "generationstopcriteria",
                                    new XAttribute("type", t.GetType().FullName),
                                    t.GetXConfig()
                                    )
                );
        }

        private static XElement CreateXElementForAlgorithms(ITestcase testCase) {
            return new XElement("algorithms",
                                from a in testCase.Algorithms
                                select new XElement(
                                    "algorithm",
                                    new XAttribute("type", a.GetType().FullName),
                                    a.GetXConfig()
                                    )
                );
        }

        private static XElement CreateXElementForAdapters(ITestcase testCase) {
            return new XElement("adapters",
                                from a in testCase.Adapters
                                select new XElement(
                                        "adapter",
                                        new XAttribute("type", a.GetType().FullName),
                                        a.GetXConfig()
                                    )
                                );
        }

        private static XElement CreateXElementForModels(ITestcase testCase) {
            return new XElement("models",
                                from g in testCase.Models
                                select new XElement(
                                        "model",
                                        new XAttribute("id", g.Id.ToString())
                                        //g.GetXConfig()
                                    )
                                );
        }
    }
}