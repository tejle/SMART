using Moq;
using NUnit.Framework;
using SMART.Core.DomainModel;
using SMART.Core.Events;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Services;

namespace SMART.Test.Gui
{
    using SMART.Gui.ViewModel.ProjectExplorer;

    [TestFixture]
    public class RootModelFolderViewModelTests
    {

        Mock<IProject> project;
        Mock<IModelService> modelservice;
        Mock<Model> model;
        RootModelFolderViewModel viewmodel;

        [SetUp]
        public void setup()
        {

            project = new Mock<IProject>();
            modelservice = new Mock<IModelService>();
            model = new Mock<Model>();
            viewmodel = new RootModelFolderViewModel(project.Object, modelservice.Object);

        }
        [Test]
        public void add_model_adds_a_new_model_on_the_project()
        {
            modelservice.Setup(g => g.CreateModel(It.IsAny<string>())).Returns(model.Object).Verifiable();
            project.Setup(p => p.AddModel(model.Object)).Verifiable();
            Assert.IsTrue(viewmodel.AddModel.OnCanExecute.Invoke(null));
            viewmodel.AddModel.OnExecute.Invoke(null);

            modelservice.Verify();
            project.Verify();

        }

        [Test]
        public void viewmodels_are_created_for_models_in_project()
        {
            // Assign
            // Act
            // Assert
            project.VerifyGet(p => p.Models);
        }

        [Test]
        public void property_models_with_null_model_should_just_return()
        {
            // Assign
            
            // Act
            project.Raise(p=>p.PropertyChanged+=null, new SmartPropertyChangedEventArgs("Models", null, SmartPropertyChangedAction.Add));
            // Assert

            Assert.AreEqual(0, viewmodel.FolderViewModels.Count);
        }


        [Test]
        public void property_models_with_model_and_add_command_count_should_be_one()
        {
            // Assign
            
            // Act
            project.Raise(p => p.PropertyChanged += null, 
                new SmartPropertyChangedEventArgs("Models", model.Object, SmartPropertyChangedAction.Add));
            // Assert

            Assert.AreEqual(1, viewmodel.FolderViewModels.Count);
        }

        [Test]
        public void property_models_with_model_and_remove_command_count_should_be_one()
        {
            // Assign
            
            // Act
            project.Raise(p => p.PropertyChanged += null,
                new SmartPropertyChangedEventArgs("Models", model.Object, SmartPropertyChangedAction.Add));
            project.Raise(p => p.PropertyChanged += null,
                new SmartPropertyChangedEventArgs("Models", model.Object, SmartPropertyChangedAction.Remove));
            // Assert

            Assert.AreEqual(0, viewmodel.FolderViewModels.Count);
        }

        [Test]
        public void property_models_with_model_and_remove_command_on_empty_should_just_return()
        {
            // Assign

            // Act
            project.Raise(p => p.PropertyChanged += null,
                new SmartPropertyChangedEventArgs("Models", model.Object, SmartPropertyChangedAction.Remove));
            // Assert

            Assert.AreEqual(0, viewmodel.FolderViewModels.Count);
        }
    }
}