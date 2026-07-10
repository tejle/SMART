using System;
using SMART.Core.Model.ProjectStructure;
using SMART.Core.DataLayer;
using SMART.IOC;

namespace SMART.Console
{
    public class ProjectLoader
    {
        public IProject Load(string path)
        {
            
            var projectReader = Resolver.Resolve<IProjectReader>();
            return projectReader.LoadProject(path);

        }

        public void Save(IProject project, string path)
        {
            throw new NotImplementedException();
        }
    }
}