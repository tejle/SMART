using SMART.Core.Interfaces;
using System.IO;

namespace SMART.Core.DataLayer.Interfaces
{
    public interface IProjectWriter
    {
        void Save(Stream stream, IProject project);
    }
}