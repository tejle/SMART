using System;
using System.IO;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;


namespace SMART.Core.DataLayer.Interfaces
{
    public interface IModelReader
    {
        IModel Load(Stream stream);
    }
}