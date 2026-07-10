using SMART.Core.Interfaces.Reporting;
using SMART.Gui.View;

namespace SMART.Gui.ViewModel.TestcaseExecution
{
    using System;
    using System.Windows;

    using Commands;

    using Core.DomainModel;
    using Core.Interfaces;
    using Core.Interfaces.Services;
    using IOC;
    using System.Threading;
    using Core.Workflow;

    public class TestcaseExecutionCompositeViewModel : ViewModelBase
    {
        private UnitOfWork workItem;
        private IProject project;
        private ITestcase testcase;
        private readonly IEventService eventService;

        private Guid currentExecutionTaskId;

        private bool isExecuting;

        public bool IsExecuting
        {
            get { return isExecuting; }
            set
            {
                isExecuting = value;
                SendPropertyChanged("ShowRunButton");
                SendPropertyChanged("ShowPauseButton");
                SendPropertyChanged("ShowStopButton");
            }
        }

        private DateTime executionStartTime;
        private Timer executionTimer;
        private TimeSpan elapsedTime;
        private string status;
        private TestcaseCommonCommandsViewModel commonCommands;
        private bool generateAndExecute;
        private int totalStepCount;
        private int stepCounter;
        private ExecutionModelViewModel executionModel;

        readonly SmartEngine smartEngine;

        public TestcaseCommonCommandsViewModel CommonCommands
        {
            get { return commonCommands; }
            set { commonCommands = value; this.SendPropertyChanged("CommonCommands"); }
        }

        public string ElapsedTime
        {
            get { return string.Format("{0}h {1}m {2}s {3}ms", elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds, elapsedTime.Milliseconds); }
            //get { return string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", elapsedTime.Days, elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds); }
        }

        public bool ShowRunButton { get { return !isExecuting; } }
        public bool ShowPauseButton { get { return isExecuting; } }
        public bool ShowStopButton { get { return isExecuting; } }

        public string Status
        {
            get { return status; }
            set { status = value; this.SendPropertyChanged("Status"); }
        }

        public RoutedActionCommand ExecuteCommand { get; set; }
        public RoutedActionCommand PauseCommand { get; set; }
        public RoutedActionCommand StopCommand { get; set; }

        public InvokeOC<ExecutionLogItemViewModel> Output { get; set; }

        public ExecutionModelViewModel CurrentModel { get; set; }

        public bool GenerateAndExecute
        {
            get { return generateAndExecute; }
            set { generateAndExecute = value; SendPropertyChanged("GenerateAndExecute"); }
        }

        public int TotalStepCount
        {
            get { return totalStepCount; }
            set { totalStepCount = value; SendPropertyChanged("TotalStepCount"); }
        }

        public int StepCounter
        {
            get { return stepCounter; }
            set { stepCounter = value; SendPropertyChanged("StepCounter"); }
        }

        public override string Icon
        {
            get { return Constants.TESTCASE_EXECUTION_ICON; }
        }

        public override string Name { get { return testcase.Name; } }

        public override Guid Id
        {
            get { return testcase.Id; }
            set { testcase.Id = value; }
        }

        public bool CanEdit { get { return false; } }

        public ExecutionModelViewModel ExecutionModel
        {
            get { return executionModel; }
            set
            {
                executionModel = value;
                CurrentModel = value;
                SendPropertyChanged("ExecutionModel");
                SendPropertyChanged("CurrentModel");
            }
        }

        public TestcaseExecutionCompositeViewModel(IProject project, ITestcase testcase)
            : this(project, testcase, Resolver.Resolve<IEventService>())
        {
        }

        public TestcaseExecutionCompositeViewModel(IProject project, ITestcase testcase, IEventService eventService)
            : base(testcase.Name)
        {
            this.project = project;
            this.testcase = testcase;

            this.eventService = eventService;
            Output = new InvokeOC<ExecutionLogItemViewModel>(Application.Current.Dispatcher);
            executionTimer = new Timer(OnExecutionTimerTick);
            executionTimer.Change(Timeout.Infinite, 0);

            GenerateAndExecute = true;
            this.eventService.GetEvent<TestExecutionDefectDetected>().Subscribe(DefectDetected);

            CommonCommands = new TestcaseCommonCommandsViewModel(this, this.testcase)
                                 {
                                     //ExecuteButtonChecked = true
                                 };

            ExecuteCommand = new RoutedActionCommand("ExecuteCommand", typeof(TestcaseExecutionCompositeViewModel))
            {
                Description = "Execute",
                OnCanExecute = o => !isExecuting,
                OnExecute = this.OnStart
            };

            PauseCommand = new RoutedActionCommand("PauseCommand", typeof(TestcaseExecutionCompositeViewModel))
            {
                Description = "Pause",
                OnCanExecute = o => isExecuting,
                OnExecute = this.OnPause
            };

            StopCommand = new RoutedActionCommand("StopCommand", typeof(TestcaseExecutionCompositeViewModel))
            {
                Description = "Stop",
                OnCanExecute = o => isExecuting,
                OnExecute = this.OnStop
            };

            smartEngine = Resolver.Resolve<SmartEngine>();
            smartEngine.CompileCompleted += smartEngine_CompileCompleted;
            smartEngine.GenerateCompleted += smartEngine_GenerateCompleted;
            smartEngine.ExecuteCompleted += smartEngine_ExecuteCompleted;

        }

        public void Init(IProject project, ITestcase testcase)
        {
            this.project = project;
            this.testcase = testcase;
            Output = new InvokeOC<ExecutionLogItemViewModel>(Application.Current.Dispatcher);
            executionTimer = new Timer(OnExecutionTimerTick);
            executionTimer.Change(Timeout.Infinite, 0);
            stepCounter = 0;
            GenerateAndExecute = true;
            
        }

        private void OnStop(object obj)
        {
            smartEngine.StopExecution(); //CancelAsync(currentExecutionTaskId);
        }

        private void OnPause(object obj)
        {
            smartEngine.PauseExecution();
        }

        ~TestcaseExecutionCompositeViewModel()
        {
            smartEngine.CompileCompleted -= smartEngine_CompileCompleted;
            smartEngine.GenerateCompleted -= smartEngine_GenerateCompleted;
            smartEngine.ExecuteCompleted -= smartEngine_ExecuteCompleted;

        }

        void smartEngine_ExecuteCompleted(object sender, ExecuteCompletedEventArgs e)
        {
            executionTimer.Change(Timeout.Infinite, 0);
            smartEngine.StepExecuted -= ExecutionEngine_StepExecuted;
            IsExecuting = false;
            TotalStepCount = StepCounter; SendPropertyChanged("StepCounter");

            Status = string.Format("Step {0} of {1} ({2:P0})", StepCounter, TotalStepCount, (decimal)stepCounter / totalStepCount);
     
            Output.Add(new ExecutionLogItemViewModel(string.Format("Execution completed in {0}", ElapsedTime), LogLevel.Information));
            if (e.Report != null)
            {
                ShowReport(e.Report);
            }
        }

        private static void ShowReport(IReport report)
        {
            var r = new ReportViewModel(report);

            var preview = new ReportPreviewView(r) { Owner = Application.Current.MainWindow };
            preview.Show();
        }

        void smartEngine_GenerateCompleted(object sender, GenerateCompletedEventArgs e)
        {
            workItem = e.UnitOfWork;
            Output.Add(new ExecutionLogItemViewModel("Successfully generated the testcase", LogLevel.Information));
            if (GenerateAndExecute)
            {
                Execute();
            }
            else
            {
                isExecuting = false;
            }
        }

        void smartEngine_CompileCompleted(object sender, CompileCompletedEventArgs e)
        {
            workItem = e.UnitOfWork;
            if (workItem == null)
            {
                Output.Add(new ExecutionLogItemViewModel("Compile completed but UnitOfWork is null!", LogLevel.Error));
            }
            else
            {
                Output.Add(new ExecutionLogItemViewModel("Successfully compiled the scenario", LogLevel.Information));
                ExecutionModel = new ExecutionModelViewModel(e.UnitOfWork.Model);
            }

        }

        private void OnExecutionTimerTick(object state)
        {
            elapsedTime = DateTime.Now - executionStartTime;
            SendPropertyChanged("ElapsedTime");
        }

        private void DefectDetected(DefectEventArgs e)
        {
            Output.Add(new ExecutionLogItemViewModel(e.Message, LogLevel.Defect));
        }

        private void OnStart(object obj)
        {
            IsExecuting = true;
            TotalStepCount = 0;
            StepCounter = 0;

            Generate();
        }


        public void Compile()
        {
            Output.Add(new ExecutionLogItemViewModel("Compiling...", LogLevel.Information));
            UnitOfWork work = SmartEngine.CreateUnitOfWork(project, testcase);
            smartEngine.CompileAsync(work, Guid.NewGuid());
        }

        private void Generate()
        {
            Output.Add(new ExecutionLogItemViewModel("Generating...", LogLevel.Information));
            smartEngine.GenerateAsync(workItem, Guid.NewGuid());
        }


        private void Execute()
        {
            if (workItem == null)
            {
                MessageBox.Show("Could not generate an execution model! \nPlease verify your settings.");
                IsExecuting = false;
                return;
            }
            executionStartTime = DateTime.Now;
            executionTimer.Change(0, 100);
            Output.Add(new ExecutionLogItemViewModel("Executing...", LogLevel.Information));
            smartEngine.StepExecuted += ExecutionEngine_StepExecuted;
            TotalStepCount = workItem.TotalSteps; //.Steps.Count();

            currentExecutionTaskId = Guid.NewGuid();
            smartEngine.ExecuteAsync(workItem); //, currentExecutionTaskId);
        }

        void ExecutionEngine_StepExecuted(object sender, StepInfo e)
        {
            StepCounter++;
            TotalStepCount = e.TotalSteps;
            Status = string.Format("Step {0} of {1} ({2:P0})", StepCounter, TotalStepCount, (decimal)stepCounter/totalStepCount);
        }

        public override void ViewLoaded()
        {
            commonCommands.ConfigButtonChecked = false;
            commonCommands.CodeButtonChecked = false;
            commonCommands.ExecuteButtonChecked = true;

            if (ExecutionModel == null)
                Compile();
            Status = "Ready to execute";
        }

        public void Reset()
        {
            ExecutionModel = null;
        }
    }
}