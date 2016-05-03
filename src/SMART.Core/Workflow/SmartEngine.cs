using System;
using System.Collections.Generic;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using System.ComponentModel;
using System.Collections.Specialized;
using SMART.Core.Interfaces.Reporting;
using SMART.Core.Workflow.Reporting;
using SMART.IOC;
using Microsoft.Practices.Unity;

namespace SMART.Core.Workflow
{
    /// <summary>
    /// http://msdn.microsoft.com/en-us/library/hkasytyf.aspx
    /// </summary>
    public class SmartEngine : Component
    {
        private Guid executeId;

        private delegate void WorkerEventHandler(UnitOfWork unitOfWork, AsyncOperation operation);

        public event EventHandler<StepInfo> StepExecuted;

        public event EventHandler<CompileCompletedEventArgs> CompileCompleted;
        public event EventHandler<GenerateCompletedEventArgs> GenerateCompleted;
        public event EventHandler<ExecuteCompletedEventArgs> ExecuteCompleted;

        private readonly HybridDictionary userStateToLifeTime = new HybridDictionary();

        private Container components;
        private IModelCompiler compiler;
        private IExecutionEnvironment sandbox;

        private IStatisticsService statisticsService;
        private IGenerationEngine generationEngine;
        private IExecutionEngine executionEngine;

        private IReport report;

        public SmartEngine(IContainer container)
        {
            container.Add(this);
            InitializeComponent();
            InitializeEngine();

        }

        [InjectionConstructor]
        public SmartEngine()
        {
            InitializeComponent();
            InitializeEngine();
        }

        private void InitializeEngine()
        {
            compiler = Resolver.Resolve<IModelCompiler>();
            executionEngine = Resolver.Resolve<IExecutionEngine>();
            generationEngine = Resolver.Resolve<IGenerationEngine>();
            statisticsService = Resolver.Resolve<IStatisticsService>();
        }

        public void CompileAsync(UnitOfWork unitOfWork, object taskId)
        {
            CreateAndInvokeOperation(taskId, unitOfWork, Compile);
        }

        public void GenerateAsync(UnitOfWork unitOfWork, object taskId)
        {
            var list = new List<IStatistic>();
            foreach (var criteria in unitOfWork.Testcase.GenerationStopCriterias)
            {
                criteria.Statistic.Reset(unitOfWork.Model);
                list.Add(criteria.Statistic);
            }

            statisticsService.Statistics = list;
            CreateAndInvokeOperation(taskId, unitOfWork, Generate);
        }

        public void ExecuteAsync(UnitOfWork unitOfWork)
        {
            if(IsExecuting()) return;
            
            executeId = Guid.NewGuid();
            
            statisticsService.Statistics = new List<IStatistic>();
            CreateAndInvokeOperation(executeId, unitOfWork, Execute);
        }

        private bool IsExecuting()
        {
            bool executing = false;
            lock (userStateToLifeTime.SyncRoot)
            {
                if (userStateToLifeTime.Contains(executeId))
                {
                    executing = true;
                }
                
            }
            return executing;
        }

        private void CreateAndInvokeOperation(object taskId, UnitOfWork unitOfWork, WorkerEventHandler workDelegate)
        {
            AsyncOperation operation = AsyncOperationManager.CreateOperation(taskId);

            lock (userStateToLifeTime.SyncRoot)
            {
                if (userStateToLifeTime.Contains(taskId))
                {
                    throw new ArgumentException("Task Id parameter must be unique", "taskId");
                }

                userStateToLifeTime[taskId] = operation;
            }

            workDelegate.BeginInvoke(unitOfWork, operation, null, null);
        }

        private void Compile(UnitOfWork unitOfWork, AsyncOperation operation)
        {
            DoTask(unitOfWork, operation, DoCompile, CompileCompletionMethod);
        }

        private void Generate(UnitOfWork unitOfWork, AsyncOperation operation)
        {
            DoTask(unitOfWork, operation, DoGenerate, GenerateCompletionMethod);
        }

        private void Execute(UnitOfWork unitOfWork, AsyncOperation operation)
        {
            DoTask(unitOfWork, operation, DoExecute, ExecuteCompletionMethod);
        }

        private void DoTask(UnitOfWork unitOfWork, AsyncOperation operation, Func<UnitOfWork, AsyncOperation, UnitOfWork> doMethod, Action<UnitOfWork, Exception, bool, AsyncOperation> completionMethod)
        {
            Exception e = null;
            UnitOfWork work = null;

            if (!TaskCanceled(operation))
            {
                try
                {
                    work = doMethod(unitOfWork, operation);
                }
                catch (Exception ex)
                {
                    e = ex;
                }
            }
            completionMethod(work, e, TaskCanceled(operation), operation);
        }

        private UnitOfWork DoCompile(UnitOfWork work, AsyncOperation operation)
        {
            var model = compiler.Compile(work.Testcase.Models);
            return new UnitOfWork(work.Project, work.Testcase, model, null);
        }

        private UnitOfWork DoGenerate(UnitOfWork work, AsyncOperation operation)
        {
            if(sandbox == null)
                sandbox = compiler.CreateSandbox(work.Model);

            var listseq = generationEngine.Generate(work.Testcase, work.Model, sandbox);
            generationEngine.Reset(work.Model);
            return new UnitOfWork(work.Project, work.Testcase, work.Model, listseq);
        }

        private UnitOfWork DoExecute(UnitOfWork work, AsyncOperation operation)
        {
            
            executionEngine.StepExecuted += executionEngine_StepExecuted;
            executionEngine.Reset();

            TimeSpan elapsed = executionEngine.Execute(work.Steps, work.Testcase, work.Model);

            bool stop = true;
            var visitCounts = new Dictionary<IModelElement, int>();
            
            foreach(var elem in work.Model.Elements)
                visitCounts.Add(elem, 0);

            int count = 0;
            do
            {
                foreach (var s in work.Testcase.GenerationStopCriterias)
                    stop &= s.ShouldStop(work.Model);

                foreach (var s in work.Testcase.ExecutionStopCriteriasas)
                    stop &= s.ShouldStop(work.Model);

                if (!stop)
                {
                    // save visit count before generation

                    foreach(var element in work.Model.Elements)
                    {
                        visitCounts[element] = element.VisitCount;
                    }

                    foreach (var s in work.Testcase.GenerationStopCriterias)
                        s.Statistic.Reset(work.Model);

                    var seq = generationEngine.Generate(work.Testcase, work.Model, sandbox);

                    // reset visit count to values be for generation
                    foreach (var elem in work.Model.Elements)
                    {
                        elem.VisitCount = visitCounts[elem];
                        elem.IsCurrent = false;
                    }

                    work = new UnitOfWork(work.Project, work.Testcase, work.Model, seq);
                    elapsed += executionEngine.Execute(work.Steps, work.Testcase, work.Model, true);

                    count++;
                }
            } while (!stop && count < 5); 
            executionEngine.StepExecuted -= executionEngine_StepExecuted;
           
            
            work.DefectFlows = executionEngine.DefectFlows;
            var reportService = new ReportService();
            report = reportService.GenerateReport(work);
            report.TotalElapsedTime = elapsed;
            
            work.Project.AddReport(report);

            return work;
        }

        void executionEngine_StepExecuted(object sender, StepInfo e)
        {
            InvokeStepExecuted(e.TotalSteps);
        }

        private void CompileCompletionMethod(UnitOfWork work, Exception exception, bool canceled, AsyncOperation operation)
        {
            if (!canceled)
            {
                RemoveOperation(operation);
            }

            var e = new CompileCompletedEventArgs(work, exception, canceled, operation.UserSuppliedState);
            operation.PostOperationCompleted(CompileCompletedDelegate, e);
        }

        private void GenerateCompletionMethod(UnitOfWork work, Exception exception, bool canceled, AsyncOperation operation)
        {
            if (!canceled)
            {
                RemoveOperation(operation);
            }

            var e = new GenerateCompletedEventArgs(work, exception, canceled, operation.UserSuppliedState);
            operation.PostOperationCompleted(GenerateCompletedDelegate, e);
        }

        private void ExecuteCompletionMethod(UnitOfWork work, Exception exception, bool canceled, AsyncOperation operation)
        {
            if (!canceled)
            {
                RemoveOperation(operation);
            }

            var e = new ExecuteCompletedEventArgs(work, exception, report, canceled, operation.UserSuppliedState);
            operation.PostOperationCompleted(ExecuteCompletedDelegate, e);
        }

        private void CompileCompletedDelegate(object state)
        {
            var e = state as CompileCompletedEventArgs;
            InvokeCompileCompleted(e);
        }

        private void GenerateCompletedDelegate(object state)
        {
            var e = state as GenerateCompletedEventArgs;
            InvokeGenerateCompleted(e);
        }

        private void ExecuteCompletedDelegate(object state)
        {
            var e = state as ExecuteCompletedEventArgs;
            InvokeExecuteCompleted(e);
        }


        private void RemoveOperation(AsyncOperation operation)
        {
            lock (userStateToLifeTime.SyncRoot)
            {
                userStateToLifeTime.Remove(operation.UserSuppliedState);
            }
        }

        public static UnitOfWork CreateUnitOfWork(IProject project, ITestcase testcase)
        {
            return new UnitOfWork(project, testcase, null, null);
        }

        public void StopExecution()
        {
            if(!userStateToLifeTime.Contains(executeId)) return;

            RemoveOperation(userStateToLifeTime[executeId] as AsyncOperation);
            executionEngine.Stop();
        }

        public void PauseExecution()
        {
            StopExecution();
        }

        public void CancelAsync(object taskId)
        {
            var operation = userStateToLifeTime[taskId] as AsyncOperation;

            if (operation != null)
            {
                lock (userStateToLifeTime.SyncRoot)
                {
                    userStateToLifeTime.Remove(taskId);
                }
            }
        }

        private bool TaskCanceled(AsyncOperation operation)
        {
            if (operation == null) return false;
            var canceled = false;
            if (operation.UserSuppliedState != null)
                canceled = userStateToLifeTime[operation.UserSuppliedState] == null;
            return canceled;
        }

        #region event invokers

        private void InvokeGenerateCompleted(GenerateCompletedEventArgs e)
        {
            var generateCompleteHandler = GenerateCompleted;
            if (generateCompleteHandler != null)
                generateCompleteHandler(this, e);
        }

        private void InvokeCompileCompleted(CompileCompletedEventArgs e)
        {
            var compileCompleteHandler = CompileCompleted;
            if (compileCompleteHandler != null)
                compileCompleteHandler(this, e);
        }

        private void InvokeExecuteCompleted(ExecuteCompletedEventArgs e)
        {
            var executeCompleteHandler = ExecuteCompleted;
            if (executeCompleteHandler != null)
                executeCompleteHandler(this, e);
        }

        private void InvokeStepExecuted(int totalSteps)
        {
            EventHandler<StepInfo> stepExecutedHandler = StepExecuted;
            if (stepExecutedHandler != null) stepExecutedHandler(this, new StepInfo(totalSteps));
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new Container();

        }
    }

    public class StepInfo : EventArgs
    {
        public int TotalSteps { get; private set; }
        public StepInfo(int totalSteps)
        {
            TotalSteps = totalSteps;
        }
    }
}