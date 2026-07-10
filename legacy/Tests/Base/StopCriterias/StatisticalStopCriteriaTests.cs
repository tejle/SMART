using SMART.Base.StopCriterias;
using NUnit.Framework;
using SMART.Core.Interfaces;

namespace SMART.Test.Base.StopCriterias
{
    using Moq;

    /// <summary>
    ///This is a test class for StatisticalStopCriteriaTest and is intended
    ///to contain all StatisticalStopCriteriaTest Unit Tests
    ///</summary>
    [TestFixture]
    public class StatisticalStopCriteriaTests
    {

        /// <summary>
        ///A test for StopLimit
        ///</summary>
        [Test]
        public void StopLimitTest()
        {
            var target = new StatisticalStopCriteria();
            double expected = 1F; 
            double actual;
            target.StopLimit = expected;
            actual = target.StopLimit;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Statistic
        ///</summary>
        [Test]
        public void StatisticTest()
        {
            var target = new StatisticalStopCriteria();
            var stats = new Mock<IStatistic>();
            IStatistic expected = stats.Object;
            IStatistic actual;
            target.Statistic = expected;
            actual = target.Statistic;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PercentComplete
        ///</summary>
        [Test]
        public void PercentComplete_should_return_50_percent()
        {
            var target = new StatisticalStopCriteria();
            target.StopLimit = 1;
            var stats = new Mock<IStatistic>();
            stats.Setup(s => s.Percent).Returns(0.5);
            target.Statistic = stats.Object;
            
            Assert.AreEqual(0.5, target.PercentComplete);
        }

        /// <summary>
        ///A test for Complete
        ///</summary>
        [Test]
        public void CompleteTest()
        {
            var target = new StatisticalStopCriteria();
            target.StopLimit = 1;
            var stats = new Mock<IStatistic>();
            stats.Setup(s => s.Percent).Returns(1.0);
            target.Statistic = stats.Object;
            
            Assert.AreEqual(true, target.Complete);
        }

        [Test]
        public void ResetTest()
        {
            var target = new StatisticalStopCriteria();
            Assert.DoesNotThrow(target.Reset);
        }

    }
}