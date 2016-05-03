using Moq;
using NUnit.Framework;

using SMART.Core.Events;
using SMART.Core.Interfaces;
using SMART.IOC;

namespace SMART.Test.Gui
{
    using SMART.Gui.ViewModel.ProjectExplorer;

    [TestFixture]
    public class RootTestcaseFolderViewModelTests
    {
        Mock<ITestcase> testcase;

        Mock<IProject> project;
        RootTestcaseFolderViewModel viewModel;

        [SetUp]
        public void setup()
        {
            BootStrapper.Configure(Resolver.Container);
            project = new Mock<IProject>();
            testcase = new Mock<ITestcase>();
            viewModel = new RootTestcaseFolderViewModel(project.Object);
        }

        [Test]
        public void adding_a_new_test_case_should_call_project()
        {
            
            project.Setup(p=>p.AddTestCase(It.IsAny<ITestcase>())).Verifiable();
            Assert.IsTrue(viewModel.AddTestcase.OnCanExecute.Invoke(null));

            viewModel.AddTestcase.OnExecute.Invoke(null);

            project.Verify();
        }

        [Test]
        [Ignore]
        public void property_changed_with_testcase_added()
        {
            // Assign
            
            // Act
            project.Raise(p=>p.PropertyChanged+=null, new SmartPropertyChangedEventArgs("Testcases",testcase.Object, SmartPropertyChangedAction.Add));
            // Assert
            Assert.AreEqual(1, viewModel.FolderViewModels.Count);
        }


        [Test]
        [Ignore]

        public void property_changed_with_testcase_remove()
        {
 

            // Act
            project.Raise(p => p.PropertyChanged += null, new SmartPropertyChangedEventArgs("Testcases", testcase.Object, SmartPropertyChangedAction.Add));

            project.Raise(p => p.PropertyChanged += null, new SmartPropertyChangedEventArgs("Testcases", testcase.Object, SmartPropertyChangedAction.Remove));
            // Assert
            Assert.AreEqual(0, viewModel.FolderViewModels.Count);
        }

        [Test]
        public void property_changed_with_testcase_null_should_just_return()
        {
            // Assign
            
            // Act
            project.Raise(p => p.PropertyChanged += null, new SmartPropertyChangedEventArgs("Testcases", null, SmartPropertyChangedAction.Add));

            // Assert
            Assert.AreEqual(0, viewModel.FolderViewModels.Count);
        }


        [Test]
        public void property_changed_with_testcase_but_no_action_should_just_return()
        {
            // Assign
            
            // Act
            project.Raise(p => p.PropertyChanged += null, new SmartPropertyChangedEventArgs("Testcases", testcase.Object, SmartPropertyChangedAction.None));

            
            // Assert
            Assert.AreEqual(0, viewModel.FolderViewModels.Count);
        }

        [Test]
        public void folderviewmodels_gets_filled_from_the_test_cases_on_the_project()
        {
            // Assign
            // Act
            
            // Assert
            
            project.VerifyGet(p=>p.Testcases);
        }
    }
}