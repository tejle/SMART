//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4913
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Text;

using System.Collections.Generic;
using WatiN.Core;
using System.Threading;
namespace SMART.Website
{


    public class systemverification : IDisposable
    {
        Dictionary<string, string> parameterLinkMap = new Dictionary<string, string>();

        public event EventHandler DefectDetected;

        private string home = "http://www.systemverification.com/sv_swe/";

        private FireFox browser;

        public systemverification()
        {


        }

        private void setupLinks()
        {
            parameterLinkMap.Add("start", "start");
            parameterLinkMap.Add("branscher", "branscher");
            parameterLinkMap.Add("kunder", "kunder");
            parameterLinkMap.Add("om_oss", "om oss");
            parameterLinkMap.Add("tjanster", "tjanster");
            parameterLinkMap.Add("nyhetsarkiv", "nyhetsarkiv");
            parameterLinkMap.Add("jobbahososs", "jobb");
            parameterLinkMap.Add("kontaktaoss", "kontakt");
            parameterLinkMap.Add("IT_Telekom", "it_telekom");
            parameterLinkMap.Add("Lakemedel", "lakemedel");
            parameterLinkMap.Add("Industri", "industri");
            parameterLinkMap.Add("Bank_Finans", "bank_finans");
            parameterLinkMap.Add("Foretagskultur", "foretagskultur");
            parameterLinkMap.Add("Kontaktpersoner", "kontaktpersoner");
            parameterLinkMap.Add("Stockholm", "stockholm");
            parameterLinkMap.Add("Goteborg", "goteborg");
            parameterLinkMap.Add("Malmo", "malmo");
            parameterLinkMap.Add("Utbildning", "utbildning");
            parameterLinkMap.Add("Kvalitetssakring", "Kvalitetssäkring");
            parameterLinkMap.Add("TPIundersokning", "TPI");
            parameterLinkMap.Add("Testautomation", "Test automation");
            parameterLinkMap.Add("Konsulttjanster", "Konsulttjanster");
            parameterLinkMap.Add("Outsourcing", "outsourcing");
            parameterLinkMap.Add("Styrelse", "styrelse");
            parameterLinkMap.Add("Affarside", "affarside");

        }

        private void init()
        {
            parameterLinkMap.Clear();
            setupLinks();
            browser = new FireFox(home);

        }

        // Reset the adapter, used from smart when going back to the start state

        public virtual void Reset()
        {
            browser.GoTo(home);
        }


        public virtual void e_click_Sida(params string[] parameters)
        {
            if (browser == null)
                init();
            else
                FindAndClick(parameterLinkMap[parameters[0]]);


        }

        public virtual void e_click(params string[] parameters)
        {
            FindAndClick(parameterLinkMap[parameters[0]]);
        }

        private void FindAndClick(string name)
        {
            browser.Image(Find.ByName(name)).Parent.Click();
            browser.WaitForComplete();
        }


        public virtual void Start_Sida()
        {
            
            if(!AssertUrl("http://www.systemverification.com/sv_swe/", true))
                AssertUrl("http://www.systemverification.com/sv_swe/index.php");

        }

        public virtual void IT_Telekom()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=it-telekom");

        }

        public virtual void Lakemedel()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=lakemedel");
        }

        public virtual void Industri()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=industri");
        }

        public virtual void Bank_Finans()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=branscher");
        }

        public virtual void Kunder_Sida()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=kunder");
        }

        public virtual void Affarside()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=om-oss");
        }

        public virtual void Foretagskultur()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=foretagskultur");
        }

        public virtual void Styrelse()
        {
            throw new Exception("Did not find Daud as a member of the board... !");
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=styrelse");
        }

        public virtual void Konsulttjanster()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=tjanster");
        }

        public virtual void Testautomation()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=testautomation");
        }

        public virtual void Outsourcing()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=outsourcing");
        }

        public virtual void TPIundersokning()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=tpi");
        }

        public virtual void Kvalitetssakring()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=kvalitet");
        }

        public virtual void Utbildning()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=utbildning");
        }

        public virtual void Nyhetsarkiv_Sida()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=nyhetsarkiv");
        }

        public virtual void JobbaHosOss_Sida()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=jobba-hos-oss");
        }

        public virtual void Kontaktpersoner()
        {

            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=kontakta-oss");
        }

        public virtual void Malmo()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=kontakt-malmo");

        }

        public virtual void Goteborg()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=kontakt-gbg");
        }

        public virtual void Stockholm()
        {
            AssertUrl("http://www.systemverification.com/sv_swe/index.php?s=kontakt-stockholm");
        }

        private void AssertUrl(string url) { AssertUrl(url, false); }

        private bool AssertUrl(string url, bool dontThrow)
        {
            Thread.Sleep(100);
            if (browser.Url.Equals(url))
                return true;

            InvokeDefectDetected(EventArgs.Empty);
            if (!dontThrow)
                throw new Exception("wrong page " + browser.Url + " : Expected " + url);

            return false;
        }

        public void Dispose()
        {
            if (browser != null)
                browser.Dispose();

            browser = null;
        }

        private void InvokeDefectDetected(EventArgs e)
        {
            EventHandler detected = DefectDetected;
            if (detected != null) detected(this, e);
        }
    }
}
