using System;
using System.ComponentModel.Composition;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;

namespace SMART.Base.Adapters
{
    [Export]
    [Export(typeof(IAdapter))]
    [Adapter(Name = "ConsoleAdapter")]
    public class ConsoleAdapter : IAdapter
    {
        public bool Execute(string function, params string[] args)
        {
            try
            {
				if(args==null)
				{
					Console.Out.WriteLine("{0}", function);
				}
				else
				{
					Console.Out.WriteLine("{0} {1}", function, string.Join(", ", args));
				}
            }
            catch (Exception e)
            {
                InvokeDefectDetected(new DefectEventArgs(null, e.Message));
                return false;
            }
            return true;
        }

        public event EventHandler<DefectEventArgs> DefectDetected;
        private void InvokeDefectDetected(DefectEventArgs e)
        {
            var tmp = DefectDetected;
            if (tmp != null) tmp(this, e);
        }

        public void PreExecution()
        {
        }

        public void PostExection()
        {
        }

        public void Dispose()
        {
            
        }
    }
}