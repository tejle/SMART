using Moq;
using NUnit.Framework;
using SMART.Core.DomainModel;
using SMART.Core.Events;
using SMART.Core.Interfaces;


namespace SMART.Test.Gui
{
    using SMART.Gui.ViewModel.ProjectExplorer;

    [TestFixture]
    public class TestcaseFolderViewModelTests
    {

        Mock<Model> model;
        Mock<ITestcase> testcase;
        Mock<IProject> project;
        TestcaseFolderViewModel viewmodel;

        [SetUp]
        public void setup()
        {
            model = new Mock<Model>();
            testcase = new Mock<ITestcase>();
            project = new Mock<IProject>();
            viewmodel = new TestcaseFolderViewModel(testcase.Object, project.Object);

        }
        [Test]
        public void testcase_property_models_add_should_return_count_one()
        {
            
            testcase.Raise(t=>t.PropertyChanged+=null, new SmartPropertyChangedEventArgs("Models", model.Object, SmartPropertyChangedAction.Add));

            Assert.AreEqual(1, viewmodel.FolderViewModels.Count);
        }

        [Test]
        public void testcase_property_models_remove_exsiting_should_return_count_zero()
        {
            testcase.Raise(t => t.PropertyChanged += null, new SmartPropertyChangedEventArgs("Models", model.Object, SmartPropertyChangedAction.Add));
            testcase.Raise(t => t.PropertyChanged += null, new SmartPropertyChangedEventArgs("Models", model.Object, SmartPropertyChangedAction.Remove));

            Assert.AreEqual(0, viewmodel.FolderViewModels.Count);
        }


        [Test]
        public void testcase_property_name_should_set_name_from_testcase()
        {
            testcase.SetupGet(t => t.Name).Returns("testcasename");
            testcase.Raise(t => t.PropertyChanged += null, new SmartPropertyChangedEventArgs("Name", null, SmartPropertyChangedAction.None));
            
            Assert.AreEqual("testcasename", viewmodel.Name);
        }

        [Test]
        public void adding_a_new_model_should_add_it_to_both_the_project_and_the_testcase()
        {
            // Assign
            project.Setup(p => p.AddModel(It.IsAny<Model>())).Verifiable();
            testcase.Setup(t => t.Add(It.IsAny<Model>())).Verifiable();
         
            // Act
            Assert.IsTrue(viewmodel.AddModel.OnCanExecute.Invoke(null));
            viewmodel.AddModel.OnExecute.Invoke(null);
            
            // Assert
            project.Verify();
            testcase.Verify();
        }

        [Test]
        public void renaming_should_set_name_on_testcase()
        {
            // Assign
            testcase.SetupSet(t=>t.Name = It.IsAny<string>()).Verifiable();
            testcase.SetupGet(t => t.Name).Returns(string.Empty);

            // Act
            Assert.IsTrue(viewmodel.Rename.OnCanExecute.Invoke(null));
            viewmodel.Rename.OnExecute.Invoke(null);
            // Assert
            testcase.Verify();
        }

        [Test]
        public void remove_should_remove_testcase_from_project()
        {
            // Assign
            project.Setup(t => t.RemoveTestCase(testcase.Object)).Returns(true).Verifiable();

            // Act
            Assert.IsTrue(viewmodel.Remove.OnCanExecute.Invoke(null));
            viewmodel.Remove.OnExecute.Invoke(null);
            // Assert
            project.Verify();
        }

        [Test]
        public void on_creation_models_are_added_to_the_folderviewmodels()
        {
            testcase.VerifyGet(t=>t.Models);
        }
    }
}