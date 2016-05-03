
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using SMART.Core;
using SMART.Core.DomainModel;
using SMART.Core.DomainModel.Layouts;
using SMART.Core.Interfaces;
using SMART.Gui.Controls.DiagramControl;

namespace SMART.Gui.ViewModel
{
    using System;
    using Commands;
    using Events;
    using IOC;
    using Core.Interfaces.Services;

    public class StartViewModel : ViewModelBase
    {
        private readonly BackgroundWorker worker;
        private IModel model;

        private readonly IEventService eventService;

        public override string Icon { get { return Constants.MISSING_ICON_URL; } }
        
        public override Guid Id { get; set; }

        public RecentFileList RecentFiles { get; set; }
        public RoutedActionCommand OpenProject { get; set; }
        public RoutedActionCommand NewProject { get; set; }

        public string VersionInfo
        {
            get
            {
                return string.Format("S.M.A.R.T v{0} © {1} System Verification AB",
                                     Assembly.GetExecutingAssembly().GetName().Version,
                                     DateTime.Now.Year
                    );
            }
        }
        
        readonly ApplicationViewModel applicationViewModel;

        public StartViewModel()
        {
            RecentFiles = new RecentFileList();
            eventService = Resolver.Resolve<IEventService>();
            eventService.GetEvent<OpenProjectEvent>().Subscribe(HandleOpenProjectEvent);
            eventService.GetEvent<SaveProjectEvent>().Subscribe(HandleSaveProjectEvent);

            applicationViewModel = Resolver.Resolve<ApplicationViewModel>();

            RoutedCommands();

            DiagramItems = new ObservableCollection<IDiagramItem>();

            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += WorkerDoWork;
            worker.RunWorkerCompleted += WorkerRunWorkerCompleted;
            worker.ProgressChanged += WorkerProgressChanged;
            worker.RunWorkerAsync();
        }

        void WorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            eventService.GetEvent<LayoutCompleteEvent>().Publish(model);
        }

        void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            DiagramItems.Add(e.UserState as IDiagramItem);
            
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Render, new PublishLayoutComplete(LayoutComplete));
            //Thread.Sleep(100);
        }

        private delegate void PublishLayoutComplete();

        private void LayoutComplete()
        {
            eventService.GetEvent<LayoutCompleteEvent>().Publish(model);
        }

        void WorkerDoWork(object sender, DoWorkEventArgs e)
        {            
            model = LoadItems();
        }

        private void RoutedCommands()
        {
            OpenProject = applicationViewModel.OpenProject;
            NewProject = applicationViewModel.NewProject;
        }

        private void HandleSaveProjectEvent(string fileName)
        {
            RecentFiles.InsertFile(fileName);
        }

        private void HandleOpenProjectEvent(string fileName)
        {
            RecentFiles.InsertFile(fileName);
            SendPropertyChanged("RecentFiles");
        }

        #region Demo Model        

        public ObservableCollection<IDiagramItem> DiagramItems { get; set; }

        private IModel LoadItems()
        {
            var model = CreateModel();
            var layout = new OrthogonalLayout(model);
            layout.BeginLayout();

            var list = (from v in model.States
                        let viewmodel = new StateViewModel(v)
                        select viewmodel).ToList();


            var arrows = (from e in model.Transitions
                          select
                                  new TransitionViewModel(
                                      e,
                                      (from v in list where (v.State == e.Source) select v)
                                          .First(),
                                      (from v in list
                                       where (v.State == e.Destination)
                                       select v).First())).ToList();

            foreach (var a in arrows)
            {
                //DiagramItems.Add(a);
                worker.ReportProgress(0,a);                
                //Thread.Sleep(100);
            }

            foreach (var l in list)
            {
                //DiagramItems.Add(l);
                worker.ReportProgress(0, l);                
                //Thread.Sleep(100);
            }
            return model;
        }

        private State CreateState(string name)
        {
            return  new State(name) { Size = new SmartSize(Constants.NODE_WIDTH, Constants.NODE_HEIGHT)}; 
        }

        private IModel CreateModel()
        {
            var modelA = new Model("A");
            modelA
                .Add(CreateState("First"))
                .Add(CreateState("Second"))
                .Add(CreateState("B"))
                .Add(CreateState("C"))
                .Add(CreateState("Third"))
                .Add(new Transition("StartToFirst")
                {
                    Source = modelA.StartState,
                    Destination = modelA["First"] as State
                })
                .Add(new Transition("FristToSecond")
                {
                    Source = modelA["First"] as State,
                    Destination = modelA["Second"] as State
                })
                .Add(new Transition("SecondToB")
                {
                    Source = modelA["Second"] as State,
                    Destination = modelA["B"] as State
                })
                .Add(new Transition("BToC")
                {
                    Source = modelA["B"] as State,
                    Destination = modelA["C"] as State
                })
                .Add(new Transition("CToThird")
                {
                    Source = modelA["C"] as State,
                    Destination = modelA["Third"] as State
                })
                .Add(new Transition("ThirdToStop")
                {
                    Source = modelA["Third"] as State,
                    Destination = modelA.StopState

                });
            
            return modelA;
        }

        #endregion
    }
}
