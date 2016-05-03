using System;

namespace SMART.Core.Interfaces.Services
{
    public interface IEventSubscription
    {
        Action<object> GetExectionStrategy();
    }
}