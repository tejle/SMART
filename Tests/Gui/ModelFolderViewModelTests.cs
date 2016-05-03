using System;
using Moq;
using NUnit.Framework;

using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

namespace SMART.Test.Gui
{
    using IOC;

    using SMART.Gui.ViewModel.ProjectExplorer;


    [TestFixture]
    public class ModelFolderViewModelTests
    {

        Mock<Model> model;
        Mock<IProject> project;
        ModelFolderViewModel viewModel;
        [SetUp]
        public void Setup()
        {
            BootStrapper.Configure(Resolver.Container);
            model = new Mock<Model>();
            project = new Mock<IProject>();
            viewModel = new ModelFolderViewModel(model.Object, project.Object);
            
        }

        [Test]
        public void upon_creation_commands_should_not_be_null()
        {
            Assert.IsNotNull(viewModel.AddToTestcase);
            Assert.IsNotNull(viewModel.Open);
            Assert.IsNotNull(viewModel.Remove);
            Assert.IsNotNull(viewModel.Rename);            
        }

        [Test]
        public void add_to_testcase_should_add_model_to_testcase()
        {            
            Assert.Ignore("AddToTestCase is not implemented yet");
        }

        [Test]
        public void rename_should_rename_the_modelmodel()
        {
            model.SetupProperty(g => g.Name, "old name");
            
            viewModel.Rename.OnExecute.Invoke(null);
            Assert.AreEqual("new name", viewModel.Name);            
        }

        [Test]
        public void remove_should_call_into_project_and_remove_model()
        {
            project.Setup(p => p.RemoveModel(model.Object)).Returns(true).Verifiable();
            viewModel.Remove.OnExecute.Invoke(null);
            project.Verify();
        }

        [Test]
        public void open_should_open_a_model_window()
        {
            Assert.Ignore("Open is not implemented yet");
        }

        [Test]
        public void verify_that_modelid_returns_the_models_id()
        {
            var id = Guid.NewGuid();
            model.SetupGet(g => g.Id).Returns(id);
            Assert.AreEqual(id, viewModel.ModelId);
        }
        
    }
}