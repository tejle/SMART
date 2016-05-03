using System;
using System.ComponentModel.Composition;
using System.Configuration;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.Practices.Unity.ObjectBuilder;
using Microsoft.Practices.ObjectBuilder2;

namespace SMART.IOC
{
    public static class Resolver
    {
        private static readonly UnityContainer container;
        public static IUnityContainer Container { get { return container; } }
        static Resolver()
        {
            container = new UnityContainer();

            //var mefContainer = new CompositionContainer(new DirectoryPartCatalog(@".\Extentions"));
            //container.RegisterInstance<ICompositionService>(mefContainer);

            //container.AddNewExtension<MEFContainerExtension>();
        }

        public static void Register<T,K>() where K : T
        {
            container.RegisterType<T,K>();
        }

        public static void RegisterInstance<T>(object instance)
        {
            container.RegisterInstance(typeof(T),instance);
        }

        public static T Resolve<T>()
        {
            return container.Resolve<T>();
        }

        public static T BuildUp<T>(T instance)
        {
            return container.BuildUp(instance);
        }

        public static void Configure()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var section = (UnityConfigurationSection)config.GetSection("unity");
            section.Containers.Default.Configure(container);
        }

        public static void RegisterSingleton(Type from, Type to)
        {

            container.RegisterType(from, to, new ContainerControlledLifetimeManager());
        }

        public static object ResolveType(Type type)
        {
            return container.Resolve(type);
        }

        
    }

    public class MEFContainerExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Context.Strategies.AddNew<MEFStrategy>(UnityBuildStage.Initialization);
        }
    }

    public class MEFStrategy : BuilderStrategy {
        private bool needComposition;

        public override void PostBuildUp(IBuilderContext context)
        {
            if(needComposition)
            {
                var policy = context.Policies.Get<ILifetimePolicy>(new NamedTypeBuildKey(typeof(ICompositionService)));
                var compositionService = policy.GetValue() as ICompositionService;
                if (compositionService != null) compositionService.Compose();
                needComposition = false;
            }
            base.PostBuildUp(context);
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            var policy = context.Policies.Get<ILifetimePolicy>(new NamedTypeBuildKey(typeof(ICompositionService)));
            var compositionService = policy.GetValue() as ICompositionService;

            if (compositionService != null)
            {
                compositionService.AddPart(context.Existing);
                needComposition = true;
            }
            base.PreBuildUp(context);
        }
    }
}
