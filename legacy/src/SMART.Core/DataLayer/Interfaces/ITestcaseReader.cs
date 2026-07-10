using System.IO;
using SMART.Core.Interfaces;

namespace SMART.Core.DataLayer.Interfaces
{
    public interface ITestcaseReader
    {
        ITestcase Load(Stream stream);
    }
}