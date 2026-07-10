using System.ComponentModel;

namespace SMART.Core.Interfaces.Metadata
{
    public interface IGenerationStopCriteriaMetadata
    {
        [DefaultValue("No description available.")]
        string Description { get; }

        [DefaultValue("0.0.0.1")]
        string Version { get; }

        [DefaultValue("N/A")]
        string Name { get; }
    }
}