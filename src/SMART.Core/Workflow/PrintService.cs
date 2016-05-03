using System;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.IO;
using Microsoft.Win32;
using SMART.Core.Interfaces.Reporting;

namespace SMART.Core.Workflow
{
    public class PrintService
    {
        public void Print(IReport report)
        {
            XDocument doc = CreateXML(report);
            MessageBoxResult result = MessageBox.Show("Save report to file?", "Report", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                SaveFileDialog fd = new SaveFileDialog()
                                        {
                                            DefaultExt = "*.xml",
                                            InitialDirectory = System.Reflection.Assembly.GetExecutingAssembly().Location
                                        };

                fd.ShowDialog();
                
                StreamWriter writer = new StreamWriter(fd.FileName);
                writer.Write(doc.ToString());
                writer.Flush();
                writer.Close();
                
            }
        }

        private XDocument CreateXML(IReport report)
        {
            var doc = new XDocument(new XDeclaration("1.0", "ISO-8859-1", "yes")
                //new XElement("?xml-stylesheet type=\"text/xsl\" href=\"smartreport.xsl\""),
                //new XElement("report",
                //             from s in report.Scenarios
                //             select new XElement(s.Name.Replace(' ', '_'),
                //                                 new XElement("Status",
                //                                              (s.DefectStates.Count() == 0 &&
                //                                               s.DefectTransitions.Count() == 0)
                //                                                  ? "Passed"
                //                                                  : "Failed"),
                //                                 new XElement("Id", s.Id),
                //                                 new XElement("ResponsibleTester", report.ResponsibleTester),
                //                                 new XElement("ElapsedTime", string.Format("{0:D2}:{1:D2}:{2:D2}",
                //                                                            report.TotalElapsedTime.Hours, 
                //                                                            report.TotalElapsedTime.Minutes, 
                //                                                            report.TotalElapsedTime.Seconds)))
                //    )
                );

            return doc;
        }
    }
}