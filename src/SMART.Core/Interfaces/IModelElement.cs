using System;
using System.Collections.Generic;
using SMART.Core.DataLayer.Interfaces;

namespace SMART.Core.Interfaces
{
    public interface IModelElement : IIdentifyable
    {
        bool IsCurrent { get; set; }
        string Label { get; set; }
        int VisitCount { get; set; }
        bool IsDefect { get; set; }

        void Visit();
        void Reset();
        Dictionary<string, object> Tags { get; set; }
    }
}