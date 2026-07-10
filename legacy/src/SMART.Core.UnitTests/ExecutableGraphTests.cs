using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SMART.Core.Model.Graph;
using Rhino.Mocks;
using SMART.Core.BusinessLayer;
using SMART.Core.Model;

namespace SMART.Core.UnitTests
{
    [TestFixture]
    public class ExecutableGraphTests
    {
        [Test]
        public void create_step_should_return_a_correctly_formed_step()
        {
            var repository = new MockRepository();
            var statisticsManager = repository.StrictMock<IStatisticsManager>();
            var sandbox = repository.StrictMock<ISandbox>();

            var v1 = new Vertex();
            var v2 = new Vertex();
            var edge = new Edge();
            edge.Label = "label";
            edge.Parameter = "p1;p2";
            edge.Source = v1;
            edge.Destination = v2;

            var graph = new ExecutableGraph(new List<Edge> {edge},
                                            new List<Vertex> {v1, v2}, statisticsManager, sandbox);

            var step = graph.CreateStep(edge);
            Assert.AreEqual("label",step.Function);
            Assert.AreEqual("p1", step.Parameters[0]);
            Assert.AreEqual("p2", step.Parameters[1]);
            Assert.AreSame(edge, step.GraphElement);
        }

        [Test]
        public void when_retrieving_out_edges_sandbox_should_be_called_to_verify_possible_edges()
        {
            var repository = new MockRepository();
            var statisticsManager = repository.StrictMock<IStatisticsManager>();
            var sandbox = repository.StrictMock<ISandbox>();

            var v1 = new Vertex();
            var v2 = new Vertex();
            var edge = new Edge();
            edge.Label = "label";
            edge.Parameter = "p1;p2";
            edge.Source = v1;
            edge.Destination = v2;
            
            Expect.Call(sandbox.CanExecute(edge)).IgnoreArguments().Return(true);
            repository.ReplayAll();


            var graph = new ExecutableGraph(new List<Edge> { edge },
                                            new List<Vertex> { v1, v2 }, statisticsManager, sandbox);


            graph.GetExecutableOutEdges(v1).ToList();

            repository.VerifyAll();
        }
    }
}