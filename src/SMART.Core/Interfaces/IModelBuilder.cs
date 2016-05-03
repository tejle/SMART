using System.Collections.Generic;
using SMART.Core.DomainModel;

namespace SMART.Core.Interfaces
{
    public interface IModelBuilder
    {
        Model Merge(IEnumerable<Model> models, Model root);

    }
}