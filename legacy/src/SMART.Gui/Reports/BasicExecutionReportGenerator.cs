using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using SMART.Core.Workflow;
using SMART.Gui.ViewModel;
using SMART.IOC;

namespace SMART.Gui.Reports
{

    public class BasicExecutionReportGenerator
    {
        //Constants for Avery Address Label 5160
        private const double PAPER_SIZE_WIDTH = 794; //210mm x 96dpi
        private const double PAPER_SIZE_HEIGHT = 1122; //297mm x 96dpi

        private const double SIDE_MARGIN = 75; //20mm x 96dpi
        private const double TOP_MARGIN = 38; //10mm x 96dpi
        private const double HORIZONTAL_GAP = 12.48; //0.13" x 96

        private static FixedPage CreatePage()
        {
            //Create new page
            var page = new FixedPage();
            //Set background
            page.Background = Brushes.White;
            //Set page size (Letter size)
            page.Width = PAPER_SIZE_WIDTH;
            page.Height = PAPER_SIZE_HEIGHT;            
            return page;
        }

        private static PageContent CreatePageContent(ReportViewModel report, Type pageTemplate)
        {
            var page = new PageContent();
            var fixedPage = CreatePage();

            UserControl content = null;
            switch (pageTemplate.Name)
            {
                case "BasicExecutionReport":
                    content = new BasicExecutionReport(report);
                    break;
                case "BasicExecutionReportDetails":
                    content = new BasicExecutionReportDetails(report);
                    break;
                case "BasicExecutionReportFlows":
                    content = new BasicExecutionReportFlows(report);
                    break;
            }

            if (content != null)
            {                                
                FixedPage.SetLeft(content, SIDE_MARGIN);
                FixedPage.SetTop(content, TOP_MARGIN);

                //Add report object to page
                fixedPage.Children.Add(content);
            }

            //Invoke Measure(), Arrange() and UpdateLayout() for drawing
            fixedPage.Measure(new Size(PAPER_SIZE_WIDTH, PAPER_SIZE_HEIGHT));
            fixedPage.Arrange(new Rect(new Point(), new Size(PAPER_SIZE_WIDTH, PAPER_SIZE_HEIGHT)));
            fixedPage.UpdateLayout();

            ((IAddChild)page).AddChild(fixedPage);
            return page;
        }

        public FixedDocument CreateDocument(ReportViewModel report)
        {
            //Create new document
            var doc = new FixedDocument();
            //Set page size
            doc.DocumentPaginator.PageSize = new Size(PAPER_SIZE_WIDTH, PAPER_SIZE_HEIGHT);
            
            //Create 1st page
            var page = CreatePageContent(report, typeof (BasicExecutionReport));
            doc.Pages.Add(page);

            //Create 2nd page
            page = CreatePageContent(report, typeof (BasicExecutionReportDetails));
            doc.Pages.Add(page);

            // Create 3rd page
            page = CreatePageContent(report, typeof (BasicExecutionReportFlows));
            doc.Pages.Add(page);

            return doc;
        }

        public FixedDocumentSequence CreateDocumentAsXps(ReportViewModel report)
        {
            var fixedDocument = CreateDocument(report);

            var ms = new MemoryStream(); 
            var documentUri = new Uri("pack://document.xps"); 
            var p = Package.Open(ms, FileMode.Create, FileAccess.ReadWrite); 
            PackageStore.AddPackage(documentUri, p);
            var xpsDocument = new XpsDocument(p, CompressionOption.NotCompressed, documentUri.AbsoluteUri);
            var dw = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
            dw.Write(fixedDocument);             
            var fixedDocumentSequence = xpsDocument.GetFixedDocumentSequence(); 
            if (fixedDocumentSequence == null)
            {
                return null;
            }
            return fixedDocumentSequence;
        }
    }

}
