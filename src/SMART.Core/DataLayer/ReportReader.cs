using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.Interfaces.Reporting;
using SMART.Core.Workflow.Reporting;

namespace SMART.Core.DataLayer
{
    public class ReportReader : ReaderBase, IReportReader
    {
        public IReport Load(Stream stream)
        {
            var xmlSettings = new XmlReaderSettings { CloseInput = false, IgnoreWhitespace = true, IgnoreComments = true };
            using (var xmlReader = XmlReader.Create(stream, xmlSettings))
            {
                var xdoc2 = XDocument.Load(xmlReader);

                var report = Configured<Report>(xdoc2.Descendants("report").First());

                report.Scenario =
                    (from e in xdoc2.Descendants("scenario") select Configured<ReportScenario>(e)).FirstOrDefault();

                report.Scenario.Models =
                    (from e in xdoc2.Descendants("model") select Configured<ReportModel>(e) as IReportModel);

                report.Scenario.States =
                    (from e in xdoc2.Descendants("state") select Configured<ReportState>(e) as IReportState);

                report.Scenario.Transitions =
                    (from e in xdoc2.Descendants("transition") select Configured<ReportTransition>(e) as IReportTransition);

                report.Scenario.Adapters =
                    (from e in xdoc2.Descendants("adapter") 
                     select GetConfiguredType<ReportAdapter>(e) as IReportAdapter);

                report.Scenario.Algorithms =
                    (from e in xdoc2.Descendants("algorithm") 
                     select GetConfiguredType<ReportAlgorithm>(e) as IReportAlgorithm);

                report.Scenario.GenerationStopCriterias =
                    (from e in xdoc2.Descendants("generationstopcriteria")
                     select GetConfiguredType<ReportGenerationStopCriteria>(e) as IReportGenerationStopCriteria);

                report.Scenario.ExecutionStopCriterias =
                    (from e in xdoc2.Descendants("executionstopcriteria")                     
                     select GetConfiguredType<ReportExecutionStopCriteria>(e) as IReportExecutionStopCriteria);

              report.Scenario.DefectFlows =
                (from e in xdoc2.Descendants("defectflow")
                 select new Queue<Guid>(from g in e.Descendants("id") select new Guid(g.Value)));

                return report;
            }
        }

        private static T GetConfiguredType<T>(XElement e) where T : class
        {
            var r = Configured<T>(e) as IReportScenarioSetting;
            var settings = from s in e.Descendants("setting") select Configured<ReportConfigSetting>(s) as IReportConfigSetting;
            if (r != null)
            {
                r.Settings = settings;
                return r as T;
            }
            return null;
        }


    }
}