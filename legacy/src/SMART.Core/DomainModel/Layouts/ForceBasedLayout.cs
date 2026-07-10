using System;
using System.Linq;
using SMART.Core.Interfaces;

namespace SMART.Core.DomainModel.Layouts
{
    public class ForceBasedLayout :LayoutBase
    {
        public new event EventHandler LayoutComplete;

        private readonly IModel model;

        private const double Threshold = 0.5;

        private double timeStep = 2;

        private double damping = 0.5d;

        public ForceBasedLayout(IModel model) : base(model)
        {
            this.model = model;
        }

        public override void BeginLayout()
        {
            RunSimulation();
            InvokeLayoutComplete(EventArgs.Empty);
           
        }

        private void RunSimulation()
        {
            // init all states with 0 velocity
            model.States.ForEach(initVelocity);
            // init state positions random, but not on top of each other
            initStatePositions();
            initStateMass();
            double total_kinetic_energy = 0;
            do
            {
                foreach (var state in model.States)
                {
                    var net_force = new Vector2D(0, 0);
                    foreach (var otherState in model.States.Except(new[] {state}))
                    {
                        net_force += Coulomb_repulsion(state, otherState);
                    }

                    foreach (var transition in state.Transitions)
                    {
                        net_force += Hooke_attraction(state, transition);
                    }

                    dampenStateVelocity(state, net_force);
                    changeStatePosition(state);
                    var velocity = (Vector2D) state.Tags[Tags.Velocity];
                    total_kinetic_energy += ((double) state.Tags[Tags.Mass]) *CalcHypothesis(velocity);
                }
            } while(total_kinetic_energy > Threshold);

        }

        private void initStateMass()
        {
            model.States.ForEach(s=> s.Tags[Tags.Mass] = 3.0d);
        }

        private double CalcHypothesis(Vector2D velocity)
        {
            return (Math.Sqrt(
                Math.Pow(
                    (velocity)
                        .X, 2)
                +
                Math.Pow(
                    (velocity)
                        .Y, 2)));
        }

        private void changeStatePosition(State state)
        {
            //     this_node.position := this_node.position + timestep * this_node.velocity
            var v = (Vector2D)state.Tags[Tags.Velocity];
            var tmp = v * timeStep;
            state.Location.X += tmp.X;
            state.Location.Y += tmp.Y;

        }

        private void dampenStateVelocity(State state, Vector2D net_force)
        {
            //this_node.velocity := (this_node.velocity + timestep * net-force) * damping
            var v = (Vector2D)state.Tags[Tags.Velocity];
            v += ((net_force * timeStep) * damping);
            state.Tags[Tags.Velocity] = v;
        }

        private Vector2D Hooke_attraction(State state, Transition transition)
        {
            return state.Tags[Tags.Velocity] as Vector2D;
        }

        private Vector2D Coulomb_repulsion(State state, State otherState)
        {
            Vector2D tmp = new Vector2D(
                Math.Abs(state.Location.X - otherState.Location.X), 
                Math.Abs(state.Location.Y - otherState.Location.X));
            var radius = CalcHypothesis(tmp);
            ////double k = 9*Math.Pow(10, 9);
            Vector2D v = new Vector2D(1,1);
            Vector2D v1 = (Vector2D)state.Tags[Tags.Velocity] +v;
            Vector2D v2 = (Vector2D)otherState.Tags[Tags.Velocity] +v;
            Vector2D force = (v1*v2)/radius;
            return force;
        }

        private void initStatePositions()
        {
            int x = 0;
            int y = 0;
            foreach (var state in model.States)
            {
                if (x > 400)
                {
                    x = 0;
                    y += 30;
                }
                state.Location.X = x;
                state.Location.Y = y;
                x += 30;
            }
        }

        private void initVelocity(State s)
        {
            if (s.Tags.ContainsKey(Tags.Velocity))
                s.Tags[Tags.Velocity] = new Vector2D(0, 0);
            else
                s.Tags.Add(Tags.Velocity, new Vector2D(0, 0));
        }

        private void InvokeLayoutComplete(EventArgs e)
        {
            LayoutComplete?.Invoke(this, e);
        }
    }

    public static class Tags
    {
        public const string Walked = "Walked";
        public const string Shift = "Shift";
        public const string Change = "Change";
        public const string Ancestor = "Ancestor";
        public const string Thread = "Thread";
        public const string Mod = "Mod";
        public const string IsLeaf = "IsLeaf";
        public const string X = "XCoord";
        public const string Mass = "Mass";
        public const string Velocity = "Velocity";
        public const string Level = "LevelOfOrder";

    }

    public class Vector2D
    {
        public double X;
        public double Y;

        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }
        public static Vector2D operator +(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2D operator -(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2D operator *(Vector2D v1, double magnitude)
        {
            return new Vector2D(v1.X * magnitude, v1.Y * magnitude);
        }

        public static Vector2D operator *(Vector2D v1, Vector2D v2)
        {
            return new Vector2D(v1.X * v2.X, v1.Y * v2.Y);
        }

        public static Vector2D operator /(Vector2D v1, double magnitude)
        {
            if(magnitude == 0) throw new ArithmeticException("magnitude can not be null");
            return new Vector2D(v1.X / magnitude, v1.Y / magnitude);
        }
    }

}