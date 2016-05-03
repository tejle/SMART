using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;


namespace SMART.Base.Algorithms
{
 
    [Algorithm(Description = "Assisted random generation of teststeps", Name = "AssistedRandomAlgorithm", Version = "1.0")]
    public class AssistedRandomAlgorithm : IAlgorithm
    {
        private readonly Random random;
        private int lookAheadThreshold = 4;
        private int lookAheadDeepth = 4;

        [ImportingConstructor]
        public AssistedRandomAlgorithm([Import]Random random)
        {
            this.random = random;
        }

        public event EventHandler<ModelElementVistedEventArgs> ModelElementVisted;
        public IModel Model{get ; set;}
        public IExecutionEnvironment ExecutionEnvironment{get; set;}

        public IModelElement Current { get; private set; }

        public bool MoveNext()
        {
            if (Current == null)
                Current = Model.StartState;

            IEnumerable<Transition> outTrans;
            if (Current is State) {
                
                outTrans = ExecutionEnvironment.GetOutTransitions((State)Current);
                if (outTrans.Count() == 0) return false;
                
                int min = outTrans.Min(e => e.VisitCount);
                if(min > lookAheadThreshold)
                {
                    bool shouldContinue = LookAhead(Current);
                    if(!shouldContinue) return false;
                }
                outTrans = outTrans.Where(t => t.VisitCount == min);
               
                Current.IsCurrent = false;
                Current = outTrans.ElementAt(random.Next(outTrans.Count() - 1));
                Current.IsCurrent = true;
                SendModelElementVisited();
            } else {
                Current.IsCurrent = false;
                Current = ((Transition)Current).Destination;
                Current.IsCurrent = true;
                if (Current == Model.StopState) return false;
                SendModelElementVisited();
            }

            return true;
        }

        private bool LookAhead(IModelElement element)
        {
            var state = element as State;
            List<State> states = new List<State>();
            foreach (var transition in state.OutTransitions)
            {
               if(!states.Contains(transition.Destination))
                   states.Add(transition.Destination);
            }

            List<State> tmp;
            for (int i = 0; i < lookAheadDeepth; i++)
            {
                tmp = new List<State>();
                foreach (var st in states)
                {
                    foreach (var t in st.OutTransitions)
                    {
                        if (!tmp.Contains(t.Destination))
                        {
                            tmp.Add(t.Destination);
                        }
                    }
                }
                foreach (var st in tmp)
                {
                    if(states.Contains(st)) continue;
                    states.Add(st);
                }
            }
            bool goOn = false;
            states.ForEach(s=>
                               {
                                   var ts = s.OutTransitions.Where(t => t.VisitCount < lookAheadThreshold);
                                   if(ts.Count() > 0) goOn = true;
                               });
            return goOn;
        }

        private void SendModelElementVisited() {
            var tmp = ModelElementVisted;
            if (tmp != null)
                tmp(this, new ModelElementVistedEventArgs(Current));
        }

        public void Reset()
        {
            Current = null;
        }

        public void Dispose()
        {
               
        }
    }
}
