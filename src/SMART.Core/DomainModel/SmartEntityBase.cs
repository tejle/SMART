using System;
using System.Collections;
using System.Collections.Generic;
using SMART.Core.Events;
using SMART.Core.Interfaces;


namespace SMART.Core.DomainModel
{
    public abstract class SmartEntityCollectionBase : SmartEntityBase, ISmartNotifyCollectionChanged
    {
        public event SmartNotifyCollectionChangedEventHandler CollectionChanged;

        protected virtual void SendCollectionChanged(SmartNotifyCollectionChangedAction action, string collecitonName, IList items) {
            var tmp = CollectionChanged;
            if (tmp != null)
                tmp(this, new SmartNotifyCollectionChangedEventArgs(action, collecitonName, items));
        }
    }
    public abstract class SmartEntityBase : ISmartNotifyPropertyChanged
    {
        public event EventHandler<SmartPropertyChangedEventArgs> PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName, object item, SmartPropertyChangedAction action)
        {
            var tmp = PropertyChanged;
            if (tmp != null)
            {
                tmp(this, new SmartPropertyChangedEventArgs(propertyName, item, action));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName, object item)
        {
            OnPropertyChanged(propertyName, item, SmartPropertyChangedAction.None);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(propertyName, null);
        }
    }
}