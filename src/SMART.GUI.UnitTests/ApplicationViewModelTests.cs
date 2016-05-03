using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Moq;
using NUnit.Framework;
using SMART.Core.DataLayer;
using SMART.Gui.ViewModel;
using SMART.Core.BusinessLayer;
using SMART.Core.Model.ProjectStructure;
using System.Threading;
using System.ComponentModel;

namespace SMART.GUI.UnitTests
{
    [TestFixture]
    public class ApplicationViewModelTests
    {
        Mock<IProject> iProj;
        Mock<ProjectManager> projMan;
        Mock<IProjectReader> projectReader;
        Mock<IProjectWriter> projectWriter;
        Mock<ProjectIOHandler> projectIO;
        Mock<ApplicationManager> appMan;
        ApplicationViewModel apw;
        Mock<ProjectViewModel> projView;

        [SetUp]
        public void setup()
        {
            iProj = new Mock<IProject>();
            projectReader = new Mock<IProjectReader>();
            projectWriter = new Mock<IProjectWriter>();
            projectIO = new Mock<ProjectIOHandler>(projectReader.Object, projectWriter.Object);

            appMan = new Mock<ApplicationManager>(projectIO.Object);
            projMan = new Mock<ProjectManager>(iProj);

            apw = new ApplicationViewModel(appMan.Object);
            projView = new Mock<ProjectViewModel>(apw,projMan);

        }
        
        [Test]
        public void create_a_application_view_model()
        {
            Assert.AreSame(appMan.Object, apw.Manager);
        }


        //[Test]
        //public void testthismethod()
        //{
        //    appMan.SetupGet(a=>a.Name).Returns("test").Verifiable();
        //    var str = apw.TestThis();
        //    Assert.AreEqual("test", str);
        //    appMan.Verify();
        //}
    }

    /// <summary>
    /// Double för BackgroundWorker
    /// </summary>
    public class BackDouble : BackgroundWorker
    {
        
        public BackDouble(): base()
        {
        }

        protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e)
        {
            //base.OnRunWorkerCompleted(e);
            //Just chill man.
        }

        protected override void OnDoWork(DoWorkEventArgs e)
        {
            //base.OnDoWork(e);
            //just chill man.

        }

        public new void RunWorkerAsync(object argument)
        {
            //chill for now
        }
  

    }


}