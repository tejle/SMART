using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Services;
using System.Diagnostics;

namespace SMART.Core.Workflow
{
    public class TestExecutionEngine : IExecutionEngine
    {
        public event EventHandler<StepInfo> StepExecuted;

        public event EventHandler ExecutionDone;

        private readonly IEventService eventService;

        private IModelElement current;

        private bool IsCanceled { get; set; }

        private readonly List<Queue<IStep>> defectFlows  =new List<Queue<IStep>>();
        public IEnumerable<Queue<IStep>> DefectFlows
        {
            get
            {
                return new ReadOnlyCollection<Queue<IStep>>(defectFlows);
            }
            
        }

        public void Reset()
        {
            defectFlows.Clear();
            IsCanceled = false;
        }

        public TestExecutionEngine(IEventService eventService)
        {
            this.eventService = eventService;
        }

        void adapter_DefectDetected(object sender, DefectEventArgs e)
        {
            eventService.GetEvent<TestExecutionDefectDetected>().Publish(e);
        }

        private int totalSteps = 0;
        private int steps = 0;
        public TimeSpan Execute(List<Queue<IStep>> list, ITestcase testcase, IModel model, bool isReExecution)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Queue<IStep> tmpSequence;
            if (!isReExecution)
            {
                steps = 0;
                InitCriterias(testcase.ExecutionStopCriteriasas);
            }
            totalSteps = steps + list.Sum(s => s.Count);
          
            using (var adapter = testcase.Adapters.First())
            {
                adapter.DefectDetected += adapter_DefectDetected;
                foreach (var sequence in list)
                {
                    tmpSequence = new Queue<IStep>();
                    if (IsCanceled)
                    {
                        break;
                    }
                    while (sequence.Count > 0)
                    {

                        var step = sequence.Dequeue();
                        tmpSequence.Enqueue(step);
                        if (ShouldStop(testcase.ExecutionStopCriteriasas, model))
                            break;
                        if (current != null)
                            current.IsCurrent = false;

                        current = model.Elements.First(e => e.Id == step.ModelElement.Id);
                        current.IsCurrent = true;

                        bool success = adapter.Execute(step.Function, step.Parameters);
                        if (success)
                        {
                            current.Visit();
                        }
                        if (!success)
                        {
                            defectFlows.Add(tmpSequence);
                            current.IsDefect = true;
                        }
                        this.InvokeStepExecuted(totalSteps);
                        if (!success) break;
                    }
                }
                adapter.DefectDetected -= adapter_DefectDetected;
            }

            sw.Stop();
            return sw.Elapsed;
        }

        public void Stop()
        {
            IsCanceled = true;
        }

        public TimeSpan Execute(List<Queue<IStep>> list, ITestcase testcase, IModel model)
        {
            return Execute(list, testcase, model, false);
        }

        private void InvokeStepExecuted(int totalSteps)
        {
            steps++;
            EventHandler<StepInfo> tmp = this.StepExecuted;
            if (tmp != null)
            {
                tmp(this, new StepInfo(totalSteps));
            }
        }

        private static void InitCriterias(IEnumerable<IExecutionStopCriteria> executionStopCriterias)
        {
            foreach (var criteria in executionStopCriterias)
            {
                criteria.Init();
            }
        }

        private bool ShouldStop(IEnumerable<IExecutionStopCriteria> executionStopCriterias, IModel model)
        {
            bool stop = true;
            foreach (var s in executionStopCriterias)
                stop &= s.ShouldStop(model);

            return IsCanceled || stop;
        }
    }

    public interface IExecutionEngine
    {
        TimeSpan Execute(List<Queue<IStep>> sequence, ITestcase testcase, IModel model);
        TimeSpan Execute(List<Queue<IStep>> sequence, ITestcase testcase, IModel model, bool isReExecution);
        event EventHandler<StepInfo> StepExecuted;
        void Stop();
        IEnumerable<Queue<IStep>> DefectFlows { get; }
        void Reset();
    }

    public class ExecutingModel : ModelDecorator, IExecutingModel
    {
        public ExecutingModel(IModel model)
            : base(model.Clone())
        {
            ResetModelCounters();
        }


        private void ResetModelCounters()
        {
            foreach (var element in Model.Elements)
            {
                element.Reset();
            }
        }
    }

    public interface IExecutingModel
    {
        IEnumerable<IModelElement> Elements { get; }
    }
}