using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMART.Core.Interfaces.Services;
using SMART.Core.Events;
using SMART.Core.Interfaces;

namespace SMART.Core.Agents
{
    public class ErrorReportingAgent : ISmartNotifyCollectionChanged, ISmartNotifyPropertyChanged, ISmartAgent
    {
        private readonly IEventService eventService;
        private List<ErrorMessage> errorMessages;
        private SmartAgentStatus status;

        public event SmartNotifyCollectionChangedEventHandler CollectionChanged;
        public event EventHandler<SmartPropertyChangedEventArgs> PropertyChanged;

        public SmartAgentStatus Status
        {
            get { return status; }
            private set { status = value; SendPropertyChanged(new SmartPropertyChangedEventArgs("Status")); }
        }

        public List<ErrorMessage> ErrorMessages
        {
            get { return errorMessages; }
        }

        public ErrorReportingAgent(IEventService eventService)
        {
            this.eventService = eventService;
            status = SmartAgentStatus.Stopped;
            errorMessages = new List<ErrorMessage>();
        }


        private void OnErrorEvent(ErrorMessage obj)
        {
            errorMessages.Add(obj);
            SendCollectionChanged(
                        new SmartNotifyCollectionChangedEventArgs(
                            SmartNotifyCollectionChangedAction.Add,
                            "ErrorMessages",
                            new[] { obj })
                        );
        }

        public void Run()
        {
            eventService.GetEvent<ErrorMessageEvent>().Subscribe(OnErrorEvent);
            Status = SmartAgentStatus.Running;
        }

        public void Stop()
        {
            eventService.GetEvent<ErrorMessageEvent>().Unsubscribe(OnErrorEvent);
            Status = SmartAgentStatus.Stopped;
        }

        private void SendPropertyChanged(SmartPropertyChangedEventArgs e)
        {
            EventHandler<SmartPropertyChangedEventArgs> changed = PropertyChanged;
            if (changed != null) changed(this, e);
        }

        private void SendCollectionChanged(SmartNotifyCollectionChangedEventArgs e)
        {
            SmartNotifyCollectionChangedEventHandler changed = CollectionChanged;
            if (changed != null) changed(this, e);
        }
    }
}
