


namespace SMART.Core.Interfaces
{
    public interface IStep
    {
        string Function { get; }
        string[] Parameters { get; }
        IModelElement ModelElement { get; }
    }
}