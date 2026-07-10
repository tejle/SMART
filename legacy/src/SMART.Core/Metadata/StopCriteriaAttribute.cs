using System;
using System.ComponentModel.Composition;
using SMART.Core.Interfaces.Metadata;

namespace SMART.Core.Metadata
{
    [MetadataAttribute]
    public sealed class StopCriteriaAttribute : Attribute, IStopCriteriaMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
    }
}