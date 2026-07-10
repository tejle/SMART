using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using ICSharpCode.SharpZipLib.Zip;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using System.IO;

namespace SMART.Core.DataLayer
{
    public class ProjectReader : ReaderBase, IProjectReader
    {
        private readonly IModelReader modelReader;
        private readonly ITestcaseReader testcaseReader;
        private readonly IReportReader reportReader;

        public ProjectReader(IModelReader modelReader, ITestcaseReader testcaseReader, IReportReader reportReader)
        {
            this.modelReader = modelReader;
            this.testcaseReader = testcaseReader;
            this.reportReader = reportReader;
        }

        public IProject Load(Stream stream)
        {
            Resources.Clear();

            var projectFile = new ZipFile(stream);
            var projectStructure = projectFile.GetEntry("project.xml");
            if (projectStructure == null)
                throw new ArgumentException("Not a valid SMART project");

            var settings = new XmlReaderSettings { CloseInput = false, IgnoreWhitespace = true, IgnoreComments = true };
            using (var xmlReader = XmlReader.Create(projectFile.GetInputStream(projectStructure), settings))
            {
                var xdoc2 = XDocument.Load(xmlReader);

                var project = Configured<Project>(xdoc2.Descendants("project").First());

                project.Models = xdoc2.Descendants("model")
                    .Attributes("id")
                        .Select(id =>
                                    {
                                        var path = string.Format("models/{0}.xml", id.Value);
                                        return modelReader.Load(GetInputStream(projectFile, path));
                                    });

                project.Testcases = xdoc2.Descendants("testcase")
                    .Attributes("id")
                        .Select(id =>
                                    {
                                        var path = string.Format("testcases/{0}.xml", id.Value);
                                        return testcaseReader.Load(GetInputStream(projectFile, path));
                                    });

                project.Reports = xdoc2.Descendants("report")
                    .Attributes("id")
                        .Select(id =>
                        {
                            var path = string.Format("reports/{0}.xml", id.Value);
                            return reportReader.Load(GetInputStream(projectFile, path));
                        });

                return project;
            }
        }

        private static Stream GetInputStream(ZipFile projectFile, string path)
        {
            return projectFile.GetInputStream(projectFile.GetEntry(path));
        }
    }
}
