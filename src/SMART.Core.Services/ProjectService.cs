using SMART.Core.DataLayer.Interfaces;
using SMART.Core.Interfaces;

namespace SMART.Core.Services
{
    using Interfaces.Services;
    using IOC;
    using Base;

    public class ProjectService : IProjectService
    {
        
        public IProject OpenProjectFromFile(string fileName)
        {           
            var ioHandler = Resolver.Resolve<IProjectIOHandler>();
            return ioHandler.Load(fileName);
        }

        public void SaveProjectToFile(IProject project, string filename)
        {
            var ioHandler = Resolver.Resolve<IProjectIOHandler>();
            ioHandler.Save(project, filename);
        }

        public IProject CreateProject(bool createDefaultData)
        {
            var project = Resolver.Resolve<IProject>();
            project.Name = string.Format("New project {0}", SystemTime.Now().ToString("yyyyMMddHHmm"));
            if (createDefaultData)
            {
                var scenario = new ScenarioService().CreateTestcase("Scenario 1");
                var model = new ModelService().CreateModel("Model 1");
                
                project.AddTestCase(scenario);
                project.AddModel(model, scenario);
            }
            return project;
        }
    }
}