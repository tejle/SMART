
namespace SMART.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Events;
    using Interfaces.Services;

    public class EventService : IEventService
    {
        private readonly List<SmartEventBase> events = new List<SmartEventBase>();

        public TEvent GetEvent<TEvent>() where TEvent : SmartEventBase
        {
            TEvent eventInstance = events.FirstOrDefault(e => e.GetType() == typeof(TEvent)) as TEvent;
            if (eventInstance == null)
            {
                eventInstance = Activator.CreateInstance<TEvent>();
                events.Add(eventInstance);
            }

            return eventInstance;
        }
    }
}