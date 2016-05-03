using System.Collections.Generic;
using SMART.Gui.Controls.DiagramControl.View;
using SMART.Gui.View;
using SMART.Gui.View.TestcaseExecution;

namespace SMART.Gui.ViewModel.TestcaseExecution
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;

    using Commands;

    using Controls.DiagramControl;

    using Core.DomainModel;
    using Core.DomainModel.Layouts;
    using Core.Interfaces;
    using Core.Interfaces.Services;

    using Events;

    using Interfaces;
    using IOC;

    public class ExecutionModelViewModel : ViewModelBase, IDiagramViewModel
    {
        public IModel Model { get; set; }

        public override string Icon { get { return Constants.MISSING_ICON_URL; } }

        public RoutedActionCommand Zoom { get; set; }
        public RoutedActionCommand Pan { get; set; }
        public RoutedActionCommand Layout { get; set; }

        private readonly ObservableCollection<IDiagramItem> diagramItems = new ObservableCollection<IDiagramItem>();

        #region IDiagramViewModel
        public override Guid Id
        {
            get { return this.Model.Id; }
            set { this.Model.Id = value; }
        }

        public RoutedActionCommand Rename
        {
            get;
            set;
        }

        public bool IsFirstRun
        {
            get; set;
        }

        public bool IsGrayed
        {
            get; set;
        }

        public CanvasData CanvasInfo
        {
            get; set;
        }

        public void BeginModelBatch()
        {
        }

        public void EndModelBatch()
        {
        }

        public State AddStateToDomainModel(Point location)
        {
            return null;
        }

        public State AddStateToDomainModel(Point location, State state)
        {
            return null;
        }

        public State AddStateToDomainModel(Point location, State state, string label)
        {
            return null;
        }

        public Transition AddTransitionToDomainModel(IConnectable source, IConnectable destination)
        {
            return null;
        }

        public Transition AddTransitionToDomainModel(IConnectable source, IConnectable destination, Transition transition)
        {
            return null;
        }

        public void RemoveElementFromDomainModel(ISelectable element)
        {
        }

        public void ToggleGridLines()
        {
        }

        public void UpdateTransitionSource(Transition transition, IConnectable newSource)
        {
        }

        public void UpdateTransitionTarget(Transition transition, IConnectable newTarget)
        {
        } 
        #endregion

        private List<LayoutType> layouts;
        public List<LayoutType> Layouts
        {
            get
            {
                if (layouts == null)
                {
                    layouts = new List<LayoutType>
                                  {
                                      new LayoutType { Id= "Orthogonal", Name = "Orthogonal"},
                                      new LayoutType { Id= "TreeLayout", Name = "TreeLayout"},
                                      new LayoutType(){Id = "Radial", Name="Radial"}
                                  };
                }
                return layouts;
            }
        }

        public ObservableCollection<IDiagramItem> DiagramItems
        {
            get
            {
                if (diagramItems.Count == 0)
                {
                    LoadItems();
                }
                return diagramItems;
            }            
        }

        public IDiagramItem CurrentItem
        {
            get; set;
        }

        public RoutedActionCommand Delete
        {
            get;
            set;
        }

        private void LoadItems()
        {
            var list = (from v in this.Model.States
                        let viewmodel = new StateViewModel(v)
                        select viewmodel).ToList();

            var arrows = (from e in this.Model.Transitions
                          select
                                  new TransitionViewModel(
                                      e,
                                      (from v in list 
                                       where (v.State == e.Source) 
                                       select v).FirstOrDefault(),
                                       (from v in list
                                       where (v.State == e.Destination)
                                       select v).FirstOrDefault())).ToList();
            
            
            
            
            foreach (var a in arrows)
            {
                a.ShowVisitCounter = true;
                diagramItems.Add(a);
            }

            foreach (var l in list)
            {
                l.ShowVisitCounter = true;
                diagramItems.Add(l);
            }
        }

        public ExecutionModelViewModel() : this(null)
        {
        }

        public ExecutionModelViewModel(IModel model)
        {
            this.Model = model;
            CanvasInfo = new CanvasData();
            CreateCommand();
        }

        public void LayoutModel(string type)
        {
            ILayout layout;

            switch (type)
            {
                case "Orthogonal":
                    layout = new OrthogonalLayout(Model);
                    layout.BeginLayout();
                    break;
                case "TreeLayout":
                    layout = new WalkerTreeLayout(Model);
                    layout.BeginLayout();
                    break;                    
                case "Radial":
                    layout = new RadialTreeLayout(Model);
                    layout.BeginLayout();
                    break;
            }

            DiagramItems.Clear();
            LoadItems();

            DiagramCanvas.Zoom("All");
        }

        public override void ViewLoaded()
        {
            this.LayoutModel("TreeLayout");
            DiagramCanvas.EditMode = DiagramCanvas.EditorMode.PanAndZoom;
        }

        private void CreateCommand()
        {
            Zoom = new RoutedActionCommand("ZoomAll", typeof(ModelViewModel))
            {
                Description = "Zoom",
                OnCanExecute = (o) => true,
                OnExecute = OnZoom,
                Text = "Zoom",
                Icon = Constants.MISSING_ICON_URL
            };

            Pan = new RoutedActionCommand("Pan", typeof(ModelViewModel))
            {
                Description = "Pan",
                OnCanExecute = (o) => true,
                OnExecute = OnPan,
                Text = "Pan",
                Icon = Constants.MISSING_ICON_URL
            };

            Layout = new RoutedActionCommand("Layout", typeof(ModelViewModel))
            {
                Description = "Layout",
                OnCanExecute = (o) => true,
                OnExecute = OnLayout,
                Text = "Layout",
                Icon = Constants.MISSING_ICON_URL
            };
        }

        private void OnLayout(object type)
        {
            LayoutModel(type.ToString());
        }

        private void OnPan(object direction)
        {
            DiagramCanvas.Pan(direction.ToString());
        }

        private void OnZoom(object action)
        {
            DiagramCanvas.Zoom(action.ToString());
        }

        public DiagramCanvas DiagramCanvas { get; set; }

        //private DiagramCanvas DiagramCanvas
        //{
        //    get { return (View as ExecutionModelView).diagramViewer.diagramView.DiagramCanvas; }
        //}
    }
}