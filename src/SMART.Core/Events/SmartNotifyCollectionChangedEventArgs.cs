using System.Collections;

namespace SMART.Core.Events
{
    public class SmartNotifyCollectionChangedEventArgs
    {
        public SmartNotifyCollectionChangedAction Action { get; private set; }
        public string CollectionName { get; private set; }
        public IList NewItems { get; private set; }
        public int NewStartingIndex { get; private set; }
        public IList OldItems { get; private set; }
        public int OldStartingIndex { get; private set; }

        public SmartNotifyCollectionChangedEventArgs(SmartNotifyCollectionChangedAction action, string collectionName, IList items)
        {
            Action = action;
            CollectionName = collectionName;
            switch (action)
            {
                case SmartNotifyCollectionChangedAction.Add:
                    NewItems = items;
                    break;
                case SmartNotifyCollectionChangedAction.Remove:
                    OldItems = items;
                    break;
            }
        }
    }
}