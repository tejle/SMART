using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SMART.Base.Sandboxes;
using SMART.Core.Model;
using SMART.Core.Model.ProjectStructure;
using SMART.IOC;

namespace SMART.Core.UnitTests
{
    [TestFixture]
    public class ResolveTests
    {

        [Test]
        public void resolve_testcase()
        {
            Resolver.Configure();
            Resolver.Register<ISandbox, Sandbox>(); 
            var testcase = Resolver.Resolve<TestCase>();

            Assert.IsNotNull(testcase);
        }
    }
}
