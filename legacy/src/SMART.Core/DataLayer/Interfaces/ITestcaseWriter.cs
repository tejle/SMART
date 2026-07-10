using System.Collections.Generic;
using System.IO;
using SMART.Core.Interfaces;


namespace SMART.Core.DataLayer.Interfaces
{
    public interface ITestcaseWriter : IWriter<ITestcase>
    {
    }

    public interface IWriter<T>
    {
        void Save(Stream stream, T item);
    }
}