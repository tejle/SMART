using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SMART.Core.Interfaces;

namespace SMART.Core.DomainModel.Layouts
{
    public class OrthogonalLayout : LayoutBase
    {
        private readonly IModel model;
        //public new event EventHandler LayoutComplete;
        List<State> vistedStates = new List<State>();
        List<State> allStates = new List<State>();
        List<State> doneStates = new List<State>();

        public int v_space = 150;
        public int h_space = 250;
        public int s_width = 175;
        public int s_height = 50;

        public int number_s;
        

        public OrthogonalLayout(IModel model) : base (model)
        {
            this.model = model;
            allStates = model.States;
            number_s = model.States.Count;
        }

        public override void BeginLayout()
        {

            State current = model.StartState;

             current.Location = new SmartPoint(h_space * number_s, 100);

            while (allStates.Count > 0)
            {
                if(allStates.Count == 1 && allStates.OfType<StopState>() != null) return;
                var out_t = current.OutTransitions;
                var states = GetStates(out_t);
                SmartPoint last_point = current.Location;
                last_point = GetNextPoint(last_point, states.Count);

                for (int i = 0; i < states.Count; i++)
                {
                    if (vistedStates.Contains(states[i]) || doneStates.Contains(states[i])) continue;

                    states[i].Location = new SmartPoint(last_point.X, last_point.Y);
                    vistedStates.Add(states[i]);
                    last_point = new SmartPoint(last_point.X + h_space, last_point.Y);
                }
                
                doneStates.Add(current);
                allStates.Remove(current);
                
                if(vistedStates.Count == 0)break;
                
                current = vistedStates.ElementAt(0); 
                vistedStates.RemoveAt(0);
            }

            // Free floating StopState? Then put it next to the StartState
            var stopState = model.StopState;
            if (stopState.InTransitions.Count == 0)
            {
                stopState.Location = new SmartPoint(model.StartState.Location.X + model.StartState.Size.Width + h_space,
                                                    model.StartState.Location.Y);
            }
        }

        private SmartPoint GetNextPoint(SmartPoint last_point, int numberofStates)
        {
            double x = 0;
            switch (numberofStates)
            {
                case 1:
                    x = 0;
                    break;
                case 2:
                    x = h_space/2;
                    break;
                case 3:
                    x = h_space;
                    break;
                case 4:
                    x = h_space*(1.5);
                    break;
                default:
                    x = h_space*((numberofStates-1)/2);
                    break;
            }
            last_point = new SmartPoint(last_point.X - x, last_point.Y + v_space);
            return last_point;
        }

        private List<State> GetStates(ReadOnlyCollection<Transition> out_t)
        {
            List<State> states = new List<State>();
            foreach (var t in out_t)
            {
                if(vistedStates.Contains(t.Destination) || doneStates.Contains(t.Destination))continue;
                states.Add(t.Destination);
            }
            return states;
        }
    }
}
