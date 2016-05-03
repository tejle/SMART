using System;

namespace SMART.Core.Interfaces
{
    public interface IAdapter : IDisposable
    {
        bool Execute(string function, params string[] args);
        event EventHandler<DefectEventArgs> DefectDetected;

        void PreExecution();
        void PostExection();        
    }

    public interface IDefectDetected
    {
        event EventHandler DefectDetected;
    }

    public class DefectEventArgs : EventArgs {

        public IModelElement DefectElement{get; private set;}
        public string Message { get; private set; }
        
        public DefectEventArgs(IModelElement defectElement) : this(defectElement, string.Empty)
        {
        }

        public DefectEventArgs(IModelElement defectElement, string message) {
            DefectElement = defectElement;
            Message = message;
        }
    }
}