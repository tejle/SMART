using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMART.Core.Interfaces.Services
{
    public interface IProjectService 
    {
        IProject OpenProjectFromFile(string fileName);

        void SaveProjectToFile(IProject project, string filename);

        IProject CreateProject(bool createDefaultData);
    }
}