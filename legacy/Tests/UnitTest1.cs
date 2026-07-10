using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Moq;
using NUnit.Framework;
using SMART.Base.Algorithms;
using SMART.Base.StopCriterias;
using SMART.Core;
using SMART.Core.BusinessLayer;
using SMART.Core.DataLayer;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

using SMART.IOC;
using SMART.Base.Adapters;
using System.IO;
using SMART.Base.Sandboxes;
using SMART.Core.Metadata;

namespace SMART.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestFixture]
    public class UnitTest1
    {
        public class TestClass
        {
            [Import]
            public Lazy<FlatFileAdapter> FlatFileAdapter;
            [Import]
            public Lazy<BreadthFirstAlgorithm> BreadthFirstAlgorithm;
            [Import]
            public Lazy<RandomAlgorithm> RandomAlgorithm;
            [Import]
            public Lazy<StatisticalStopCriteria> StatisticalStopCriteria;
            [Import]
            public Lazy<Sandbox> SimpleSandbox;
            [Import]
            public Lazy<InterfaceAdapter> InterfaceAdapter;
            [Import]
            public Lazy<AssistedRandomAlgorithm> AssistedRandomAlgorithm;
            [Import]
            public Lazy<ConsoleAdapter> ConsoleAdapter;

            public IModel SimpleModel { get { return GetSimpleModel(); } }

            private static IModel GetSimpleModel()
            {
                var v1 = new StartState { Label = "Start" };
                var v2 = new State { Label = "V2" };
                var v3 = new State { Label = "V3" };
                var v4 = new StopState { Label = "Stop" };
                var e1 = new Transition { Source = v1, Destination = v2, Label = "E1" };
                var e2 = new Transition { Source = v2, Destination = v3, Label = "E2" };
                var e3 = new Transition { Source = v3, Destination = v4, Label = "E3" };
                return new Model().Add(new[] { e1, e2, e3 }).Add(new[] { v1, v2, v3, v4 });
            }
        }


        [SetUp]
        public void setup() {
            Resolver.Register<IModelReader, ModelReader>();
            Resolver.Register<ITestcaseReader, TestcaseReader>();
            
            Resolver.Register<ISandbox, Sandbox>();
            Resolver.RegisterInstance<Random>(new Random());
        }

      

        [Test]
        public void ExtentionsOfConfigAttributeObjects_using_nonconfigurable_object_returns_empty_config()
        {
            var testObject = new NonConfigurableTestClass();
            var config = testObject.GetConfig();
            Assert.IsNotNull(config);
            Assert.AreEqual(0, config.Count);
        }

        [Test]
        public void ExtentionsOfConfigAttributeObjects_using_configurable_object_returns_config()
        {
            var testObject = new ConfigurableTestClass();
            var config = testObject.GetConfig();
            Assert.IsNotNull(config);
            Assert.AreEqual(2, config.Count);
        }

        [Test]
        public void ExtentionsOfConfigAttributeObjects_using_config_configures_configurable_object()
        {
            var testObject = new ConfigurableTestClass();
            var config = testObject.GetConfig();

            config.Update("Setting", "Set");

            Assert.IsNull(testObject.Setting);
            Assert.IsNull(testObject.SettingWithDefault);

            testObject.SetConfig(config);

            Assert.AreEqual("Set", testObject.Setting);
            Assert.AreEqual("Default", testObject.SettingWithDefault);

            config.Update("SettingWithDefault", "DefaultOverride");

            testObject.SetConfig(config);

            Assert.AreEqual("Set", testObject.Setting);
            Assert.AreEqual("DefaultOverride", testObject.SettingWithDefault);
        }

        public class NonConfigurableTestClass
        {
            public string Setting { get; set; }
        }

        public class ConfigurableTestClass
        {
            [Config]
            public string Setting { get; set; }

            [Config(Default = "Default")]
            public string SettingWithDefault { get; set; }
        }
    }
}
