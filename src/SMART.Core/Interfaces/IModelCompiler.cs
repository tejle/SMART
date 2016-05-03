namespace SMART.Core.DomainModel
{
    using System.Collections.Generic;

    using Interfaces;

    public interface IModelCompiler {
        IModel Compile(IEnumerable<IModel> models);
        IExecutionEnvironment	CreateSandbox(IModel model);
        IStep CreateStep(IModelElement modelElement);
    }
}