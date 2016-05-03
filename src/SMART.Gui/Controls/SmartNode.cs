
using SMART.Core.DomainModel;

namespace SMART.Gui.Controls
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Commands;

    using Syncfusion.Windows.Diagram;

    

    public class SmartNode : Node, ISmartDiagramElement
    {
        public static readonly DependencyProperty IsVisitedProperty =
            DependencyProperty.Register("IsVisited", typeof(bool), typeof(SmartNode), new PropertyMetadata(false));

        public static readonly DependencyProperty VisitCountProperty =
            DependencyProperty.Register("VisitCount", typeof(int), typeof(SmartNode), new PropertyMetadata(0));

        public static readonly DependencyProperty IsCurrentProperty =
            DependencyProperty.Register("IsCurrent", typeof(bool), typeof(SmartNode), new PropertyMetadata(false));


        public static RoutedActionCommand SetNormalType = new RoutedActionCommand("SetNormalType", typeof(SmartNode))
        {
            Text = "Normal Node",
            OnExecute = o => SetNodeType(o, StateType.Normal),
            OnCanExecute = o => !IsNodeType(o, StateType.Normal)
        };

        public static RoutedActionCommand SetLocalReferenceType = new RoutedActionCommand("SetLocalReferenceType", typeof(SmartNode))
        {
            Text = "Local Model Reference",
            OnExecute = o => SetNodeType(o, StateType.LocalReference),
            OnCanExecute = o => !IsNodeType(o, StateType.LocalReference)
        };

        public static RoutedActionCommand SetGlobalReferenceType = new RoutedActionCommand("SetGlobalReferenceType", typeof(SmartNode))
        {
            Text = "Global Model Reference",
            OnExecute = o => SetNodeType(o, StateType.GlobalReference),
            OnCanExecute = o => !IsNodeType(o, StateType.GlobalReference)
        };

        public RoutedActionCommand Rename;
        
        private static void SetNodeType(object o, StateType type)
        {
            var node = o as SmartNode;
            if (node != null) node.NodeType = type;
        }

        private static bool IsNodeType(object o, StateType type)
        {
            var node = o as SmartNode;
            return node != null && node.NodeType == type;
        }

        public IEnumerable<RoutedActionCommand> Commands
        {
            get
            {
                //yield return SetNormalType;
                //yield return SetLocalReferenceType;
                //yield return SetGlobalReferenceType;
                yield return Rename;
            }
        }

        public State localState { get; set; }

        public int VisitCount
        {
            get
            {
                int count = 0; 
                Dispatcher.BeginInvoke((Func<int>)(() => count= (int)GetValue(VisitCountProperty)), DispatcherPriority.Normal);
                return count;
            }
            set { Dispatcher.BeginInvoke((Action)(() => SetValue(VisitCountProperty, value)), DispatcherPriority.Normal); }
        }

        public bool IsCurrent
        {
            get
            {
                bool current = false;
                Dispatcher.BeginInvoke((Func<bool>)(() => current = (bool)GetValue(IsCurrentProperty)), DispatcherPriority.Normal);
                return current;
            }
            set { Dispatcher.BeginInvoke((Action)(() => SetValue(IsCurrentProperty, value)), DispatcherPriority.Normal); }
        }

        public bool IsVisited
        {
            get { return (bool)GetValue(IsVisitedProperty); }
            set
            {
                Dispatcher.BeginInvoke((Action)(() => SetValue(IsVisitedProperty, value)), DispatcherPriority.Normal);

            }
        }

        public StateType NodeType
        {
            get { return localState.Type; }
            set
            {
                if (localState.Type == value) return;
                localState.Type = value;
                OnPropertyChanged("NodeType");
            }
        }

        public SmartNode(State state) : base(state.Id, "v" + state.GetHashCode())
        {            
            localState = state;
            NodeType = state.Type;

            OffsetX = state.Location.X;
            OffsetY = state.Location.Y;
            Label = state.Label;
            LabelVerticalAlignment = VerticalAlignment.Center;
            Height = Constants.NODE_HEIGHT;
            Width = Constants.NODE_WIDTH;
            //AllowRotate = false;
            Shape = Shapes.FlowChart_Process;

            //PortVisibility = Visibility.Visible;

            localState.PropertyChanged += State_PropertyChanged;

            CreateCommand();
        }

        private void CreateCommand()
        {
            Rename = new RoutedActionCommand("Rename", typeof(SmartNode))
                    {Description = "description", OnCanExecute = (o) => true, OnExecute = OnRename, Text = "Rename"};
        }
        private void OnRename(object obj)
        {
        //    SetInEditMode();
        }

        public void SetInEditMode()
        {            
            var node = new Node();
            var field = node.GetType().GetField("editor", BindingFlags.Instance | BindingFlags.NonPublic);
            var labeleditor = field.GetValue(this) as LabelEditor;
            if (labeleditor != null)
            {
                var method = (labeleditor).GetType().GetMethod(
                        "LabelEditStartInternal", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(labeleditor, new object[] {labeleditor});
            }
        }
    
        void State_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("VisitCount"))
            {
                VisitCount = localState.VisitCount;
                if (VisitCount > 0) IsVisited = true;
                OnPropertyChanged("VisitCount");
            }
            if (e.PropertyName.Equals("IsCurrent"))
            {
                IsCurrent = localState.IsCurrent;
                OnPropertyChanged("IsCurrent");
            }
        }


    }
}