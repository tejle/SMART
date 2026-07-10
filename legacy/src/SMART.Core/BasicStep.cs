using System.Linq;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

namespace SMART.Core
{
    public class BasicStep : IStep
    {
        public BasicStep(IModelElement element)
        {
            ModelElement = element;
        }

        public string Function {
            get { return ModelElement.Label; } 
        }

        public string[] Parameters
        {
            get { return (ModelElement is Transition) ? ((Transition) ModelElement).Parameters.ToArray() : null; }
        }

        public IModelElement ModelElement { get; set; }

        public override string ToString() {
            return string.Format("{0} {1}", Function, (string.Join(",", Parameters)) );
        }
    }
}