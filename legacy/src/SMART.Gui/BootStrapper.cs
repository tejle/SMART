using System;
using Microsoft.Practices.Unity;
using SMART.Core;
using SMART.Core.DataLayer;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Factories;
using SMART.Core.Interfaces.Services;
using SMART.Core.Services;
using SMART.Core.Interfaces.Repository;
using SMART.Core.Workflow;
using SMART.Gui.ViewModel;

namespace SMART.Gui
{
    public class BootStrapper
    {
        public static void Configure(IUnityContainer container)
        {
            container
                //.RegisterType<IBusinessLayer, ApplicationManager>()
                .RegisterType<ITestcase, Testcase>()
                .RegisterType<IProject, Project>()
                .RegisterType<IModel, Model>()
                .RegisterType<IProjectIOHandler, ProjectIOHandler>()
                .RegisterType<IProjectReader, ProjectReader>()
                .RegisterType<IModelReader, ModelReader>()
                .RegisterType<IReportReader, ReportReader>()
                .RegisterType<ITestcaseReader, TestcaseReader>()
                .RegisterType<IProjectWriter, ProjectWriter>()
                .RegisterType<IModelWriter, ModelWriter>()
                .RegisterType<IReportWriter, ReportWriter>()
                .RegisterType<ITestcaseWriter, TestcaseWriter>()
                //.RegisterType<ISandbox, Sandbox>()
                .RegisterType<IProjectService, ProjectService>()
                .RegisterType<IScenarioService, ScenarioService>()
                .RegisterType<IModelService, ModelService>()
                .RegisterType<IEventService, EventService>(new ContainerControlledLifetimeManager())
                .RegisterType<IStatisticsService, SimpleStatisticsService>()
                .RegisterType<IAdapterRepository, AdapterRepository>()
                .RegisterType<IAdapterFactory, AdapterFactory>()
                .RegisterType<IAlgorithmRepository, AlgorithmRepository>()
                .RegisterType<IAlgorithmFactory, AlgorithmFactory>()
                .RegisterType<IStatisticsRepository, StatisticsRepository>()
                .RegisterType<IStatisticsFactory, StatisticsFactory>()
                .RegisterType<IGenerationStopCriteriaRepository, GenerationStopCriteriaRepository>()
                .RegisterType<IGenerationStopCriteriaFactory, GenerationStopCriteriaFactory>()
                .RegisterType<IExecutionStopCriteriaRepository, ExecutionStopCriteriaRepository>()
                .RegisterType<IExecutionStopCriteriaFactory, ExecutionStopCriteriaFactory>()
                .RegisterType<IModelCompiler, ModelCompiler>()
                .RegisterType<IExecutionEngine, TestExecutionEngine>()
                .RegisterType<IGenerationEngine, TestGenerationEngine>()
                .RegisterType<IViewModelFactory, ViewModelFactory>()
                .RegisterType<ICodeGenerationService, CodeGenerationService>()
                .RegisterType<SmartEngine, SmartEngine>(new ContainerControlledLifetimeManager())
                .RegisterInstance(typeof(Random), new Random());


        }

        //      <container>
        //  <types>
        //    <type type="SMART.Core.BusinessLayer.IBusinessLayer,SMART.Core" mapTo="SMART.Core.BusinessLayer.ApplicationManager,SMART.Core"/>
        //    <type type="SMART.Core.Interfaces.ITestcase,SMART.Core" mapTo="SMART.Core.DomainModel.Testcase,SMART.Core"/>
        //    <type type="SMART.Core.Interfaces.IProject,SMART.Core" mapTo="SMART.Core.DomainModel.Project,SMART.Core"/>
        //    <type type="SMART.Core.DomainModel.Model,SMART.Core" mapTo="SMART.Core.DomainModel.Model,SMART.Core"/>
        //    <type type="SMART.Core.BusinessLayer.IStatisticsManager,SMART.Core" mapTo="SMART.Core.BusinessLayer.StatisticsManager,SMART.Core"/>
        //    <type type="SMART.Core.DataLayer.IProjectIOHandler,SMART.Core" mapTo="SMART.Core.DataLayer.ProjectIOHandler,SMART.Core"/>
        //    <type type="SMART.Core.DataLayer.IProjectReader,SMART.Core" mapTo="SMART.Core.DataLayer.ProjectReader,SMART.Core"/>
        //    <type type="SMART.Core.DataLayer.IModelReader,SMART.Core" mapTo="SMART.Core.DataLayer.ModelReader,SMART.Core"/>
        //    <type type="SMART.Core.DataLayer.ITestcaseReader,SMART.Core" mapTo="SMART.Core.DataLayer.TestcaseReader,SMART.Core"/>
        //    <type type="SMART.Core.DataLayer.IProjectWriter,SMART.Core" mapTo="SMART.Core.DataLayer.ProjectWriter,SMART.Core"/>
        //    <type type="SMART.Core.DataLayer.IModelWriter,SMART.Core" mapTo="SMART.Core.DataLayer.ModelWriter,SMART.Core"/>
        //    <type type="SMART.Core.DataLayer.ITestcaseWriter,SMART.Core" mapTo="SMART.Core.DataLayer.TestcaseWriter,SMART.Core"/>
        //    <type type="SMART.Core.Interfaces.ISandbox,SMART.Core" mapTo="SMART.Base.Sandboxes.Sandbox,SMART.Base"/>
        //    <type type="SMART.Core.IModelBuilder,SMART.Core" mapTo="SMART.Core.ModelBuilder,SMART.Core"/>
        //    <type type="SMART.Core.BusinessLayer.ITestcaseExecutor,SMART.Core" mapTo="SMART.Core.BusinessLayer.TestcaseExecutor,SMART.Core"/>
        //    <type type="SMART.Core.Interfaces.Services.IProjectService, SMART.Core" mapTo="SMART.Core.Services.ProjectService, SMART.Core.Services"/>
        //    <type type="SMART.Core.Interfaces.Services.ITestcaseService, SMART.Core" mapTo="SMART.Core.Services.TestcaseService, SMART.Core.Services"/>
        //    <type type="SMART.Core.Interfaces.Services.IModelService, SMART.Core" mapTo="SMART.Core.Services.ModelService, SMART.Core.Services"/>
        //    <type type="System.Random, mscorlib" mapTo="System.Random, mscorlib">
        //      <typeConfig extensionType="Microsoft.Practices.Unity.Configuration.TypeInjectionElement,
        //          Microsoft.Practices.Unity.Configuration">
        //        <constructor/>
        //      </typeConfig>
        //    </type>

        //  </types>
        //</container>
    }
}