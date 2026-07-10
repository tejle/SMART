using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SMART.Core;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;

namespace SMART.Base.StopCriterias
{
    [Export]
    [Export(typeof(IStopCriteria))]
    [StopCriteria(Name = "And")]
    public class AndStopCriteria : IStopCriteria
    {
        public IEnumerable<IStopCriteria> Criterias { get; set; }

        public bool Complete
        {
            get { return Criterias.All(c=>c.Complete); }
        }

        public double PercentComplete
        {
            get { return Criterias.Average(c=>c.PercentComplete); }
        }

        public void Reset()
        {
            Criterias.ForEach(c=>c.Reset());
        }
    }

}
