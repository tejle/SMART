using SMART.Base.StopCriterias;
using System;
using Moq;
using NUnit.Framework;

namespace SMART.Test.Base.StopCriterias
{
    using SMART.Base;

    /// <summary>
    ///This is a test class for TimeBasedStopCriteriaTest and is intended
    ///to contain all TimeBasedStopCriteriaTest Unit Tests
    ///</summary>
    [TestFixture]
    public class TimeBasedStopCriteriaTest
    {
        /// <summary>
        ///A test for PercentComplete
        ///</summary>
        [Test]
        public void PercentComplete_should_return_50_percent()
        {
            var target = new TimeBasedStopCriteria { Limit = new TimeSpan(0, 0, 60) };

            SystemTime.Now = () => new DateTime(2009, 1, 1, 12, 0, 0);
            target.Reset();
            SystemTime.Now = () => new DateTime(2009, 1, 1, 12, 0, 30);

            Assert.AreEqual(0.5, target.PercentComplete);
        }

        /// <summary>
        ///A test for Limit
        ///</summary>
        [Test]
        public void LimitTest()
        {
            var target = new TimeBasedStopCriteria {Limit = new TimeSpan(0, 1, 0)};
            Assert.AreEqual(new TimeSpan(0, 1, 0), target.Limit);
        }

        /// <summary>
        ///A test for Complete
        ///</summary>
        [Test]
        public void CompleteTest()
        {
            var target = new TimeBasedStopCriteria { Limit = new TimeSpan(0, 0, 60) };

            SystemTime.Now = () => new DateTime(2009, 1, 1, 12, 0, 0);
            target.Reset();
            SystemTime.Now = () => new DateTime(2009, 1, 1, 12, 1, 0);

            Assert.AreEqual(true, target.Complete);
            
        }

        /// <summary>
        ///A test for Reset
        ///</summary>
        [Test]
        public void ResetTest()
        {
            var target = new TimeBasedStopCriteria();
            
            Assert.DoesNotThrow(target.Reset);
        }

    }
}