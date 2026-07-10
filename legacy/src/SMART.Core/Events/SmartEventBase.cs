
using System;
using System.Linq;

namespace SMART.Core.Events
{
    using System.Collections.Generic;
    using Interfaces.Services;

    public class SmartEventBase
    {
        protected readonly List<IEventSubscription> subscriptions = new List<IEventSubscription>();
        protected ICollection<IEventSubscription> Subscriptions { get { return subscriptions; } }

        protected virtual void InternalSubscribe(IEventSubscription eventSubscription)
        {
            lock (subscriptions)
            {
                subscriptions.Add(eventSubscription);
            }
        }

        protected virtual void InternalUnsubscribe(IEventSubscription eventSubscription)
        {
            lock (subscriptions)
            {
                subscriptions.Remove(eventSubscription);
            }
        }

        protected virtual void InternalPublish(object argument)
        {
            foreach (var subscription in subscriptions)
            {
                var action = subscription.GetExectionStrategy();
                action(argument);
            }
        }
    }

    
  
}
