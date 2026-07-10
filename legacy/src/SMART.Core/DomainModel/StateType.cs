using System.ComponentModel;

namespace SMART.Core.DomainModel
{
    public enum StateType
    {
        [Description("Normal node")]
        Normal,

        [Description("Global model reference")]
        GlobalReference,

        [Description("Local model reference")]
        LocalReference
    }
}