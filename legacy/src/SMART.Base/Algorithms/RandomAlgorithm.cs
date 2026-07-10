using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;

using System;
using SMART.IOC;
using SMART.Core.DomainModel;

namespace SMART.Base.Algorithms
{
    public class randomHelperClass
    {
        [Export]
        public Random rand = Resolver.Resolve<Random>();
    }
    
    [Export]
    [Export(typeof(IAlgorithm))]
    [Algorithm(Description = "Random generation of teststeps", Name = "Random", Version = "1.0")]
    public class RandomAlgorithm : IAlgorithm, IEnumerator<IModelElement>
    {
        private IModelElement current;

        private readonly Random random;

        [ImportingConstructor]
        public RandomAlgorithm([Import]Random random)
        {
            this.random = random;
        }

        public event EventHandler<ModelElementVistedEventArgs> ModelElementVisted;

        public IModelElement Current
        {
            get { return current; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public IModel Model { get; set; }

        public IExecutionEnvironment ExecutionEnvironment { get; set; }

        public bool MoveNext()
        {
            if (current == null)
                current = Model.StartState;

            IEnumerable<Transition> outTrans;
            if (current is State)
            {
                outTrans = ExecutionEnvironment.GetOutTransitions((State) current);
                if (outTrans.Count() == 0) return false;

                current.IsCurrent = false;
                current = outTrans.ElementAt(random.Next(outTrans.Count()- 1));
                current.IsCurrent = true;
                SendModelElementVisited();
            }
            else
            {
                current.IsCurrent = false;
                current = ((Transition) current).Destination;
                current.IsCurrent = true;
                if(current == Model.StopState) return false;
                SendModelElementVisited();
            }
            
            return true;
        }

        public void Dispose()
        {
            

        }

        private void SendModelElementVisited()
        {
            var tmp = ModelElementVisted;
            if(tmp != null)
                tmp(this, new ModelElementVistedEventArgs(current));
        }

        public void Reset()
        {
            current = null;
        }

        public IEnumerable<IStep> GetEnumerator(Model model, State startState)
        {
            //var currentState = startState;
            //var outTransitions = ExecutableOutTransitions(model, currentState);
            //while (outTransitions.Count()>0)
            //{
            //    var transition = outTransitions.ElementAt(random.Next(outTransitions.Count() - 1));
            //    yield return model.CreateStep(transition);

            //    currentState = transition.Destination;
            //    yield return model.CreateStep(currentState);

            //    outTransitions = ExecutableOutTransitions(model, currentState);
            //}

            yield return null;
        }

        public IEnumerable<IModelElement> Traverse(IModel model, IExecutionEnvironment environment) {
            State currentState = model.StartState;
            IEnumerable<Transition> outTransitions = environment.GetOutTransitions(currentState);
            while (outTransitions.Count() > 0) {
                var transition = outTransitions.ElementAt(random.Next(outTransitions.Count() - 1));
                yield return transition;

                currentState = transition.Destination;
                yield return currentState;

                outTransitions = environment.GetOutTransitions(currentState);
            }
        }

        protected static IEnumerable<Transition> ExecutableOutTransitions(Model model, State currentState)
        {
            return null;// model.GetExecutableOutTransitions(currentState);
        }
    }
}
