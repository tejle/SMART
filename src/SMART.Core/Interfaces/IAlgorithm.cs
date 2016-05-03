
using System;
using System.Collections.Generic;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;

namespace SMART.Core.Interfaces
{
    public interface IAlgorithm 
    {
        event EventHandler<ModelElementVistedEventArgs> ModelElementVisted;
        
        IModel Model { get; set; }
        IModelElement Current { get; }
        IExecutionEnvironment ExecutionEnvironment { get; set; }
        
        bool MoveNext();
        void Reset();
    }

    public class ModelElementVistedEventArgs : EventArgs
    {
        public IModelElement Element { get; set; }

        public ModelElementVistedEventArgs(IModelElement element )
        {
            Element = element;
        }
    }
}