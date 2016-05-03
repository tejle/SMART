namespace SMART.Gui.ViewModel
{
    using System;
    using Controls.DiagramControl;
    using System.Windows;
    using Controls.DiagramControl.Shapes;
    using Core.DomainModel;

    public class TransitionViewModel : ShapeBaseViewModel, IConnection
    {
        private readonly Transition transition;
        private IConnectable source;
        private IConnectable target;
        private Point startPoint;
        private Point endPoint;

        public Transition Transition
        {
            get { return transition; }
        }

        public double Top { get; set; }
        public double Left { get; set; }

        public override string Name
        {
            get
            {
                return transition == null ? string.Empty : transition.Label;
            }
            set { transition.Label = value; SendPropertyChanged("Name"); SendPropertyChanged("LabelAndParameters"); }
        }

        public override Guid Id
        {
            get { return transition.Id; }
            set { transition.Id = value; }
        }

        public IConnectable Source
        {
            get { return source; }
            set
            {
                if (source != null)
                {
                    source.PropertyChanged -= this.source_PropertyChanged;
                }
                source = value;
                source.PropertyChanged += this.source_PropertyChanged;
                this.SendPropertyChanged("Source");
            }
        }


        public IConnectable Target
        {
            get { return this.target; }
            set
            {
                if (target != null)
                {
                    target.PropertyChanged -= this.target_PropertyChanged;
                }
                target = value;
                target.PropertyChanged += this.target_PropertyChanged;
                this.SendPropertyChanged("Target");
            
            }
        }

        public Point StartPoint
        {
            get { return startPoint; }
            set { startPoint = value; this.SendPropertyChanged("StartPoint"); }
        }

        public Point EndPoint
        {
            get { return endPoint; }
            set { endPoint = value; this.SendPropertyChanged("EndPoint"); }
        }

        public string Parameters
        {
            get { return transition == null ? string.Empty : transition.Parameter.Replace(";", Environment.NewLine); }
            set
            {
                transition.Parameter = value.Replace(Environment.NewLine, ";");                 
                SendPropertyChanged("ParametersFormatted");
                SendPropertyChanged("LabelAndParameters");
            }
        }

        public string ParametersFormatted
        {
            get { return transition == null ? string.Empty : transition.Parameter; }
            set { Parameters = value; }
        }

        public string LabelAndParameters
        {
            get { return Name + " " + ParametersFormatted; }
        }

        private bool isTemp;

        public bool IsTemp
        {
            get { return this.isTemp; }
            set { this.isTemp = value; this.SendPropertyChanged("IsTemp"); }
        }

        public TransitionViewModel()
        {
        }

        public TransitionViewModel(Transition transition, IConnectable source, IConnectable target)
            : base(transition.Label)
        {
            this.Source = source;
            this.Target = target;
            this.transition = transition;
            transition.PropertyChanged += this.transition_PropertyChanged; 
        }

        void target_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Location"))
            {
                this.SendPropertyChanged("Target");
                //this.SendPropertyChanged("StartPoint");
                //this.SendPropertyChanged("EndPoint");
            }
        }

        void source_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Location"))
            {
                this.SendPropertyChanged("Source");
                //this.SendPropertyChanged("StartPoint");
                //this.SendPropertyChanged("EndPoint");
            }
        }

        void transition_PropertyChanged(object sender, SMART.Core.Events.SmartPropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("VisitCount"))
            {
                VisitCount = transition.VisitCount;
            }
            else if (e.PropertyName.Equals("IsCurrent"))
            {
                IsCurrent = transition.IsCurrent;
            }
            else if (e.PropertyName.Equals("IsDefect"))
            {
                IsDefect = transition.IsDefect;
            }
        }

        public override void ViewLoaded()
        {
            if (StartInEditMode)
            {
                IsInEditMode = true;
            }
        }

        public override void OnDelete(object obj)
        {
            (View as TransitionShape).Delete();
        }

    }
}