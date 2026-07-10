using System.ComponentModel;

namespace SMART.Core.Events
{
    public enum SmartPropertyChangedAction
    {
        Add,
        Remove,
        None
    }

    public class SmartPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        private readonly object item;

        private readonly SmartPropertyChangedAction action;
        public object Item
        {
            get { return this.item; }
        }

        public SmartPropertyChangedAction Action
        {
            get { return this.action; }
        }

        public SmartPropertyChangedEventArgs(string propertyName) : this(propertyName, null)
        {
        }

        public SmartPropertyChangedEventArgs(string propertyName, object item) : this(propertyName, item, SmartPropertyChangedAction.None)
        {
        }

        public SmartPropertyChangedEventArgs(string propertyName, object item, SmartPropertyChangedAction action) : base(propertyName)
        {
            this.item = item;
            this.action = action;
        }
    }
}