using NUnit.Framework;
using SMART.Core.DomainModel;


namespace SMART.Test.Core
{
    [TestFixture]
    public class StateTests
    {
        [Test]
        public void state_visit_should_fire_a_propertychangedevent()
        {
            var state = new State();
            var b = false;
            state.PropertyChanged += (sender, e) => { b = true; };

            state.Visit();
            Assert.AreEqual(true, b, "VisitEvent not fired");
            Assert.AreEqual(1, state.VisitCount);
        }
        [Test]
        public void changing_iscurrent_should_invoke_propertychanged()
        {
            var state = new State();
            var b = false;
            state.PropertyChanged += (sender, e) => { b = true; };

            state.IsCurrent = true;
            Assert.AreEqual(true, b, "VisitEvent not fired");
            Assert.IsTrue(state.IsCurrent);
        }
    }
}