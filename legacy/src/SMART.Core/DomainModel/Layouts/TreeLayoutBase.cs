using System;
using System.Collections.Generic;
using System.Linq;
using SMART.Core.Interfaces;

namespace SMART.Core.DomainModel.Layouts
{
    public abstract class TreeLayoutBase : LayoutBase
    {
        protected LayoutTree Tree { get; private set; }

        protected TreeLayoutBase(IModel model) : base(model)
        {
            Tree = LayoutTree.CreateFromModel(model);

        }

        protected void FixStopState()
        {
            if (Model.StopState.InTransitions.Count() == 0)
            {
                Model.StopState.Location = new SmartPoint(Model.StartState.Location.X + 300, Model.StartState.Location.Y);
            }
        }
    }

    public class RadialTreeLayout : TreeLayoutBase
    {
        private int change = 2;
        private int layer_spacing = 300;

        public RadialTreeLayout(IModel model) : base(model)
        {
        }

        public override void BeginLayout()
        {
            TreeLayout();
            FixStopState();
        }

        private void TreeLayout()
        {
            int max = 1;
            int tmp = 0;
            foreach (var state in Model.States)
            {
                tmp = state.GetLevel();
                if (tmp > max)
                    max = tmp;
            }

            List<State> states = new List<State>();
            SmartPoint root = null;
            for(int i = 1; i <= max; i++ )
            {
                int i1 = i;
                states = Model.States.FindAll(s => s.GetLevel() == i1);
                for (int j = 0; j < states.Count; j++ )
                {
                    if(i == 1)
                    {
                        root = new SmartPoint(800,800);
                        states[j].Location = root;
                        continue;
                    }

                    int radius = i*layer_spacing;
                    double alpha = (Math.PI * (360d/states.Count) * j) / 180d; 
                    states[j].Location.X = root.X + (radius*(Math.Cos(alpha)));
                    states[j].Location.Y = root.Y + (radius*(Math.Sin(alpha)));
                }
            }
        }
    }
}