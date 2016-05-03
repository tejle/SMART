using SMART.Core.Interfaces;
using System.IO;

namespace SMART.Core.DataLayer.Interfaces
{
    public interface IProjectReader
    {
        IProject Load(Stream stream);
    }
}