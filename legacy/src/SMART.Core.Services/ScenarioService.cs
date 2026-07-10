using System.Linq;
using SMART.Core.Interfaces;

namespace SMART.Core.Services
{
    using Interfaces.Services;
    using IOC;
    using SMART.Core.Interfaces.Repository;
    using SMART.Core.DomainModel;

    public class ScenarioService : IScenarioService
    {
        public ITestcase CreateTestcase(string name)
        {
            var testcase = Resolver.Resolve<ITestcase>();
            testcase.Name = name;
            return testcase;
        }

    }
}