using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Wpf.Controls
{
    /// <summary>
    /// A Command Slider which implements ICommandSource and uses the Value as CommandParameter and supports a RangedValue if using CommandCanExecuteParameter.
    /// Also adds Precision (defaults to 10) and better behavior for IsMoveToPoint which is true by default
    /// </summary>
    public class XSlider : Slider, ICommandSource
    {
        public XSlider()
        {
            this.IsMoveToPointEnabled = true;
        }


        /// <summary>
        /// Sets the Precision. (defaults to 10)
        /// </summary>
        public int Precision
        {
            get { return (int)GetValue(PrecisionProperty); }
            set { SetValue(PrecisionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Precision.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PrecisionProperty =
            DependencyProperty.Register("Precision", typeof(int), typeof(XSlider), new UIPropertyMetadata(-1));


        protected override void OnValueChanged(double oldValue, double newValue)
        {
            //defaults to 10 and must be between 0 and 15
            int precision = Math.Min(15, Math.Max(0, Precision < 0 ? 10 : Precision));

            if (precision >= 0)
            {
                double value = Math.Round(newValue, precision);
                if (value != newValue)
                    Value = value;
            }

            base.OnValueChanged(oldValue, newValue);
        }

        private System.Windows.Controls.Primitives.Thumb _thumb;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_thumb != null)
            {
                _thumb.MouseEnter -= new MouseEventHandler(thumb_MouseEnter);
            }

            if (_thumb == null)
            {
                System.Windows.Controls.Primitives.Track track = this.Template.FindName(
                   "PART_Track", this) as System.Windows.Controls.Primitives.Track;
                if (track != null)
                    _thumb = track.Thumb;
            }

            if (_thumb != null)
            {
                _thumb.MouseEnter += thumb_MouseEnter;
            }
        }

        void thumb_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!this.IsMoveToPointEnabled)
                return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                // the left button is pressed on mouse enter
                // but the mouse isn't captured (when MouseEnter fires), so the thumb
                // must have been moved under the mouse in response
                // to a click on the track.
                // Generate a MouseLeftButtonDown event so that
                // the user can just drag the thumb without having to click again.

                var args = new MouseButtonEventArgs(
                    e.MouseDevice, e.Timestamp, MouseButton.Left);

                args.RoutedEvent = MouseLeftButtonDownEvent;
                (sender as System.Windows.Controls.Primitives.Thumb).RaiseEvent(args);
            }
        }

        #region Command dependency property and routed event

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public readonly static DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command",
            typeof(ICommand),
            typeof(XSlider),
            new PropertyMetadata(default(ICommand), new PropertyChangedCallback(OnCommandChanged)));

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            XSlider owner = (XSlider)d;
            owner.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
        }

        public static readonly RoutedEvent CommandChangedEvent = EventManager.RegisterRoutedEvent(
            "CommandChangedEvent",
            RoutingStrategy.Bubble,
            typeof(RoutedPropertyChangedEventHandler<ICommand>),
            typeof(XSlider));

        public event RoutedPropertyChangedEventHandler<ICommand> CommandChanged
        {
            add { AddHandler(CommandChangedEvent, value); }
            remove { RemoveHandler(CommandChangedEvent, value); }
        }

        protected virtual void OnCommandChanged(ICommand oldValue, ICommand newValue)
        {
            RoutedPropertyChangedEventArgs<ICommand> args = new RoutedPropertyChangedEventArgs<ICommand>(oldValue, newValue);
            args.RoutedEvent = XSlider.CommandChangedEvent;
            RaiseEvent(args);

            if (cmd_CanExecuteChangedHandler == null)
                cmd_CanExecuteChangedHandler = cmd_CanExecuteChanged;
            if (oldValue != null)
                oldValue.CanExecuteChanged -= cmd_CanExecuteChangedHandler;
            if (newValue != null)
                newValue.CanExecuteChanged += cmd_CanExecuteChangedHandler;
            else
                cmd_CanExecuteChangedHandler = null;

            UpdateCanExecute();
        }
        // hold a reference to it, it might me stored as a weak reference and never be called otherwise...
        EventHandler cmd_CanExecuteChangedHandler;
        void cmd_CanExecuteChanged(object sender, EventArgs e)
        {
            UpdateCanExecute();
        }

        void UpdateCanExecute()
        {
            if (IsCommandExecuting)
                return;

            ICommand cmd = Command;
            if (cmd == null)
            {
                IsEnabled = true;
            }
            else
            {
                try
                {
                    IsCommandExecuting = true;

                    var ca = new CommandCanExecuteParameter(CommandParameter);
                    bool enabled = CommandUtil.CanExecute(this, CommandParameter);
                    IsEnabled = enabled;
                    CommandUtil.CanExecute(this, ca);
                    if (enabled && ca.CurrentValue != null)
                    {
                        if (ca.CurrentValue is float)
                        {
                            Value = (float)ca.CurrentValue;
                        }
                        else if (ca.CurrentValue is double)
                        {
                            Value = (double)ca.CurrentValue;
                        }
                        if (ca.CurrentValue is RangedValue)
                        {
                            var rangedValue = (RangedValue)ca.CurrentValue;
                            Value = rangedValue.Value;
                            if (rangedValue.Minimum.HasValue)
                                Minimum = rangedValue.Minimum.Value;
                            if (rangedValue.Maximum.HasValue)
                                Maximum = rangedValue.Maximum.Value;
                        }
                    }
                }
                finally { IsCommandExecuting = false; }
            }
        }
        private bool IsCommandExecuting { get; set; }

        #endregion

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (IsInitialized
                && !IsCommandExecuting
                && e.Property == ValueProperty)
                try
                {
                    IsCommandExecuting = true;
                    CommandUtil.Execute(this);
                }
                finally { IsCommandExecuting = false; }
        }

        #region ICommandSource Members

        #region CommandTarget dependency property and routed event

        public IInputElement CommandTarget
        {
            get { return (IInputElement)GetValue(CommandTargetProperty); }
            set { SetValue(CommandTargetProperty, value); }
        }

        public readonly static DependencyProperty CommandTargetProperty = DependencyProperty.Register(
            "CommandTarget",
            typeof(IInputElement),
            typeof(XSlider),
            new PropertyMetadata(default(IInputElement), new PropertyChangedCallback(OnCommandTargetChanged)));

        private static void OnCommandTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            XSlider owner = (XSlider)d;
            owner.UpdateCanExecute();
        }

        #endregion

        public object CommandParameter
        {
            get
            {
                return Value;
            }
        }

        #endregion
    }
}
