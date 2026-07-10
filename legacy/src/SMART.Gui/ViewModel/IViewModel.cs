using System.Windows.Controls;
using SMART.Core.Interfaces;

namespace SMART.Gui.ViewModel
{
    using System;
    using System.ComponentModel;

    public interface IViewModel
    {
        event PropertyChangedEventHandler PropertyChanged;

        Guid Id { get; set; }

        void ViewLoaded();

        ContentControl View { get; set; }
    }
}
