using System;

namespace SMART.Core.Metadata
{
    [AttributeUsage(AttributeTargets.Property,Inherited = true)]
    public sealed class ConfigAttribute : Attribute
    {
        public object Default { get; set; }
        public string Description { get; set; }
        public ConfigEditor Editor { get; set; }
    }
}