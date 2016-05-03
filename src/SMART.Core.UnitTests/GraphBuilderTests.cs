using System;
using Moq;
using NUnit.Framework;
using SMART.Core.Model;
using SMART.Core.BusinessLayer;
using SMART.Core.Model.Graph;
using System.Collections.Generic;
using System.Linq;
namespace SMART.Core.UnitTests
{
    [TestFixture]
    public class GraphBuilderTests
    {
        [Test]
        public void merging_one_graph_should_return_a_executable_graph()
        {
            var sandbox = new Mock<ISandbox>();
            var statManager = new Mock<IStatisticsManager>();
            var graphBuilder = new GraphBuilder(sandbox.Object, statManager.Object);

            var graph = new Graph();
            graph.Add(new StartVertex { Id = Guid.NewGuid(), Label = "Start" });
            graph.Add(new StopVertex { Id = Guid.NewGuid(), Label = "Stop" });
            graph.AddRange(new List<Vertex> 
                {
                    new Vertex()
                     {
                         Label = "v1", Type = VertexType.Normal
                     },
                     new Vertex()
                         {
                             Label="v2", Type = VertexType.Normal
                         }
                });
            graph.AddRange(new List<Edge>
                               {
                                   new Edge
                                       {
                                           Label = "e1",
                                           Source = graph.StartVertex,
                                           Destination = graph.Vertices.Where(v => v.Label == "v1").FirstOrDefault()
                                       },
                                   new Edge()
                                       {
                                           Label = "e2",
                                           Source =
                                               (from v in graph.Vertices where v.Label.Equals("v1") select v).
                                               FirstOrDefault(),
                                            Destination = graph.Vertices.Where(v => v.Label.Equals("v2")).FirstOrDefault()
                                       },
                                   new Edge()
                                       {
                                           Label = "e3",
                                           Source = graph.Vertices.Where(v => v.Label.Equals("v2")).FirstOrDefault(),
                                           Destination = graph.StopVertex
                                       }
                               });


            var exegraph = graphBuilder.Merge(new List<IEditableGraph> { graph }, graph);
            Assert.IsNotNull(exegraph);
            Assert.AreEqual(4, exegraph.Vertices.Count());
            Assert.AreEqual(3, exegraph.Edges.Count());
            //Assert.AreEqual(null, exegraph.StartVertex);
            //Assert.AreEqual(null, exegraph.StopVertex);
        }
    }
}