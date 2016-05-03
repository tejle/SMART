using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArtOfTest.WebAii.Controls.HtmlControls;
using ArtOfTest.WebAii.Core;
using ArtOfTest.WebAii.TestTemplates;

namespace SMART.Demo.TestModel
{
    public class WebTest : BaseNUnitTest, IDisposable //, IWebTest
{
    // Methods
    public WebTest()
    {
        base.Initialize();
        base.Manager.Settings.AnnotateExecution = true;
        base.Manager.Settings.AnnotationMode = AnnotationMode.All;
        base.Manager.Settings.KillBrowserProcessOnClose = true;
        base.Manager.Settings.CreateLogFile = true;
        base.Manager.Settings.LogAnnotations = true;
        base.Manager.Settings.LogLocation = "WebAii.log";
        base.Manager.Settings.BaseUrl = "http://www.systemverification.com";
        base.Manager.LaunchNewBrowser();
        base.ActiveBrowser.WaitUntilReady();
    }

    public void e_Init(params string[] parameters)
    {
        base.Manager.ActiveBrowser.NavigateTo("/sv_swe/");
        base.ActiveBrowser.WaitUntilReady();
    }

    public void e_MainMenu(params string[] parameters)
    {
        Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
        dictionary2.Add("about_us", new string[] { "name=om oss" });
        dictionary2.Add("customers", new string[] { "name=kunder" });
        dictionary2.Add("tjanster", new string[] { "name=tjanster" });
        dictionary2.Add("start", new string[] { "name=start" });
        Dictionary<string, string[]> dictionary = dictionary2;
        if (parameters[0].Equals("start"))
        {
            base.Manager.ActiveBrowser.NavigateTo("/sv_swe/");
        }
        else
        {
            base.ActiveBrowser.Find.ByAttributes<HtmlImage>(dictionary[parameters[0]]).Parent<HtmlAnchor>().Click();
        }
        base.ActiveBrowser.WaitUntilReady();
    }

    public void e_Search(params string[] parameters)
    {
        base.ActiveBrowser.Find.ByAttributes<HtmlInputText>(new string[] { "name=q" }).Text = parameters[0];
        base.ActiveBrowser.Find.ByAttributes<HtmlInputSubmit>(new string[] { "type=submit", "value=search" }).Click();
        base.ActiveBrowser.WaitUntilReady();
    }

    public void e_SubMenu(params string[] parameters)
    {
        Dictionary<string, string[]> dictionary2 = new Dictionary<string, string[]>();
        dictionary2.Add("test_automation", new string[] { "name=Test automation" });
        dictionary2.Add("outsourcing", new string[] { "name=outsourcing" });
        dictionary2.Add("values", new string[] { "name=foretagskultur" });
        Dictionary<string, string[]> dictionary = dictionary2;
        base.ActiveBrowser.Find.ByAttributes<HtmlImage>(dictionary[parameters[0]]).Parent<HtmlAnchor>().Click();
        base.ActiveBrowser.WaitUntilReady();
    }

    public void v_About_usPage()
    {
    }

    public void v_CustomersPage()
    {
    }

    public void v_OutSourcingPage()
    {
    }

    public void v_SearchPage()
    {
    }

    public void v_ServicesPage()
    {
    }

    public void v_StartPage()
    {
    }

    public void v_TestAutomationPage()
    {
    }

    public void v_ValuesPage()
    {
    }

        public void Dispose()
        {
            base.CleanUp();
            base.ActiveBrowser.Close();
        }
}
}
