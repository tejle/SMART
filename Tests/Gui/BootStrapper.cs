using System;
using Microsoft.Practices.Unity;
using SMART.Core.DataLayer;
using SMART.Core.DataLayer.Interfaces;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Services;
using SMART.Core.Services;

namespace SMART.Test.Gui
{
    public class BootStrapper {
 
        public static void Configure(IUnityContainer container) {
                container
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
                .RegisterType<IProjectService, ProjectService>()
                .RegisterType<IScenarioService, ScenarioService>()
                .RegisterType<IModelService, ModelService>()
                .RegisterType<IEventService, EventService>(new ContainerControlledLifetimeManager())
                .RegisterType<IStatisticsService, SimpleStatisticsService>()
                .RegisterType(typeof(Random), typeof(Random));

        }
    }
}