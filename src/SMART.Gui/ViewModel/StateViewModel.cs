namespace SMART.Gui.ViewModel
{
    using System;
    using System.Windows;

    using Commands;

    using Controls.DiagramControl.Shapes;

    using Core.DomainModel;
    using Controls.DiagramControl;


    public sealed class StateViewModel : ShapeBaseViewModel, IConnectable, IDraggable
    {
        private State state;
        private bool isDragging;

        private bool isStartStop;

        public State State
        {
            get { return this.state; }
            set { this.state = value; }
        }

        public RoutedActionCommand MakeRef { get; set; }

        public bool CanConnect { get; set; }

        public Point Location { get { return new Point(Left, Top); } }

        public double Top
        {
            get { return state.Location.Y; }
            set { state.Location.Y = value; this.SendPropertyChanged("Top"); this.SendPropertyChanged("Location"); }
        }

        public double Left
        {
            get { return state.Location.X; }
            set { state.Location.X = value; this.SendPropertyChanged("Left"); this.SendPropertyChanged("Location"); }
        }

        public double Height
        {
            get { return state.Size.Height; }
            set { state.Size.Height = value; this.SendPropertyChanged("Height"); }
        }

        public double Width
        {
            get { return state.Size.Width; }
            set { state.Size.Width = value; SendPropertyChanged("Width"); }
        }

        public bool CanDrag { get; set; }

        public bool IsDragging
        {
            get { return isDragging; }
            set { isDragging = value; SendPropertyChanged("IsDragging"); }
        }

        public bool IsStartStop
        {
            get { return isStartStop; }
            set { isStartStop = value; SendPropertyChanged("IsStartStop"); }
        }

        public override string Name
        {
            get { return state.Label; }
            set { state.Label = value; SendPropertyChanged("Name"); }
        }

        public override Guid Id
        {
            get { return state.Id; }
            set { state.Id = value; }
        }




        public StateViewModel(State state)
            : base(state.Label)
        {
            this.state = state;
            isStartStop = state is StartState || state is StopState;
            this.CanDrag = true;
            this.CanConnect = true;
            this.state.PropertyChanged += state_PropertyChanged;

            MakeRef = new RoutedActionCommand("MakeRef", typeof(StateViewModel))
            {
                Description = "Make state to reference",
                OnCanExecute = (o) => !isStartStop,
                OnExecute = OnMakeRef,
                Text = "Is Reference",
                Icon = Constants.MISSING_ICON_URL
            };
        }

        public bool IsRef
        {
            get { return state.Type == StateType.GlobalReference; }
            set { this.state.Type = value ? StateType.GlobalReference : StateType.Normal; this.SendPropertyChanged("IsRef"); }
        }

        private void OnMakeRef(object obj)
        {
        }

        void state_PropertyChanged(object sender, Core.Events.SmartPropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("VisitCount"))
            {
                VisitCount = state.VisitCount;
            }
            else if (e.PropertyName.Equals("IsCurrent"))
            {
                IsCurrent = state.IsCurrent;
            }
            else if (e.PropertyName.Equals("IsDefect"))
            {
                IsDefect = state.IsDefect;
            }
        }

        public override void OnDelete(object obj)
        {
            if (View is StateShape)
                (View as StateShape).Delete();
        }

        public override bool OnCanRename(object obj)
        {
            return !isStartStop;
        }

        public override bool OnCanDelete(object obj)
        {
            return !isStartStop;
        }

        public override void ViewLoaded()
        {
            if (StartInEditMode)
            {
                IsInEditMode = true;
            }
        }
    }
}