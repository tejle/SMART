using System;
using SMART.Core.Metadata;

namespace SMART.Core.DataLayer.Interfaces
{
    public interface IConfigSetting
    {
        string Name { get; set; }
        ConfigAttribute Config { get; set; }
        object Value { get; set; }
        Type Type { get; set; }        
    }
}