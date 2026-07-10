

namespace SMART.Core.Events
{
    using System;
    using System.Linq;
    using Interfaces.Services;
    
    public class SmartEvent<TPayload> : SmartEventBase
    {
        public void Subscribe(Action<TPayload> action)
        {
            base.InternalSubscribe(new EventSubscription<TPayload>(action));
        }

        public void Unsubscribe(Action<TPayload> subscriber)
        {
            IEventSubscription subscription =
                Subscriptions.Cast<EventSubscription<TPayload>>().FirstOrDefault(evt => evt.Action == subscriber);
            if (subscription != null)
                base.InternalUnsubscribe(subscription);
        }

        public void Publish(TPayload payload)
        {
            base.InternalPublish(payload);
        }
    }
}