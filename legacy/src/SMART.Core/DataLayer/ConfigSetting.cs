using System;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.Metadata;
using SMART.Core.DataLayer;

namespace SMART.Core.DataLayer
{
    public class ConfigSetting : IConfigSetting
    {
        public string Name { get; set; }

        public ConfigAttribute Config { get; set; }

        public object Value { get; set; }

        public Type Type { get; set; }
    }
}