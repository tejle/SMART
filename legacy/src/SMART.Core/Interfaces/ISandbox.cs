using SMART.Core.DomainModel;

namespace SMART.Core.Interfaces
{
    public interface ISandbox
    {
        bool CanExecute(Transition transition);
        void Execute(Transition transition);
        void Reset();
    }
}