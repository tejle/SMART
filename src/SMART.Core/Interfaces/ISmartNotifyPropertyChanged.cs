using SMART.Core.Events;

namespace SMART.Core.Interfaces
{
    using System;

    public interface ISmartNotifyPropertyChanged
    {
        event EventHandler<SmartPropertyChangedEventArgs> PropertyChanged;
    }
}