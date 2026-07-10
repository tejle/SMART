using System;
using System.ComponentModel.Composition;
using System.Reflection;
using SMART.Core.DomainModel.Validation;
using SMART.Core.Interfaces;
using SMART.Core.Metadata;

namespace SMART.Base.Adapters
{
    using System.Security.Policy;
    using SMART.Core.Interfaces.Services;
    using SMART.Core.Events;

    [Export]
    [Export(typeof(IAdapter))]
    [Adapter(Name = "AssemblyAdapter", Description = "This adapter uses an external assembly")]
    public class AssemblyAdapter : IAdapter
    {
        private readonly IEventService eventService;
        AppDomain testDomain;
        private object _instance;

        public event EventHandler<DefectEventArgs> DefectDetected;

        [Config(Description = "The full class name in the assembly, i.e. namespace.classname")]
        [Required]
        public string ClassName { get; set; }

        [Config(Editor = ConfigEditor.Assembly, Description = "The assembly to execute against")]
        [Required]
        [FileMustExist]
        public string AssemblyPath { get; set; }

        private object Instance
        {
            get
            {
                if (_instance == null)
                {
                    //var evidence = new Evidence(AppDomain.CurrentDomain.Evidence);
                    //testDomain = AppDomain.CreateDomain(
                    //        "TestDomain", evidence, AppDomain.CurrentDomain.BaseDirectory, this.AssemblyPath, true);
                    var assembly = Assembly.LoadFrom(AssemblyPath);
                    //var assembly = testDomain.Load(AssemblyPath);
                    //_instance = testDomain.CreateInstance(AssemblyPath, ClassName);
                    _instance = assembly.CreateInstance(ClassName);
                    signupToEvent();
                    if (_instance == null)
                    {
                        eventService
                            .GetEvent<ErrorMessageEvent>()
                            .Publish(new ErrorMessage("Assembly adapter could not create the class: " + ClassName));
                    }
                }
                return _instance;
            }
        }

        private void signupToEvent()
        {
            var i = _instance as IDefectDetected;
            if (i == null) return;

            i.DefectDetected += OnDefectDetected;
        }

        private void OnDefectDetected(object sender, EventArgs eventArgs)
        {
            Console.WriteLine("defect");
        }

        public AssemblyAdapter(IEventService eventService)
        {
            this.eventService = eventService;
        }

        public bool Execute(string function, params string[] args)
        {
            try
            {
                if (args == null) //This is a state!
                {
                    Execute(Instance, function.Replace(" ", "_"), null);
                }
                else
                {
                    Execute(Instance, function.Replace(" ", "_"), new object[] { args });
                }
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                    InvokeDefectDetected(e.InnerException);
                else
                    InvokeDefectDetected(e);
                return false;
            }
            return true;
        }

        private void InvokeDefectDetected(Exception e)
        {
            var tmp = DefectDetected;
            if (tmp != null)
                tmp(this, new DefectEventArgs(null, e.Message));
        }

        public void PreExecution() { }
        public void PostExection() { }

        private static void Execute(object obj, string method, object[] parameters)
        {
            obj.GetType().GetMethod(method).Invoke(obj, parameters);
        }

        public void Dispose()
        {
            try
            {
                if(_instance == null) return;

                _instance.GetType().GetMethod("Dispose").Invoke(_instance, null);
            }
            catch
            {
                // Do nothing, the above might fail and we don't know why
            }

            //   AppDomain.Unload(testDomain);
        }


    }

}
