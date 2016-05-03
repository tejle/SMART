using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using SMART.Core;
using SMART.Core.Events;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Services;


namespace SMART.Gui.Events
{
    using Controls.DiagramControl;

    using Core.DomainModel;

    using SMART.Gui.ViewModel;

    public class SmartUIEvent<TPayload> : SmartUIEventBase
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

    public class OpenModelViewEvent : SmartUIEvent<IModel>{}

    public class OpenProjectEvent : SmartUIEvent<string> { }

    public class SaveProjectEvent : SmartUIEvent<string> { }

    public class OpenTestcaseViewEvent : SmartUIEvent<ITestcase>{}

    public class DiagramElementAddEvent : SmartUIEvent<ISelectable>{}


    public class LayoutEvent : SmartUIEvent<object>{}
    public class LayoutCompleteEvent : SmartUIEvent<IModel>{}
    public class ActivateModelEvent : SmartUIEvent<IModel>{}
    public class DeactivateModelEvent : SmartUIEvent<IModel>{}

    public class OpenPopUpEvent : SmartUIEvent<OpenPopUpEventArgs> { }

    public class OpenPopUpEventArgs
    {
        private Type type;

        public Type Type
        {
            get { return this.type; }
        }

        public Guid Id
        {
            get { return this.id; }
        }

        private Guid id;

        public OpenPopUpEventArgs(Type type, Guid id)
        {
            this.type = type;
            this.id = id;
        }
    }
    public class ClosePopUpEvent : SmartUIEvent<IViewModel> { }
}