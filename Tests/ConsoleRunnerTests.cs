using System.Linq;
using SMART.Core.BusinessLayer;
using SMART.Core.DataLayer;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.IOC;
using NUnit.Framework;
namespace SMART.Test
{
    public class ConsoleRunnerTests
    {
        private const string path = @"..\..\Demo Project\project3.smart";
        
        public ConsoleRunnerTests()
        {
            Resolver.Register<ITestcaseReader, TestcaseReader>();
            Resolver.Register<IModelReader, ModelReader>();
            Resolver.Register<IProjectReader, ProjectReader>();
            Resolver.Register<ITestcase, Testcase>();
            Resolver.Register<IProjectReader, ProjectReader>();
            
        }


       // [Fact]
        public void Load_project_should_return_a_project_given_a_path()
        {
            var projectLoader = Resolver.Resolve<ProjectIOHandler>();
            var project = projectLoader.Load(path);

            Assert.NotNull(project);
        }

       // [Fact]
        public void Save_project()
        {
            var projectLoader = Resolver.Resolve<ProjectIOHandler>();
            var project = projectLoader.Load(path);

            projectLoader.Save(project, path);
        }

       //// [Fact]
       // public void a_loaded_project_should_contain_testcase()
       // {
       //     var projectLoader = Resolver.Resolve<ProjectIOHandler>();
       //     var project = projectLoader.Load(path);

       //     Assert.IsNotEmpty(project.Testcases.ToList());
       //     Assert.IsNotEmpty(project.Testcases.First(t=>t.Name.StartsWith("Web")).ExecutionPath.ToList());
       // }
    }
}
