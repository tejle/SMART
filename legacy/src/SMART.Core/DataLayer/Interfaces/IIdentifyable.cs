using System;

namespace SMART.Core.DataLayer.Interfaces
{
    public interface IIdentifyable
    {
        Guid Id { get; set; }
    }
}