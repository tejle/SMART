using System.ComponentModel;

namespace SMART.Core.Events
{

    using System;
    using Interfaces.Services;

    public class EventSubscription<TPayload> : IEventSubscription
    {

        public EventSubscription(Action<TPayload> action)
        {
            Action = action;
        }

        public Action<TPayload> Action { get; private set; }

        public Action<object> GetExectionStrategy()
        {
            return argument =>
            {
                TPayload arg = default(TPayload);
                //if (arguments != null && arguments.Length > 0 && arguments[0] != null)
                //{
                //    argument = (TPayload)arguments[0];
                //}
                arg = (TPayload) argument;
                InvokeAction(Action, arg);

            };
        }

        public virtual void InvokeAction(Action<TPayload> action, TPayload argument)
        {
            action(argument);

        }
    }
}