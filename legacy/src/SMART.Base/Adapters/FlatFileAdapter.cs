using System;
using System.ComponentModel.Composition;
using System.IO;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;

namespace SMART.Base.Adapters
{
    [Export]
    [Export(typeof(IAdapter))]
    [Adapter(Name="FlatFileAdapter")]
    public class FlatFileAdapter : IAdapter
    {
        [Config]
        public string FileName { get; set; }

        public bool Execute(string function, params string[] args)
        {
            try
            {
                using (var writer = new StreamWriter(FileName, true))
                {
                    writer.WriteLine("{0} {1}", function, string.Join(", ", args));
                }
            }
            catch (Exception e)
            {
                DefectDetected(this, new DefectEventArgs(null, e.Message));
                return false;
            }
            return true;
        }

        public event EventHandler<DefectEventArgs> DefectDetected;

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
