using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SMART.Core.Interfaces;

namespace SMART.Core.DomainModel.Layouts
{
    public class LayoutModel : ModelDecorator
    {
        private List<LayoutState> states;
        private List<LayoutTransition> transitions;
        private LayoutState startState;
        private LayoutState stopState;

        public LayoutModel(IModel model)
            : base(model)
        {
        }

        public new List<LayoutState> States
        {
            get
            {
                if (states == null)
                    states = new List<LayoutState>(from s in Model.States select new LayoutState(s));
                return states;
            }
            set { states = value; }
        }

        public new List<LayoutTransition> Transitions
        {
            get
            {
                if (transitions == null)
                    transitions = new List<LayoutTransition>(from t in Model.Transitions select new LayoutTransition(t));
                return transitions;
            }
            set { transitions = value; }
        }

        public new LayoutState StartState
        {
            get
            {
                if(startState == null)
                    startState = new LayoutState(Model.StartState);
                return startState;
            }
            set { startState = value; }
        }
        public new LayoutState StopState
        {
            get
            {
                if (stopState == null)
                    stopState = new LayoutState(Model.StopState);
                return stopState;
            }
            set { startState = value; }
        }
    }

    public class LayoutState
    {
        private readonly State state;

        public ReadOnlyCollection<Transition> Transitions { get { return state.Transitions; } }

        public LayoutState(State state)
        {
            this.state = state;

        }
    }

    public class LayoutTransition
    {
        private readonly Transition transition;
        public LayoutState Source { get { return new LayoutState(transition.Source); } }
        public LayoutState Destination { get { return new LayoutState(transition.Destination); } }

        public bool IsBackLoop { get; set; }

        public LayoutTransition(Transition transition)
        {
            this.transition = transition;

        }

    }
}