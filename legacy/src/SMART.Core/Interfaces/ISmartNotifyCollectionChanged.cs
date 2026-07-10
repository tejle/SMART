using SMART.Core.Events;

namespace SMART.Core.Interfaces
{
    public interface ISmartNotifyCollectionChanged
    {
        event SmartNotifyCollectionChangedEventHandler CollectionChanged;
    }
}