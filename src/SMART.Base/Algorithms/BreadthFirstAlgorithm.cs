using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;


namespace SMART.Base.Algorithms
{
    [Algorithm(Description = "Breadth first generation of teststeps", Name = "Breadth first", Version = "1.0")]
    public class BreadthFirstAlgorithm : IAlgorithm
    {
        private Queue<IModelElement> queue;
        private Queue<IModelElement> path;
        public event EventHandler<ModelElementVistedEventArgs> ModelElementVisted;

        public IModel Model { get; set; }
        public IExecutionEnvironment ExecutionEnvironment { get; set; }

        private IModelElement current;
        public IModelElement Current
        {
            get { return current; }
            private set
            {
                if(current != null)
                    current.IsCurrent = false;
                current = value;
                if (current != null)
                    current.IsCurrent = true;
            }
        }

        public BreadthFirstAlgorithm()
        {
            queue = new Queue<IModelElement>();
            path = new Queue<IModelElement>();
        }

        public bool MoveNext()
        {
            if (Current == null)
            {
                if(queue.Count == 0)
                {
                    queue.Enqueue(Model.StartState);
                   
                }
            }
            else if(Current is State && Current != Model.StartState)
            {
                Current = null;
                path.Clear();
                return false;
            }
            
            Current = queue.Dequeue();
            InvokeModelElementVisted(Current);
            path.Enqueue(Current);
            if(Current is State)
            {
                var outTransitions = from t in ((State) Current).OutTransitions
                                      where t.VisitCount == 0
                                      select t;

                // Enqueue path to current
                foreach(var i in path)
                    queue.Enqueue(i);
                outTransitions.ToList().ForEach(t=>
                                                    {
                                                        queue.Enqueue(t);
                                                        queue.Enqueue(t.Destination);
                                                    });
            }
          
            return Current != null;
        }

        public void Reset()
        {
            Current = null;
            queue.Clear();
            path.Clear();
        }

        private void InvokeModelElementVisted(IModelElement element)
        {
            var e = new ModelElementVistedEventArgs(element);
            EventHandler<ModelElementVistedEventArgs> visted = ModelElementVisted;
            if (visted != null) visted(this, e);
        }
    }

}


//public IEnumerator<IStep> GetEnumerator()
//{
//    //yield return model.CreateStep(current);
//foreach(var e in model.GetExecutableOutTransitions(current))
//    notExaminedTransitions.Enqueue(e);
//while(notExaminedTransitions.Count > 0)
//{
//    if(StopCriteria.Complete) break;
//    var transition = notExaminedTransitions.Dequeue();
//    yield return model.CreateStep(transition);
//    yield return model.CreateStep(transition.Destination);
//    foreach(var e in model.GetExecutableOutTransitions(transition.Destination))
//    {
//        if(!notExaminedTransitions.Contains(e)) // to avoid unnecessaary loops?
//            notExaminedTransitions.Enqueue(e);
//    }
//}
//TODO: 
// Start over?
// Look at visit count to get out of bad loops?
// Sort list on visited count?   
//}

//public IEnumerable<IStep> GetEnumerator(Model model, State startState)
//      {
//var current = startState;
//foreach (var e in model.GetExecutableOutTransitions(current))
//    notExaminedTransitions.Enqueue(e);
//while (notExaminedTransitions.Count > 0)
//{
//    var transition = notExaminedTransitions.Dequeue();
//    yield return model.CreateStep(transition);
//    yield return model.CreateStep(transition.Destination);
//    foreach (var e in model.GetExecutableOutTransitions(transition.Destination))
//    {
//        if (!notExaminedTransitions.Contains(e)) // to avoid unnecessaary loops?
//            notExaminedTransitions.Enqueue(e);
//    }
//}
//    yield return null;
//}