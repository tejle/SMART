using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SMART.Core.Model.Graph;

namespace SMART.Core.UnitTests
{
    [TestFixture]
    public class GraphTests
    {
        Graph graph;

        [SetUp]
        public void setup()
        {
            graph = new Graph();
            graph.Add(new StartVertex() {Id = Guid.NewGuid(), Label = "Start"});
            graph.Add(new StopVertex() { Id = Guid.NewGuid(), Label = "Stop" });
        }

        [Test]
        public void adding_a_vertex_to_the_graph_should_result_in_vertex_count_be_equal_to_one()
        {
            graph = new Graph();
            var vertex = new Vertex();

            var success = graph.Add(vertex);
            Assert.AreEqual(true, success, "Graph.Add failed");
            Assert.AreEqual(1, graph.Vertices.Count());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void adding_a_null_vertex_to_a_graph_should_throw_an_argumentnullexception()
        {
            graph.Add((Vertex)null);
        }

        [Test]
        public void adding_the_same_vertex_twice_should_ignore_the_second_and_return_false()
        {
            graph = new Graph();
            var vertex = new Vertex();

            graph.Add(vertex);
            Assert.AreEqual(false, graph.Add(vertex));
            Assert.AreEqual(1, graph.Vertices.Count());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void adding_a_null_edge_to_a_graph_should_throw_an_argumentnullexception()
        {
            graph.Add((Edge)null);
        }

        [Test]
        public void adding_a_edge_to_the_graph_should_set_the_edge_to_the_vertices()
        {
            var vertex1 = new Vertex();
            var vertex2 = new Vertex();
            var edge = new Edge();
            edge.Source = vertex1;
            edge.Destination = vertex2;

            graph.Add(vertex1);
            graph.Add(vertex2);

            var success = graph.Add(edge);
            Assert.IsTrue(success);

        }

        [Test]
        public void outedges_should_return_edges_that_have_this_as_source()
        {
            var vertex1 = new Vertex();
            var vertex2 = new Vertex();
            var edge = new Edge();
            edge.Source = vertex1;
            edge.Destination = vertex2;

            graph.Add(vertex1);
            graph.Add(vertex2);
            graph.Add(edge);

            var edges = graph.OutEdges(e => e.Source.Equals(vertex1));
            Assert.Contains(edge, edges.ToList());
        }

        [Test]
        public void removing_a_vertex_should_return_true_if_success()
        {
            var vertex = new Vertex();
            graph.Add(vertex);
            Assert.IsTrue(graph.Remove(vertex));
        }

        [Test]
        public void removing_a_list_of_vertex_should_return_true_if_success()
        {
            var vertex = new List<Vertex> { new Vertex() };
            graph.AddRange(vertex);
            Assert.IsTrue(graph.RemoveRange(vertex));
        }

        [Test]
        public void add_list_of_edges_should_return_true()
        {

            var edge = new List<Edge>{new Edge()};
            Assert.IsTrue(graph.AddRange(edge));
        }

        [Test]
        public void removing_a_edge_should_return_true_if_success()
        {
            var vertex1 = new Vertex();
            var vertex2 = new Vertex();
            var edge = new Edge();
            edge.Source = vertex1;
            edge.Destination = vertex2;

            graph.Add(vertex1);
            graph.Add(vertex2);
            graph.Add(edge);
            Assert.IsTrue(graph.Remove(edge));
        }

        [Test]
        public void removing_a_list_of_edges_should_return_true_if_success()
        {
            var vertex1 = new Vertex();
            var vertex2 = new Vertex();
            var edge = new Edge();
            edge.Source = vertex1;
            edge.Destination = vertex2;

            graph.Add(vertex1);
            graph.Add(vertex2);
            graph.Add(edge);

            var range = new List<Edge> {edge};
            Assert.IsTrue(graph.RemoveRange(range));
        }

        [Test]
        public void removing_a_edge_from_a_start_vertex_should_return_true()
        {
            var edge = new Edge();
            
            graph.Add(edge);

            var start = graph.StartVertex.Out.ToList();
            Assert.IsTrue(graph.RemoveRange(start));
        }

        [Test]
        public void removing_a_edge_from_a_stop_vertex_should_return_true()
        {
            var edge = new Edge();

            graph.Add(edge);

            var stop = graph.StopVertex.In.ToList();
            Assert.IsTrue(graph.RemoveRange(stop));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void removerange_null_edge_should_throw_argument_null_exception()
        {
            graph.RemoveRange((List<Edge>) null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void removerange_null_vertex_should_throw_argument_null_exception()
        {
            graph.RemoveRange((List<Vertex>)null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void remove_null_edge_should_throw_argument_null_exception()
        {
            graph.Remove((Edge)null);
        }
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void remove_null_vertex_should_throw_argument_null_exception()
        {
            graph.Remove((Vertex) null);

        }
        [Test]
        public void add_edge_should_return_false_when_edge_exists()
        {
            var vertex1 = new Vertex();
            var vertex2 = new Vertex();
            var edge = new Edge();
            edge.Source = vertex1;
            edge.Destination = vertex2;

            graph.Add(vertex1);
            graph.Add(vertex2);
            graph.Add(edge);

            Assert.IsFalse(graph.Add(edge));
        }

        [Test]
        public void add_edge_should_return_false_when_source_does_not_exist_in_graph()
        {
            var vertex1 = new Vertex();
            var vertex2 = new Vertex();
            var edge = new Edge();
            edge.Source = vertex1;
            edge.Destination = vertex2;

            graph.Add(vertex2);
            Assert.IsFalse(graph.Add(edge));

            
        }


        [Test]
        public void add_edge_should_return_false_when_destination_does_not_exist_in_graph()
        {
            var vertex1 = new Vertex();
            var vertex2 = new Vertex();
            var edge = new Edge();
            edge.Source = vertex1;
            edge.Destination = vertex2;

            graph.Add(vertex1);
            Assert.IsFalse(graph.Add(edge));


        }
    }
}
