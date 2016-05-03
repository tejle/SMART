using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.Interfaces.Reporting;

namespace SMART.Core.DataLayer
{
    public class ReportWriter : WriterBase, IReportWriter
    {
        public void Save(Stream stream, IReport report)
        {
            var settings = new XmlWriterSettings { Indent = true, CloseOutput = false };
            var xmlWriter = XmlWriter.Create(stream, settings);
            if (xmlWriter == null) return;

            var doc = GetDoc(report);
            doc.Save(xmlWriter);
            xmlWriter.Close();
        }

        private static XDocument GetDoc(IReport report)
        {
            return new XDocument(new XDeclaration("1.0", "UTF-8", "no"),
                                 new XElement("smart", GetReport(report)));
        }

        private static XElement GetReport(IReport report)
        {
            return new XElement("report",
                                new XAttribute("id", report.Id.ToString()),
                                GetScenario(report),
                                report.GetXConfig()
                );
        }

        private static XElement GetScenario(IReport report)
        {
            if (report.Scenario == null) return null;
            return new XElement("scenario", new XAttribute("id", report.Scenario.Id),
                GetModels(report),
                GetStates(report),
                GetTransitions(report),
                GetAlgorithms(report),
                GetAdapters(report),
                GetGenerationStopCriterias(report),
                GetExecutionStopCriterias(report),
                GetDefectFlows(report),
                report.Scenario.GetXConfig()
                                                                    );
        }

        private static XElement GetDefectFlows(IReport report)
        {
            if (report.Scenario.DefectFlows == null) return null;
            return new XElement("defectflows", from a in report.Scenario.DefectFlows
                                               let flow = from e in a select e
                                                   select new XElement("defectflow", from g in flow select new XElement("id", g)));
        }

        private static XElement GetModels(IReport report)
        {
            if (report.Scenario.Models == null) return null;
            return new XElement("models", from a in report.Scenario.Models
                                          select new XElement("model", a.GetXConfig()));
        }

        private static XElement GetStates(IReport report)
        {
            if (report.Scenario.States == null) return null;
            return new XElement("states", from a in report.Scenario.States
                                          select new XElement("state", new XAttribute("id", a.Id), a.GetXConfig()));
        }

        private static XElement GetTransitions(IReport report)
        {
            if (report.Scenario.Transitions == null) return null;
            return new XElement("transitions", from a in report.Scenario.Transitions
                                               select new XElement("transition", new XAttribute("id", a.Id),
                                                 new XElement("parameters", from p in a.Parameters select new XElement("parameter", p)),
                                                 a.GetXConfig()));
        }

        private static XElement GetAlgorithms(IReport report)
        {
            if (report.Scenario.Algorithms == null) return null;
            return new XElement("algorithms", from a in report.Scenario.Algorithms
                                              select new XElement("algorithm", GetSettings(a.Settings), a.GetXConfig()));
        }

        private static XElement GetAdapters(IReport report)
        {
            if (report.Scenario.Adapters == null) return null;
            return new XElement("adapters", from a in report.Scenario.Adapters
                                            select new XElement("adapter", GetSettings(a.Settings), a.GetXConfig()));
        }

        private static XElement GetGenerationStopCriterias(IReport report)
        {
            if (report.Scenario.GenerationStopCriterias == null) return null;
            return new XElement("generationstopcriterias", from a in report.Scenario.GenerationStopCriterias
                                                           select new XElement("generationstopcriteria", GetSettings(a.Settings), a.GetXConfig()));
        }

        private static XElement GetExecutionStopCriterias(IReport report)
        {
            if (report.Scenario.ExecutionStopCriterias == null) return null;
            return new XElement("executionstopcriterias", from a in report.Scenario.ExecutionStopCriterias
                                                          select new XElement("executionstopcriteria", GetSettings(a.Settings), a.GetXConfig()));
        }

        private static XElement GetSettings(IEnumerable<IReportConfigSetting> settings)
        {
            if (settings == null) return null;
            return new XElement("settings", from s in settings
                                            select new XElement("setting", s.GetXConfig()));
        }
    }
}
