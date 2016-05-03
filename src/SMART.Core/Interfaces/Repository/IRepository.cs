using System.Collections.Generic;
using SMART.Core.Metadata;

namespace SMART.Core.Interfaces.Repository
{
    public interface IRepository
    {
        IEnumerable<ClassDescription> GetAll();
    }
}