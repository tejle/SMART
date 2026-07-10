using System.Collections.Generic;
using NUnit.Framework;
using SMART.Core.DomainModel;


namespace SMART.Test.Core
{

    [TestFixture]
    public class TransitionTests
    {
        [Test]
        public void transition_visit_should_fire_a_propertychangedevent()
        {
            var transition = new Transition();
            var b = false;
            transition.PropertyChanged += (sender, e) => { b = true; };

            transition.Visit();
            Assert.AreEqual(true, b, "VisitEvent not fired");
            Assert.AreEqual(1, transition.VisitCount);
    
        }

        [Test]
        public void parameters_should_be_returned_as_semicolon_separated_string()
        {
            string p = "parameter1;parameter2";
            var transition = new Transition();
            
            Assert.IsNotNull(transition.Parameters);
            
            transition.Parameter = p;
            Assert.AreEqual(2, transition.Parameters.Count);
            Assert.AreEqual("parameter1", transition.Parameters[0]);
            Assert.AreEqual("parameter2", transition.Parameters[1]);
            Assert.AreEqual(p, transition.Parameter);

        }

        [Test]
        public void call_to_IsCurrent_should_invoke_an_event()
        {
            var transition = new Transition();
            bool s = false;
            transition.PropertyChanged += (sender, e) => s = true;

            transition.IsCurrent = true;

            Assert.IsTrue(s);
            Assert.IsTrue(transition.IsCurrent);
        }
    }
}