using NUnit.Framework;
using SMART.Core.Model.Graph;

namespace SMART.Core.UnitTests
{
    [TestFixture]
    public class VertexTests
    {
        [Test]
        public void vertex_visit_should_fire_a_propertychangedevent()
        {
            var vertex = new Vertex();
            var b = false;
            vertex.PropertyChanged += (sender, e) => { b = true; };

            vertex.Visit();
            Assert.AreEqual(true, b, "VisitEvent not fired");
            Assert.AreEqual(1, vertex.VisitCount);
        }

    }
}