using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using SMART.Gui.View;

namespace SMART.Gui.ViewModel
{
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using Core.Interfaces;
    using Image = Image;
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows;
    using Commands;
    using Controls.DiagramControl;
    using Controls.DiagramControl.View;
    using Core;
    using Core.DomainModel;
    using Core.Events;
    using Interfaces;

    using IOC;
    using Core.DomainModel.Layouts;

    public class ModelViewModel : ViewModelBase, IDiagramViewModel
    {
        private const int StateInsertIndex = 0;
        private readonly IModel internalModel;
        private IModel model;
        private IBatchModel batchModel;

        //private readonly SnapToGridRenderer grid;// = new SnapToGridRenderer();

        public RoutedActionCommand Zoom { get; set; }
        public RoutedActionCommand Pan { get; set; }
        public RoutedActionCommand EditMode { get; set; }
        public RoutedActionCommand Layout { get; set; }

        public RoutedActionCommand Delete { get; set; }

        public RoutedActionCommand MakeRef { get; set; }

        public IModel Model { get { return model; } }

        public bool IsFirstRun { get; set; }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; SendPropertyChanged("IsSelected"); }
        }

        public override string Name
        {
            get { return base.Name; }
            set { base.Name = value; SendPropertyChanged("Name"); }
        }

        private bool isGrayed;

        public bool IsGrayed
        {
            get { return isGrayed; }
            set
            {
                isGrayed = value;
                if (DiagramCanvas != null)
                {
                    if (isGrayed)
                        DiagramCanvas.SetEditMode(DiagramCanvas.EditorMode.PanAndZoom);
                    else
                        DiagramCanvas.SetEditMode(DiagramCanvas.EditorMode.CreateStatesAndTransitions);
                }
                SendPropertyChanged("IsGrayed");
            }
        }

        public CanvasData CanvasInfo
        {
            get; set;
        }

      //private LayoutType currentLayout;
      //public LayoutType CurrentLayout
      //{
      //  get
      //  {
      //    return currentLayout;
      //  }
      //  set
      //  {
      //    if (currentLayout != value)
      //    {
      //      currentLayout = value;
      //      OnLayout(currentLayout);
      //    }
      //  }
      //}

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
                                      new LayoutType { Id= "Radial", Name = "Radial" }
                                  };
                }
                return layouts;
            }
        }

        public List<TransitionViewModel> Transitions
        {
            get
            {
                return diagramItems.OfType<TransitionViewModel>().ToList();
            }
        }

        private IDiagramItem currentItem;

        public IDiagramItem CurrentItem
        {
            get { return currentItem; }
            set
            {
                currentItem = value;
                if (!DiagramCanvas.SelectionService.GetComponentSelected(currentItem))
                {
                    DiagramCanvas.SelectionService.SetSelectedComponents(new[] { currentItem });
                }
                SendPropertyChanged("CurrentItem");
            }
        }

        private readonly ObservableCollection<IDiagramItem> diagramItems = new ObservableCollection<IDiagramItem>();

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

        private void LoadItems()
        {
            var states = (from v in model.States
                        let viewmodel = new StateViewModel(v)
                        select viewmodel).ToList();


            var transitions = (from e in model.Transitions
                          select
                                  new TransitionViewModel(
                                      e,
                                      (from v in states where (v.State == e.Source) select v)
                                          .First(),
                                      (from v in states
                                       where (v.State == e.Destination)
                                       select v).First())).ToList();

            
            //diagramItems.Add(grid);

            foreach (var a in transitions)
            {
                diagramItems.Add(a);
            }

            foreach (var l in states)
            {
                diagramItems.Insert(0, l);
            }                        
        }

        public override Guid Id
        {
            get { return model.Id; }
            set { model.Id = value; }
        }

        public ModelViewModel(IModel model)
            : base(model.Name)
        {
            internalModel = model;
            this.model = internalModel;
            model.CollectionChanged += model_CollectionChanged;
            model.PropertyChanged += model_PropertyChanged;
            
            IsFirstRun = true;
            CanvasInfo = new CanvasData() { ScaleViewX = 1.0d, ScaleViewY = 1.0d };

            CreateCommand();
        }

        void model_PropertyChanged(object sender, SmartPropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Name"))
            {
                if (Name != model.Name)
                    Name = model.Name;
            }
        }

        void model_CollectionChanged(object sender, SmartNotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case SmartNotifyCollectionChangedAction.Add:
                    AddElement(e);
                    break;
                case SmartNotifyCollectionChangedAction.Remove:
                    RemoveElement(e);
                    break;
            }
        }

        private void AddElement(SmartNotifyCollectionChangedEventArgs e)
        {
            foreach (var newItem in e.NewItems)
            {
                if (newItem is State)
                {
                    var state = new StateViewModel(newItem as State) { StartInEditMode = true };
                    DiagramItems.Insert(StateInsertIndex, state);
                }

                if (newItem is Transition)
                {
                    var t = newItem as Transition;
                    var source =
                            (from n in DiagramItems
                             where
                                     n is StateViewModel
                                     && (n as StateViewModel).State == t.Source
                             select n).First() as StateViewModel;

                    var target =
                            (from n in DiagramItems
                             where
                                     n is StateViewModel
                                     && (n as StateViewModel).State == t.Destination
                             select n).First() as StateViewModel;

                    var line = new TransitionViewModel(newItem as Transition, source, target) { StartInEditMode = true };
                    DiagramItems.Add(line);
                }
            }
            UpdateThumbnail();
            SendPropertyChanged("Transitions");
        }

        public void UpdateThumbnail()
        {
            model.Thumbnail = new SmartImage(StreamImage(GetBitmap()));
        }

        private static byte[] StreamImage(Image image)
        {
            if (image.Source != null && image.Source is RenderTargetBitmap)
            {
                //Image image = new Image(); 
                //using (MemoryStream memoryStream = new MemoryStream(imageContent))
                //{
                //  BitmapImage imageSource = new BitmapImage(); 
                //  imageSource.BeginInit(); 
                //  imageSource.StreamSource = memoryStream; 
                //  imageSource.EndInit(); 
                //  image.Source = imageSource;
                //}
                var src = image.Source as BitmapSource;

                var stream = new MemoryStream();
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(src));
                encoder.Save(stream);
                stream.Flush();
                return stream.ToArray();
            }
            return null;
        }


        private void RemoveElement(SmartNotifyCollectionChangedEventArgs e)
        {
            foreach (var oldItem in e.OldItems)
            {
                IDiagramItem item = null;

                if (oldItem is State)
                {
                    var s = oldItem as State;

                    item = (from n in DiagramItems
                            where n is StateViewModel && (n as StateViewModel).State == s
                            select n).FirstOrDefault();

                }
                if (oldItem is Transition)
                {
                    var t = oldItem as Transition;

                    item = (from n in DiagramItems
                            where n is TransitionViewModel && (n as TransitionViewModel).Transition == t
                            select n).FirstOrDefault();

                }
                if (item != null)
                {
                    DiagramItems.Remove(item);
                }
            }
            UpdateThumbnail();
            SendPropertyChanged("Transitions");

        }

        public State AddStateToDomainModel(Point location)
        {
            return AddStateToDomainModel(location, null, null);
        }

        public State AddStateToDomainModel(Point location, State state)
        {
            return AddStateToDomainModel(location, state, null);
        }

        public State AddStateToDomainModel(Point location, State state, string label)
        {
            State newState;

            if (state == null)
            {
                newState = Resolver.Resolve<State>();
                newState.Location = new SmartPoint(location.X, location.Y);
                newState.Size = new SmartSize(Constants.NODE_WIDTH, Constants.NODE_HEIGHT);
                newState.Label = label ?? "{new state}";
            }
            else
            {
                newState = state;
            }
            model.Add(newState);
            return newState;
        }

        public Transition AddTransitionToDomainModel(IConnectable source, IConnectable destination)
        {
            return AddTransitionToDomainModel(source, destination, null);
        }

        public Transition AddTransitionToDomainModel(IConnectable source, IConnectable destination, Transition transition)
        {
            Transition newTransition;
            if (transition == null)
            {
                newTransition = Resolver.Resolve<Transition>();
                newTransition.Source = source.State;
                newTransition.Destination = destination.State;
                newTransition.Label = "{new transition}";
            }
            else
            {
                newTransition = transition;
            }
            model.Add(newTransition);
            return newTransition;
        }

        public void RemoveElementFromDomainModel(ISelectable element)
        {
            if (element is StateViewModel)
            {
                if (diagramItems.OfType<StateViewModel>().Contains(element as StateViewModel))
                    model.Remove((element as StateViewModel).State);
            }

            if (element is TransitionViewModel)
            {
                if (diagramItems.OfType<TransitionViewModel>().Contains(element as TransitionViewModel))
                    model.Remove((element as TransitionViewModel).Transition);
            }
        }

        private void CreateCommand()
        {
            Zoom = new RoutedActionCommand("ZoomAll", typeof(ModelViewModel))
                         {
                             Description = "Zoom",
                             OnCanExecute = o => true,
                             OnExecute = OnZoom,
                             Text = "Zoom",
                             Icon = Constants.MISSING_ICON_URL
                         };

            Pan = new RoutedActionCommand("Pan", typeof(ModelViewModel))
                            {
                                Description = "Pan",
                                OnCanExecute = o => true,
                                OnExecute = OnPan,
                                Text = "Pan",
                                Icon = Constants.MISSING_ICON_URL
                            };

            Layout = new RoutedActionCommand("Layout", typeof(ModelViewModel))
                            {
                                Description = "Layout",
                                OnCanExecute = o => true,
                                OnExecute = OnLayout,
                                Text = "Layout",
                                Icon = Constants.MISSING_ICON_URL
                            };

            EditMode = new RoutedActionCommand("EditMode", typeof(ModelViewModel))
                            {
                                Description = "EditMode",
                                OnCanExecute = o => true,
                                OnExecute = OnEditMode,
                                Text = "EditMode",
                                Icon = Constants.MISSING_ICON_URL
                            };

            //Delete = new RoutedActionCommand("Delete", typeof(ModelViewModel))
            //             {
            //                 Description = "Delete the item",
            //                 OnCanExecute = (o) => true,
            //                 OnExecute = OnDelete,
            //                 Text = "Delete",
            //                 Icon = Constants.DELETE_ICON_URL
            //             };


            //MakeRef = new RoutedActionCommand("MakeRef", typeof (ModelViewModel))
            //            {
            //                Description = "Make state to reference",
            //                OnCanExecute = (o) => o is StateViewModel,
            //                OnExecute = OnMakeRef,
            //                Text = "Make Reference",
            //                Icon = Constants.MISSING_ICON_URL
            //            };
        }

        private void OnEditMode(object mode)
        {
            switch (mode.ToString())
            {
                case "PanAndZoom":
                    DiagramCanvas.SetEditMode(DiagramCanvas.EditorMode.PanAndZoom);
                    break;
                case "CreateNodes":
                    DiagramCanvas.SetEditMode(DiagramCanvas.EditorMode.CreateStatesAndTransitions);
                    break;
                case "Nearest":
                    DiagramCanvas.SetEditMode(DiagramCanvas.EditorMode.NearestNeighbour);
                    break;
            }
        }

        private void OnLayout(object type)
        {
            if (type != null)
            {
                LayoutModel(type.ToString());                      
            }
        }

        private void OnPan(object direction)
        {
            DiagramCanvas.Pan(direction.ToString());
        }

        private void OnZoom(object action)
        {
            DiagramCanvas.Zoom(action.ToString());
        }
        

        //private void OnMakeRef(object obj)
        //{
        //    (obj as StateViewModel).State.Type = StateType.GlobalReference;
        //}

        //private void OnDelete(object obj)
        //{
        //    RemoveElementFromDomainModel(obj as ISelectable);
        //}

        public override string Icon
        {
            get
            {
                return Constants.GRAPH_ICON_URL;
            }
        }


        public void UpdateTransitionSource(Transition transition, IConnectable newSource)
        {
            var newSourceState = newSource as StateViewModel;

            if (newSourceState != null)
            {
                model.ChangeTransitionSource(transition, newSourceState.State);
            }
        }

        public void UpdateTransitionTarget(Transition transition, IConnectable newTarget)
        {
            var newTargetState = newTarget as StateViewModel;

            if (newTargetState != null)
            {
                model.ChangeTransitionDestination(transition, newTargetState.State);
            }

        }

        public void LayoutModel(string type)
        {
            ILayout layout;

            switch (type)
            {
                case "Orthogonal":
                    layout = new OrthogonalLayout(model);
                    layout.BeginLayout();
                    break;
                case "TreeLayout":
                    layout = new WalkerTreeLayout(model);
                    layout.BeginLayout();
                    break;
                case "Radial":
                    layout = new RadialTreeLayout(model);
                    layout.BeginLayout();
                    break;

            }

            DiagramItems.Clear();
            LoadItems();

            DiagramCanvas.Zoom("All");            
            //eventService.GetEvent<LayoutCompleteEvent>().Publish(model);
        }

        public void BeginModelBatch()
        {

            batchModel = new BatchModel(model);
            batchModel.BeginBatch();
            model = batchModel;
            //model.BeginBatch();
        }

        public void EndModelBatch()
        {
            batchModel.EndBatch();
            batchModel = null;
            model = internalModel;
            //model.EndBatch();
        }

        public void ToggleGridLines()
        {
            //if (!diagramItems.Contains(grid))
            //    diagramItems.Insert(0, grid);
            //else
            //    diagramItems.Remove(grid);
        }

        public override void ViewLoaded()
        {
            //var tmp = IsGrayed;
            //IsGrayed = false;
            //UpdateThumbnail();
            //IsGrayed = tmp;
            
            //eventService.GetEvent<LayoutCompleteEvent>().Publish(this.model);
        }

        private ContentControl view;

        public override ContentControl View
        {
            get { return view; }
            set { view = value; SendPropertyChanged("View"); }
        }

        private Image thumbnail;

        public Image Thumbnail
        {
            get { if (thumbnail == null) thumbnail = LoadImage(); return thumbnail; }
            set { thumbnail = value; SendPropertyChanged("Thumbnail"); }
        }

        private Image LoadImage()
        {
            if (model.Thumbnail != null && model.Thumbnail.Image != null)
            {
                var stream = new MemoryStream(model.Thumbnail.Image);
                var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                var img = new Image { Source = decoder.Frames[0], SnapsToDevicePixels = true };
                return img;
            }
            return null;
        }

        private Image GetBitmap()
        {
            //var renderBitmap = new RenderTargetBitmap(96, 64, 1 / 300, 1 / 300, PixelFormats.Pbgra32);
            //var visual = new DrawingVisual();
            //using (var context = visual.RenderOpen())
            //{
            //  var brush = new VisualBrush(View);
            //  context.DrawRectangle(brush, null, new Rect(new Point(), new Size(View.ActualWidth, View.ActualHeight)));
            //}
            //visual.Transform = new ScaleTransform(96 / View.ActualWidth, 64 / View.ActualHeight);
            //renderBitmap.Render(visual);

            //return new Image { Source = renderBitmap };
            return new Image { Source = CaptureScreen(View, 96, 96) };
        }

        private static BitmapSource CaptureScreen(Visual target, double dpiX, double dpiY)
        {
            if (target == null)
            {
                return null;
            }

            var bounds = VisualTreeHelper.GetDescendantBounds(target);

            var rtb = new RenderTargetBitmap((int)(bounds.Width * dpiX / 96.0),
                                                            (int)(bounds.Height * dpiY / 96.0),
                                                            dpiX,
                                                            dpiY,
                                                            PixelFormats.Pbgra32);

            var dv = new DrawingVisual();
            using (var ctx = dv.RenderOpen())
            {
                var vb = new VisualBrush(target);
                ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            }

            rtb.Render(dv);

            return rtb;
        }


        public DiagramCanvas DiagramCanvas
        {
            get; set;
            //get
            //{
            //    if (View == null) return null;
            //    return ((ModelView) View).diagramViewer.diagramView.DiagramCanvas;
            //}
        }
    }

    public class LayoutType
    {
        public string Name { get; set; }
        public string Id { get; set; }
    }
}
