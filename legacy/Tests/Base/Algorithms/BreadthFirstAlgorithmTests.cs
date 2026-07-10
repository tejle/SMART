using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using NUnit.Framework;
using SMART.Base.Adapters;
using SMART.Base.Algorithms;
using SMART.Base.Sandboxes;
using SMART.Base.Statistics;
using SMART.Base.StopCriterias;
using SMART.Core;
using SMART.Core.BusinessLayer;
using SMART.Core.DataLayer;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

using SMART.IOC;

namespace SMART.Test.Base.Algorithms
{
    public class BreadthFirstAlgorithmTests
    {
        //[SetUp]
        public void Setup()
        {
            Resolver.Register<ITestcaseReader, TestcaseReader>();
            Resolver.Register<IModelReader, ModelReader>();
            Resolver.Register<IProjectReader, ProjectReader>();
            Resolver.Register<ITestcase, Testcase>();
            Resolver.Register<IProjectReader, ProjectReader>();
        }


        ////[Test]
        //public void algorithm_walk()
        //{
        //    var model = GetSimpleModel();
        //    var TestContainer = Resolver.Resolve<TestClass>();

        //    var testcase = Resolver.Resolve<Testcase>();
        //    testcase.Models = new[] {model};
            
        //    var stat = testcase.StatisticsManager.Statistics.OfType<TransitionCoverageStatistic>().First();
        //    TestContainer.StatisticalStopCriteria.Statistic = stat;
        //    TestContainer.StatisticalStopCriteria.StopLimit = 2;

        //    //TestContainer.BreadthFirstAlgorithm.StopCriteria = TestContainer.StatisticalStopCriteria;

        //    var adapters = new List<IAdapter> { TestContainer.FlatFileAdapter };
        //    testcase.Sandbox = TestContainer.SimpleSandbox;
        //    testcase.Adapters = adapters;
        //    testcase.Algorithms = new[] { TestContainer.BreadthFirstAlgorithm };
        //    testcase.StartExecution();
            
        //}

        private static Model GetSimpleModel()
        {
            var v1 = new State { Label = "Start" };
            var v2 = new State { Label = "B" };
            var v3 = new State { Label = "C" };
            var v4 = new State { Label = "D" };
            var v5 = new State { Label = "E" };
            var v6 = new State { Label = "F" };
            var e1 = new Transition { Source = v1, Destination = v2, Label = "AB" };
            var e2 = new Transition { Source = v2, Destination = v3, Label = "BC" };
            var e3 = new Transition { Source = v2, Destination = v4, Label = "BD" };
            var e4 = new Transition { Source = v3, Destination = v5, Label = "CE" };
            var e5 = new Transition { Source = v4, Destination = v6, Label = "DF" };
            var e6 = new Transition {Source = v4, Destination = v2, Label = "DB"};
            
            var model = Resolver.Resolve<Model>();
            model.Add(v1);
            model.Add(v2);
            model.Add(v3);
            model.Add(v4);
            model.Add(v5);
            model.Add(v6);

            model.Add(e1);
            model.Add(e2);
            model.Add(e3);
            model.Add(e4);
            model.Add(e5);
            model.Add(e6);
            return model;
        }
    }
}