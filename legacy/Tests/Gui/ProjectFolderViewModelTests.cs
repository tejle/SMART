using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Services;

namespace SMART.Test.Gui
{
    using IOC;

    using Moq;

    using NUnit.Framework;

    using SMART.Gui.ViewModel.ProjectExplorer;


    [TestFixture]
    public class ProjectFolderViewModelTests
    {

        Mock<IProject> project;
        Mock<IModelService> modelservice;
        Mock<IScenarioService> testcaseservice;
        
        ProjectFolderViewModel viewModel;
        [SetUp]
        public void Setup()
        {

            project = new Mock<IProject>();
            testcaseservice = new Mock<IScenarioService>();
            modelservice = new Mock<IModelService>();
            viewModel = new ProjectFolderViewModel(project.Object, testcaseservice.Object, modelservice.Object);
        }

        [Test]
        public void upon_creation_commands_and_collections_should_not_be_null()
        {            
            Assert.IsNotNull(viewModel.AddModel);
            Assert.IsNotNull(viewModel.AddTestcase);
            Assert.IsNotNull(viewModel.Rename);
            Assert.IsNotNull(viewModel.FolderViewModels);
        }

        [Test]
        public void rename_should_rename_the_project()
        {            
            
            project.SetupProperty(p => p.Name, "old name");
            
            viewModel.Rename.OnExecute.Invoke(null);
            Assert.AreEqual("nytt namn", viewModel.Name);
        }

        [Test]
        public void add_model_should_call_into_project_and_add_a_model()
        {
            project.Setup(p => p.AddModel(It.IsAny<Model>())).Returns(true).Verifiable();
            
            viewModel.AddModel.OnExecute.Invoke(null);
            project.Verify();
        }

    }
}
