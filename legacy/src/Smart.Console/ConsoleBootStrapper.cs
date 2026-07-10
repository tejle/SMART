using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using SMART.Core.BusinessLayer;
using SMART.Core.DataLayer;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Repository;
using SMART.Core.Interfaces.Services;
using SMART.Core.Services;

namespace SMART.Console
{
    public class ConsoleBootStrapper {
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
        //        <type type="System.Random, mscorlib" mapTo="System.Random, mscorlib">
        //      <typeConfig extensionType="Microsoft.Practices.Unity.Configuration.TypeInjectionElement,
        //          Microsoft.Practices.Unity.Configuration">
        //        <constructor/>
        //      </typeConfig>
        //    </type>

        //  </types>
        //</container>

        public static void Configure(IUnityContainer container) {
            container
                //.RegisterType<IBusinessLayer, ApplicationManager>()
                .RegisterType<ITestcase, Testcase>()
                .RegisterType<IProject, Project>()
                .RegisterType<IModel, Model>()
                .RegisterType<IProjectIOHandler, ProjectIOHandler>()
                .RegisterType<IProjectReader, ProjectReader>()
                .RegisterType<IModelReader, ModelReader>()
                .RegisterType<ITestcaseReader, TestcaseReader>()
                .RegisterType<IProjectWriter, ProjectWriter>()
                .RegisterType<IModelWriter, ModelWriter>()
                .RegisterType<ITestcaseWriter, TestcaseWriter>()
                //.RegisterType<ISandbox, Sandbox>()
                .RegisterType<IProjectService, ProjectService>()
                .RegisterType<IScenarioService, ScenarioService>()
                .RegisterType<IModelService, ModelService>()
                .RegisterType<IEventService, EventService>(new ContainerControlledLifetimeManager())
                .RegisterType<IStatisticsRepository, StatisticsRepository>()
                .RegisterType(typeof(Random), typeof(Random));

        }

        public static IProject GetDummyProject()
        {
            var project = new Project(Guid.NewGuid())
                              {
                                  
                                  Name = "TestProject",
                                  
                              };

            Model model = new Model("TestModel");
            model
                .Add(new State("StateA"))
                .Add(new State("StateB"))
                .Add(new State("StateC"))
                .Add(new Transition("StartToA") {Source = model.StartState, Destination = model["StateA"] as State, Parameter = "param1"})
                .Add(new Transition("AToB") {Source = model["StateA"] as State, Destination = model["StateB"] as State})
                .Add(new Transition("BToC") {Source = model["StateB"] as State, Destination = model["StateC"] as State})
                .Add(new Transition("CToStop") {Source = model["StateC"] as State, Destination = model.StopState});
            Testcase testcase = new Testcase("Testcase");
            
            project.AddTestCase(testcase);
            
            project.AddModel(model,testcase);
            return project;
        }
    }
}