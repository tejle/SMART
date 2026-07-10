using System;
using System.ComponentModel.Composition;
using SMART.Core.Interfaces.Metadata;

namespace SMART.Core.Metadata
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public class GenerationStopCriteriaAttribute : Attribute, IGenerationStopCriteriaMetadata
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
    }
}