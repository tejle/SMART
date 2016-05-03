using System.IO;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.Interfaces;
using SMART.Core.DataLayer;

namespace SMART.Core.DataLayer
{
    public class ProjectIOHandler : IProjectIOHandler
    {
        public IProjectReader ProjectReader { get; set; }

        public IProjectWriter ProjectWriter { get; set; }

        public ProjectIOHandler(IProjectReader projectReader, IProjectWriter projectWriter)
        {
            ProjectReader = projectReader;
            ProjectWriter = projectWriter;
        }

        public IProject Load(string path)
        {
            using (var stream = File.OpenRead(path))
            {
                return ProjectReader.Load(stream);                                
            }
        }

        public void Save(IProject project, string path)
        {
            using (var stream = File.Create(path))
            {
                ProjectWriter.Save(stream, project);                
            }
        }
    }
}