using System.ComponentModel.Composition;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

namespace SMART.Base.Sandboxes
{
    [Export]
    [Export(typeof(ISandbox))]
    public class Sandbox : ISandbox
    {
        public bool CanExecute(Transition transition)
        {
            return true;
        }

        public void Execute(Transition transition)
        {
        }

        public void Reset()
        {
        }
    }
}