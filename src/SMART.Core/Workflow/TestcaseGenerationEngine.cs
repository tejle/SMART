using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Factories;
using SMART.Core.Interfaces.Repository;
using SMART.Core.Interfaces.Services;
using SMART.IOC;
using SMART.Core.BusinessLayer.Threading;

namespace SMART.Core.Workflow
{
    public class TestGenerationEngine : IGenerationEngine
    {
        private readonly IEventService eventService;
        private readonly IModelCompiler compiler;
        public TestGenerationEngine(IEventService eventService, IModelCompiler compiler)
        {
            this.eventService = eventService;
            this.compiler = compiler;
        }


        public List<Queue<IStep>> Generate(ITestcase testcase, IModel model, IExecutionEnvironment sandbox)
        {
            var elements = new List<IModelElement>();
            var listseq = new List<Queue<IStep>>();
            var seq = new Queue<IStep>();

            var algorithm = testcase.Algorithms.First();

            algorithm.Reset();
            algorithm.ModelElementVisted += algorithm_ModelElementVisted;

            algorithm.Model = model;
            algorithm.ExecutionEnvironment = sandbox;

            while (!ShouldStop(sandbox, testcase, model))
            {
                if (algorithm.MoveNext())
                    elements.Add(algorithm.Current);
                else
                {
                    if (elements.Count == 0) break;
                    algorithm.Reset();
                    seq = new Queue<IStep>();
                    foreach (var element in elements)
                    {
                        seq.Enqueue(compiler.CreateStep(element));
                    }
                    listseq.Add(seq);
                    elements.Clear();
                }
            }

            algorithm.ModelElementVisted -= algorithm_ModelElementVisted;
            if (elements.Count > 0)
            {
                seq = new Queue<IStep>();
                foreach (var element in elements)
                {
                    seq.Enqueue(compiler.CreateStep(element));
                }
                listseq.Add(seq);
            }



            return listseq;
        }

        public void Reset(IModel model)
        {
            var elem = (from e in model.Elements select e).ToList();
            foreach (var e in elem)
            {
                e.Reset();
            }

        }

        private static bool ShouldStop(IExecutionEnvironment executionEnvironment, ITestcase testcase, IModel model)
        {
            return executionEnvironment
                .CheckCriteria(model, testcase.GenerationStopCriterias);
        }

        private void algorithm_ModelElementVisted(object sender, ModelElementVistedEventArgs e)
        {
            eventService.GetEvent<TestGenerationEvent>().Publish(e.Element);

        }
    }

    public interface IGenerationEngine
    {
        List<Queue<IStep>> Generate(ITestcase testcase, IModel model, IExecutionEnvironment sandbox);
        void Reset(IModel model);
    }
}