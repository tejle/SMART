using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using SMART.Base.StopCriterias;
using SMART.Core;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Factories;
using SMART.Core.Interfaces.Repository;
using SMART.Core.Interfaces.Services;
using SMART.Core.Services;
using SMART.IOC;
using SMART.Base.Algorithms;
using SMART.Core.DomainModel;
using SMART.Base.Statistics;

namespace SMART.Console
{
    public class Program
    {
        Runner runner;
        ManualResetEvent resetEvent = new ManualResetEvent(false);

        public Program(string[] args)
        {
            //if (args.Length < 1)
            //{
            //    ShowParameterError();
            //    return;
            //}
            ConsoleBootStrapper.Configure(Resolver.Container);
       
            //var loader = Resolver.Resolve<IProjectIOHandler>();
            //var project = loader.Load(args[0]);

            var project = ConsoleBootStrapper.GetDummyProject();


            runner = new Runner(project);
            
            System.Console.CancelKeyPress += Console_CancelKeyPress;

            readInput();
            
        }

        private void readInput()
        {
            ConsoleKeyInfo key;
            key = System.Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.S:
                    start();
                    break;
                case ConsoleKey.P:
                    runner.StopExecution();
                    break;
                case ConsoleKey.Q:
                    return;
                case ConsoleKey.G:
                    GC.Collect();
                    break;
                case ConsoleKey.UpArrow:
                    runner.DelayBetweenExectionSteps += 10;
                    break;
                case ConsoleKey.DownArrow:
                    if(runner.DelayBetweenExectionSteps >= 10)
                        runner.DelayBetweenExectionSteps -= 10;
                    break;

            }
          readInput();
        }

        void start()
        {
            runner.Execute();
        
        }

        void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            runner.StopExecution();
            System.Console.WriteLine("Execution is stopped");
            System.Console.WriteLine("Press Space to restart, any other key to Exit");
            if( System.Console.ReadKey().Key == ConsoleKey.Spacebar)
            {
                start();
               
            }
            else
                Environment.Exit(0);
        }

        private void parseParameters(string[] args)
        {
            if(args.Length == 1) // execute as normal
            {
                runner.Execute();                
            }
        }

        private static void ShowParameterError()
        {
            System.Console.WriteLine("No parameter used, use the help option.");
            PrintHelp();

        }

        private static void PrintHelp()
        {
            System.Console.WriteLine("SMART Console Runner");
        }

        static void Main(string[] args)
        {
            //ConfigureIOC();
            new Program(args);
        }

        private static void ConfigureIOC()
        {
            
            Resolver.Configure();
            //Resolver.Register<ITestcaseReader, TestcaseReader>();
            //Resolver.Register<IModelReader, ModelReader>();
            //Resolver.Register<IProjectReader, ProjectReader>();
            //Resolver.Register<IModelWriter, ModelWriter>();
            //Resolver.Register<IModelReader, ModelReader>();
            //Resolver.Register<IProjectWriter, ProjectWriter>();
            //Resolver.Register<ITestCase, TestCase>();
            //Resolver.Register<IStatisticsManager, StatisticsManager>();
            ////Resolver.Register<ProjectIOHandler, ProjectIOHandler>();
            //Resolver.RegisterInstance<Random>(new Random());

        }
    }

    public class Runner
    {
        private readonly IProject project;
        ITestcase testcase;

        public Runner(IProject project)
        {
            this.project = project;
            
        }

        public int DelayBetweenExectionSteps { get; set; }
        private ManualResetEvent wait = new ManualResetEvent(false);
        public void Execute()
        {
            testcase = project.Testcases.First();
            testcase.Add(new AssistedRandomAlgorithm(new Random()));
            
            IModelCompiler compiler = new SimpleModelCompiler();
            IEventService eventService = new EventService();
            var statisticRepository = Resolver.Resolve<IStatisticsRepository>();
            var statisticFactory = Resolver.Resolve<IStatisticsFactory>();
            
            var statistic = statisticRepository.GetAll().OfType<StateCoverageStatistic>().First();
            var stateCoverage = new StateCoverageGenerationStopCriteria {Statistic = statistic, Threshold = 1.00d};
            testcase.Add(stateCoverage);

            IStatisticsService statisticsService = new SimpleStatisticsService(eventService);
            var list = new List<IStatistic>();
            list.Add(stateCoverage.Statistic);
            statisticsService.Statistics = list;

            //var engine = new TestGenerationEngine(testcase, compiler, eventService, statisticsService);

            //engine.Generate_Done += new EventHandler(engine_Generate_Done);
            //engine.Traverse_Done += new EventHandler(engine_Traverse_Done);
            //wait.Reset();
            //engine.Compile();

            //engine.TraverseModel();
            //wait.WaitOne();
            //wait.Reset();
            //engine.GenerateTestCase();
            //wait.WaitOne();

            
            //foreach (var step in engine.Sequence)
            //{
            //    System.Console.Out.WriteLine(step);
            //}
            
        }

        void engine_Traverse_Done(object sender, EventArgs e) {
            wait.Set();
        }

        void engine_Generate_Done(object sender, EventArgs e) {
            wait.Set();
        }

        public void StopExecution()
        {
            
        }
    }
}
