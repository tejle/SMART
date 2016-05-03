using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SMART.Base.Sandboxes;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.IOC;

namespace SMART.Test.Core
{
    [TestFixture]
    public class ResolveTests
    {

        [Test]
        [Ignore]
        public void resolve_testcase()
        {
            Resolver.Configure();
            Resolver.Register<ISandbox, Sandbox>(); 
            var testcase = Resolver.Resolve<Testcase>();

            Assert.IsNotNull(testcase);
        }
    }
}