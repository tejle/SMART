


using System.Windows.Controls;

namespace SMART.Gui.ViewModel
{
    using System;
    using System.ComponentModel;
    using System.Windows.Threading;
    using System.Diagnostics;

    public abstract class ViewModelBase : IViewModel, INotifyPropertyChanged
    {
        public abstract string Icon { get; }

        private string name;
        public virtual string Name
        {
            get { return name; }
            set { name = value; SendPropertyChanged("Name"); }
        }

        private readonly Dispatcher _dispatcher;

        protected  ViewModelBase():this(string.Empty){}

        protected ViewModelBase(string name)
        {
            this.name = name ?? string.Empty;
            _dispatcher = Dispatcher.CurrentDispatcher;
        }


        [Conditional("Debug")]
        protected void VerifyCalledOnUIThread()
        {
            Debug.Assert(Dispatcher.CurrentDispatcher == _dispatcher, "Call must be made on UI thread.");
        }

        ///<summary>
        ///PropertyChanged event for INotifyPropertyChanged implementation.
        ///</summary>
        private PropertyChangedEventHandler _propertyChangedEvent;

        private bool ThrowOnInvalidPropertyName = false;

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                VerifyCalledOnUIThread();
                _propertyChangedEvent += value;
            }
            remove
            {
                VerifyCalledOnUIThread();
                _propertyChangedEvent -= value;
            }
        }

        public void SendPropertyChanged(string propertyName)
        {
            VerifyCalledOnUIThread();

            this.VerifyPropertyName(propertyName);

            if (_propertyChangedEvent != null)
            {
                _propertyChangedEvent(this, new PropertyChangedEventArgs(propertyName));
            }
            
        }

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }


        public abstract Guid Id
        {
            get;
            set;
        }

        public virtual void ViewLoaded()
        {
            
        }

      public virtual ContentControl View
      {
        get; set;
      }
    }

}
