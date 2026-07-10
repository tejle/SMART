using SMART.Base.StopCriterias;
using SMART.Core;
using System.Collections.Generic;
using NUnit.Framework;
using Moq;
using SMART.Core.Interfaces;

namespace SMART.Test.Base.StopCriterias
{
    /// <summary>
    ///This is a test class for AndStopCriteriaTest and is intended
    ///to contain all AndStopCriteriaTest Unit Tests
    ///</summary>
    [TestFixture]
    public class AndStopCriteriaTest
    {
        /// <summary>
        ///A test for PercentComplete
        ///</summary>
        [Test]
        public void PercentComplete_should_return_100_percent()
        {
            var target = new AndStopCriteria();

            var stat1 = new Mock<IStopCriteria>();
            stat1.SetupGet(s => s.PercentComplete).Returns(1.0).Verifiable();

            var stat2 = new Mock<IStopCriteria>();
            stat2.SetupGet(s => s.PercentComplete).Returns(1.0).Verifiable();

            target.Criterias = new List<IStopCriteria> { stat1.Object, stat2.Object };
            Assert.AreEqual(1.0, target.PercentComplete);
            stat1.Verify();
            stat2.Verify();
        }

        /// <summary>
        ///A test for PercentComplete
        ///</summary>
        [Test]
        public void PercentComplete_should_return_50_percent()
        {
            var target = new AndStopCriteria();

            var stat1 = new Mock<IStopCriteria>();
            stat1.Setup(s => s.PercentComplete).Returns(0.25);

            var stat2 = new Mock<IStopCriteria>();
            stat2.Setup(s => s.PercentComplete).Returns(0.75);

            target.Criterias = new List<IStopCriteria> { stat1.Object, stat2.Object };
            Assert.AreEqual(0.5, target.PercentComplete);
        }

        /// <summary>
        ///A test for Criterias
        ///</summary>
        [Test]
        public void CriteriasTest()
        {
            var target = new AndStopCriteria();
            
            var stat1 = new Mock<IStopCriteria>();
            var stat2 = new Mock<IStopCriteria>();

            target.Criterias = new List<IStopCriteria> {stat1.Object, stat2.Object};
            int count = 0;
            target.Criterias.ForEach(c => count++);
            Assert.AreEqual(2, count);
        }

        /// <summary>
        ///A test for Complete
        ///</summary>
        [Test]
        public void Complete_should_return_true()
        {
            var target = new AndStopCriteria();

            var stat1 = new Mock<IStopCriteria>();
            stat1.Setup(s => s.Complete).Returns(true);

            var stat2 = new Mock<IStopCriteria>();
            stat2.Setup(s => s.Complete).Returns(true);

            target.Criterias = new List<IStopCriteria> { stat1.Object, stat2.Object };
            Assert.AreEqual(true, target.Complete);
        }

        /// <summary>
        ///A test for Complete
        ///</summary>
        [Test]
        public void Complete_should_return_false()
        {
            var target = new AndStopCriteria();

            var stat1 = new Mock<IStopCriteria>();
            stat1.Setup(s => s.Complete).Returns(true);

            var stat2 = new Mock<IStopCriteria>();
            stat2.Setup(s => s.Complete).Returns(false);

            target.Criterias = new List<IStopCriteria> { stat1.Object, stat2.Object };
            Assert.AreEqual(false, target.Complete);
        }

        /// <summary>
        ///A test for Reset
        ///</summary>
        [Test]
        public void ResetTest()
        {
            var target = new AndStopCriteria();
            var stat1 = new Mock<IStopCriteria>();
            var stat2 = new Mock<IStopCriteria>();
            target.Criterias = new List<IStopCriteria> { stat1.Object, stat2.Object };
            Assert.DoesNotThrow(target.Reset);
        }

    }
}