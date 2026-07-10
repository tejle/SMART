using SMART.Core.DataLayer.Interfaces;
using SMART.Core.Interfaces;

namespace SMART.Core.DataLayer.Interfaces
{
    public interface IProjectIOHandler
    {
        IProjectReader ProjectReader
        {
            get;
            set;
        }

        IProjectWriter ProjectWriter
        {
            get;
            set;
        }

        IProject Load(string path);

        void Save(IProject project, string path);
    }
}