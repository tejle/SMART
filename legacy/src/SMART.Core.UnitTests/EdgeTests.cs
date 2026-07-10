using System.Collections.Generic;
using NUnit.Framework;
using SMART.Core.Model.Graph;

namespace SMART.Core.UnitTests
{
    [TestFixture]
    public class EdgeTests
    {
        [Test]
        public void edge_visit_should_fire_a_propertychangedevent()
        {
            var edge = new Edge();
            var b = false;
            edge.PropertyChanged += (sender, e) => { b = true; };

            edge.Visit();
            Assert.AreEqual(true, b, "VisitEvent not fired");
            Assert.AreEqual(1, edge.VisitCount);
    
        }

        [Test]
        public void parameters_should_be_returned_as_semicolon_separated_string()
        {
            string p = "parameter1;parameter2";
            var edge = new Edge();
            
            Assert.IsNotNull(edge.Parameters);
            
            edge.Parameter = p;
            Assert.AreEqual(2, edge.Parameters.Count);
            Assert.AreEqual("parameter1", edge.Parameters[0]);
            Assert.AreEqual("parameter2", edge.Parameters[1]);
            Assert.AreEqual(p, edge.Parameter);

        }
    }
}