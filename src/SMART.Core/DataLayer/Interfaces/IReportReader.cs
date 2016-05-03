using System.IO;
using SMART.Core.Interfaces.Reporting;

namespace SMART.Core.DataLayer.Interfaces
{
    public interface IReportReader
    {
        IReport Load(Stream stream);
    }
}