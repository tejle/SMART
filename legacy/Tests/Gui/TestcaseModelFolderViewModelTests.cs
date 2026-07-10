using Moq;
using NUnit.Framework;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

using SMART.Gui.ViewModel.FolderViewModels;
using System;

namespace SMART.Test.Gui
{
    [TestFixture]
    public class TestcaseModelFolderViewModelTests
    {
        Mock<Model> model;
        Mock<ITestcase> testcase;
        TestcaseModelFolderViewModel viewmodel;

        [SetUp]
        public void setup()
        {
            model = new Mock<Model>();
            model.SetupGet(g => g.Name).Returns("Mocked model");

            testcase = new Mock<ITestcase>();
            viewmodel = new TestcaseModelFolderViewModel(model.Object, testcase.Object);

        }

        [Test]
        public void rename_should_set_name_to_model()
        {
            model.SetupSet(g=>g.Name = "new name").Verifiable();
            
            Assert.IsTrue(viewmodel.Rename.OnCanExecute.Invoke(null));

            viewmodel.Rename.OnExecute.Invoke(null);

            model.Verify();
        }

        [Test]
        public void remove_should_remove_model_from_testcase()
        {
            // Assign
            testcase.Setup(t=>t.RemoveModel(model.Object)).Verifiable();

            // Act
            Assert.IsTrue(viewmodel.Remove.OnCanExecute.Invoke(null));
            viewmodel.Remove.OnExecute.Invoke(null);

            // Assert
            testcase.Verify();

        }

        [Test]
        [Ignore]
        public void open_should_raise_smartrouted_event_and_sender_should_be_a_model()
        {
            //var eventservice = new Mock<IEventService>();
            //var viewmodel = new TestcaseModelFolderViewModel(model.Object, testcase.Object,eventservice.Object);
            //Model g= null;
            //viewmodel.SmartRoutedEvent += (s, e) => g = e.State as Model;

            //viewmodel.Open.OnExecute.Invoke(null);
            //Assert.AreSame(model.Object, g);
        }

    }
}