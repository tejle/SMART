using Moq;
using NUnit.Framework;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Services;

namespace SMART.Test.Gui
{
    using IOC;

    using SMART.Core.Services;
    using SMART.Gui.ViewModel.ProjectExplorer;

    [TestFixture]
    public class ProjectExplorerViewModelTests
    {
        [Test]
        public void upon_creation_collections_and_commands_should_not_be_null()
        {
            Resolver.RegisterSingleton(typeof(IEventService), typeof(EventService));

            var mock = new Mock<IProjectService>();
            ProjectExplorerViewModel viewModel = new ProjectExplorerViewModel(null, mock.Object, null);
            Assert.IsNotNull(viewModel.OpenProject);
            Assert.IsNotNull(viewModel.ProjectFolderViewModels);
        }

        [Test]
        [Ignore("Problem with resolve")]
        public void open_project_from_file_should_call_service_and_add_to_collection()
        {
            var service = new Mock<IProjectService>();
            var project = new Mock<IProject>();
            
            service.Setup(s => s.OpenProjectFromFile(It.IsAny<string>())).Returns(project.Object).Verifiable();
         
            var viewModel = new ProjectExplorerViewModel(null,service.Object, null);
            viewModel.OpenProjectFromFile("");

            Assert.AreEqual(1, viewModel.ProjectFolderViewModels.Count);
            service.Verify();
         
        }

        [Test]
        public void verify_that_viewmodel_is_connected_to_the_models_event()
        {
            //var service = new Mock<IProjectService>();
            //var project = new Mock<IProject>();

            //service.Setup(s => s.OpenProjectFromFile(It.IsAny<string>())).Returns(project.Object);
            //string actual = string.Empty;
            
            //var viewModel = new ProjectExplorerViewModel(service.Object);
            //viewModel.PropertyChanged += (s, e) => actual = e.PropertyName;
            //viewModel.OpenProjectFromFile("");
            //project.Raise(p => p.PropertyChanged += null, new SmartPropertyChangedEventArgs("Name"));

            //Assert.AreEqual("Name", actual);
            
        }
    }
}