using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

using Rhino.Mocks;
using Moq;
using SMART.Core.BusinessLayer;

namespace SMART.Test.Core
{
    [TestFixture]
    public class ProjectUnitTests
    {
        private IProject project;
        [SetUp]
        public void setup()
        {
            project = new Project();
        }
        [Test]
        public void adding_the_same_model_twice_should_return_false_and_count_be_one()
        {

            var g = new Model();

            project.AddModel(g);
            var s = project.AddModel(g);
            Assert.IsFalse(s);
            Assert.AreEqual(1, project.Models.Count());
        }

        [Test]
        public void removing_a_model_that_does_not_exist_should_return_false()
        {
            var g = new Model();

            project.AddModel(g);

            Assert.IsTrue(project.RemoveModel(g));
            Assert.IsFalse(project.RemoveModel(g));
        }
         
        
        [Test]
        public void adding_the_same_testcase_twice_should_return_false_and_count_be_one()
        {
            ITestcase testCase = MockRepository.GenerateStub<ITestcase>();
            Assert.IsTrue(project.AddTestCase(testCase));
            Assert.IsFalse(project.AddTestCase(testCase));
            Assert.AreEqual(1, project.Testcases.Count());
        }

        [Test]
        public void removing_a_testcase_that_does_not_exist_should_return_false()
        {
            ITestcase testCase = MockRepository.GenerateStub<ITestcase>();
            Assert.IsTrue(project.AddTestCase(testCase));
            Assert.IsTrue(project.RemoveTestCase(testCase));
            Assert.IsFalse(project.RemoveTestCase(testCase));
            Assert.AreEqual(0, project.Testcases.Count());
        }

        //[Test]
        //public void setting_execution_style_to_async_should_set_execution_style_in_the_executor()
        //{
        //    var mock = new Mock<ITestcaseExecutor>();
        //    mock.Setup(t => t.SetExecutionPolicy(ExecutionPolicy.Asynchronous)).Verifiable();
        //    project.TestcaseExecutor = mock.Object;
        //    project.SetExecutionPolicy(ExecutionPolicy.Asynchronous);
            
        //    mock.Verify();
        //}

        //[Test]
        //public void get_execution_policy_from_test_executor_should_return_syncronous_as_default()
        //{
        //    var mock = new Mock<ITestcaseExecutor>();
        //    mock.Setup(t => t.GetExecutionPolicy()).Returns(ExecutionPolicy.Synchronous);
        //    project.TestcaseExecutor = mock.Object;
        //    ExecutionPolicy policy = project.GetExecutionPolicy();
        //    Assert.AreEqual(ExecutionPolicy.Synchronous, policy);
        //}

        //[Test]
        //public void when_no_executor_is_set_on_the_project_policy_is_NotSet()
        //{
        //    Assert.AreEqual(ExecutionPolicy.NotSet, project.GetExecutionPolicy());
        //}

        //[Test]
        //public void start_executing_project()
        //{
        //    //var testcase = new Mock<ITestCase>();
        //    //testcase.Setup(t=>t.StartExecution()).Verifiable();



        //}

        //[Test]
        //public void stop_execution_project(){}
    }
}