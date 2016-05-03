using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMART.Core.BusinessLayer;
using SMART.Core.Model.Graph;
using NUnit.Framework;

namespace SMART.Core.UnitTests
{
    public class GraphManagerTests
    {
        private GraphManager<Graph> graphManager;
 
        [SetUp]
        public void setup()
        {
            Graph graph = new Graph();
            graphManager = new GraphManager<Graph>(graph);
        }

    }
}
