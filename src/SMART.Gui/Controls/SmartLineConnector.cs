
using System;
using System.Windows;
using SMART.Core.DomainModel;

namespace SMART.Gui.Controls
{
    using System.ComponentModel;
    using Syncfusion.Windows.Diagram;
    
    public class SmartLineConnector : LineConnector, ISmartDiagramElement
    {
        private Point headPosition;
        private Point tailPosition;
        public Core.DomainModel.Transition Transition { get; set; }

        public SmartLineConnector(IShape source, IShape destination, Transition transition)
        {
            Transition = transition;
            HeadNode = destination;
            TailNode = source;

            IsDirected = true;
            IsEnabled = true;
            IsLabelEditable = true;                       

            HeadDecoratorShape = DecoratorShape.Arrow;
            TailDecoratorShape = DecoratorShape.None;
            ConnectorType = ConnectorType.Straight;
            Label = transition.Label;
            Transition.PropertyChanged += Transition_PropertyChanged;
        }

        public void SetInEditMode()
        {
            this.Labeledit();
        }

        void Transition_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("VisitCount"))
            {
                VisitCount = Transition.VisitCount;
                OnPropertyChanged("VisitCount");
            }
        }

        protected override void Line_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName.Equals("VisitCount") || e.PropertyName.Equals("IsCurrent")) return;
            if (HeadNode != null && HeadNode.Equals(sender))
            {
                if (headPosition.X == HeadNode.OffsetX && headPosition.Y == HeadNode.OffsetY)
                    return;
                headPosition = new Point(HeadNode.OffsetX, HeadNode.OffsetY);
            }
            if (TailNode != null && TailNode.Equals(sender))
            {
                if (tailPosition.X == TailNode.OffsetX && tailPosition.Y == TailNode.OffsetY)
                    return;
                tailPosition = new Point(TailNode.OffsetX, TailNode.OffsetY);
            }
            base.Line_PropertyChanged(sender, e);
            
        }

        public int VisitCount { get; private set; }

        public override string ToString()
        {
            return string.Format("Head: {0}, Tail: {1}", this.HeadNode.ID, this.TailNode.ID);
        }
    }
}