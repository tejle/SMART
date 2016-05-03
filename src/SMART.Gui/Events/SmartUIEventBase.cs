using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using SMART.Core.Events;
using SMART.Core.Interfaces.Services;

namespace SMART.Gui.Events
{
    public class SmartUIEventBase:SmartEventBase
    {
        protected override void InternalPublish(object argument)
        {
            foreach (var subscription in subscriptions)
            {
                var action = subscription.GetExectionStrategy();
                Dispatcher.CurrentDispatcher.Invoke(action, argument);

            }
        }
    }
}
