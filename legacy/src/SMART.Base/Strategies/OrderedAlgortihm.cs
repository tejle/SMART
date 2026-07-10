using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;


namespace SMART.Base.Strategies
{

    [Export]
    [Export(typeof(IAlgorithm))]
    [Algorithm(Name = "Ordered")]
    public class OrderedAlgortihm : IAlgorithm
    {
        public event EventHandler<ModelElementVistedEventArgs> ModelElementVisted;
        public IModel Model {
            get; set;

        }

        public IExecutionEnvironment ExecutionEnvironment
        {
            get; set;
        }

        public bool MoveNext()
        {
            throw new System.NotImplementedException();
        }

        public IModelElement Current {
            get; private set;
        }

        public void Reset()
        {}

        public IEnumerable<IStep> GetEnumerator(Model model, State startState)
        {
            //var states = from v in model.States
            //               orderby v.Label
            //               select v;

            //foreach (var v in states)
            //{
            //    yield return model.CreateStep(v);
            //}

            //var transitions = from e in model.Transitions
            //            orderby e.Label
            //            select e;
            //foreach (var e in transitions)
            //{
            //    yield return model.CreateStep(e);

            //}
            yield return null;
        }

        public IEnumerable<IModelElement> Traverse(IModel model, IExecutionEnvironment environment)
        {
            throw new System.NotImplementedException();
        }
    }
}