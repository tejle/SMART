
namespace SMART.Core.Interfaces.Services
{
    using Events;
    public interface IEventService
    {
        TEvent GetEvent<TEvent>() where TEvent : SmartEventBase;
    }
}