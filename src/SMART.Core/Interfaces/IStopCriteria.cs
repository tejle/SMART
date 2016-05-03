using System;
using System.Collections.Generic;
using System.Text;

namespace SMART.Core.Interfaces
{
    public interface IStopCriteria
    {
        bool Complete { get; }
        double PercentComplete { get; }
        void Reset();
    }
}
