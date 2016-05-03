using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.Interfaces;

using System.IO;
using System.Xml.Linq;
using SMART.Core.Workflow;

namespace SMART.Core.DataLayer
{
    public class ProjectWriter : IProjectWriter
    {
        private readonly IModelWriter ModelWriter;
        private readonly ITestcaseWriter TestcaseWriter;
        private readonly IReportWriter ReportWriter;

        public ProjectWriter(IModelWriter modelWriter, ITestcaseWriter testcaseWriter, IReportWriter reportWriter)
        {
            ModelWriter = modelWriter;
            TestcaseWriter = testcaseWriter;
            ReportWriter = reportWriter;
        }

        public void Save(Stream stream, IProject project)
        {
            using (var projectFile = new ZipOutputStream(stream))
            {
                const string fileName = "project.xml";
                projectFile.PutNextEntry(new ZipEntry(fileName));
                SaveProjectStructure(project, projectFile);
                projectFile.CloseEntry();

                foreach (var testcase in project.Testcases)
                {
                    projectFile.PutNextEntry(new ZipEntry(string.Format("testcases/{0}.xml", testcase.Id)));
                    TestcaseWriter.Save(projectFile, testcase);
                    projectFile.CloseEntry();
                }
                foreach (var model in project.Models)
                {
                    projectFile.PutNextEntry(new ZipEntry(string.Format("models/{0}.xml", model.Id)));
                    ModelWriter.Save(projectFile, model);
                    projectFile.CloseEntry();
                }
                foreach (var report in project.Reports)
                {
                    projectFile.PutNextEntry(new ZipEntry(string.Format("reports/{0}.xml", report.Id)));
                    ReportWriter.Save(projectFile, report);
                    projectFile.CloseEntry();
                }
                projectFile.Finish();
            }
        }

        private static void SaveProjectStructure(IProject project, Stream projectFile)
        {
            var xmlWriter = XmlWriter.Create(projectFile, new XmlWriterSettings { Indent = true, CloseOutput = false });
            if (xmlWriter == null) return;

            XDocument doc = GetDoc(project);
            doc.Save(xmlWriter);
            xmlWriter.Close();
        }

        private static XDocument GetDoc(IProject project)
        {
            return new XDocument(
                new XDeclaration("1.0", "UTF-8", "no"),
                new XElement("smart",
                             GetProject(project)));
        }

        private static XElement GetProject(IProject project)
        {
            return new XElement("project", 
                                GetModels(project),
                                GetTestcases(project),
                                GetReports(project),
                                project.GetXConfig()
                );
        }

        private static XElement GetTestcases(IProject project)
        {
            return new XElement("testcases", from t in project.Testcases select new XElement("testcase", new XAttribute("id", t.Id)));
        }

        private static XElement GetModels(IProject project)
        {
            return new XElement("models", from g in project.Models select new XElement("model", new XAttribute("id", g.Id)));
        }

        private static XElement GetReports(IProject project)
        {
            return new XElement("reports", from r in project.Reports select new XElement("report", new XAttribute("id", r.Id)));
        }
    }
}
