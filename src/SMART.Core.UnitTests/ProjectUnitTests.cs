using System.Linq;
using NUnit.Framework;
using SMART.Core.Model.ProjectStructure;
using SMART.Core.Model.Graph;
using Rhino.Mocks;
using Moq;
using SMART.Core.BusinessLayer;

namespace SMART.Core.UnitTests
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
        public void adding_the_same_graph_twice_should_return_false_and_count_be_one()
        {

            var g = new Graph();

            project.AddGraph(g);
            var s = project.AddGraph(g);
            Assert.IsFalse(s);
            Assert.AreEqual(1, project.Graphs.Count());
        }

        [Test]
        public void removing_a_graph_that_does_not_exist_should_return_false()
        {
            var g = new Graph();

            project.AddGraph(g);

            Assert.IsTrue(project.RemoveGraph(g));
            Assert.IsFalse(project.RemoveGraph(g));
        }
         
        
        [Test]
        public void adding_the_same_testcase_twice_should_return_false_and_count_be_one()
        {
            ITestCase testCase = MockRepository.GenerateStub<ITestCase>();
            Assert.IsTrue(project.AddTestCase(testCase));
            Assert.IsFalse(project.AddTestCase(testCase));
            Assert.AreEqual(1, project.Testcases.Count());
        }

        [Test]
        public void removing_a_testcase_that_does_not_exist_should_return_false()
        {
            ITestCase testCase = MockRepository.GenerateStub<ITestCase>();
            Assert.IsTrue(project.AddTestCase(testCase));
            Assert.IsTrue(project.RemoveTestCase(testCase));
            Assert.IsFalse(project.RemoveTestCase(testCase));
            Assert.AreEqual(0, project.Testcases.Count());
        }

        [Test]
        public void setting_execution_style_to_async_should_set_execution_style_in_the_executor()
        {
            var mock = new Mock<ITestcaseExecutor>();
            mock.Setup(t => t.SetExecutionPolicy(ExecutionPolicy.Asynchronous)).Verifiable();
            project.TestcaseExecutor = mock.Object;
            project.SetExecutionPolicy(ExecutionPolicy.Asynchronous);
            
            mock.Verify();
        }

        [Test]
        public void get_execution_policy_from_test_executor_should_return_syncronous_as_default()
        {
            var mock = new Mock<ITestcaseExecutor>();
            mock.Setup(t => t.GetExecutionPolicy()).Returns(ExecutionPolicy.Synchronous);
            project.TestcaseExecutor = mock.Object;
            ExecutionPolicy policy = project.GetExecutionPolicy();
            Assert.AreEqual(ExecutionPolicy.Synchronous, policy);
        }

        [Test]
        public void when_no_executor_is_set_on_the_project_policy_is_NotSet()
        {
            Assert.AreEqual(ExecutionPolicy.NotSet, project.GetExecutionPolicy());
        }
    }
}