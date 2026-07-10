using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml.Linq;
using SMART.Core;
using SMART.Core.DomainModel;
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Services;
using SMART.Gui.Commands;
using SMART.Gui.Controls.DiagramControl.Services;
using SMART.Gui.Controls.DiagramControl.Services.Commands;
using SMART.Gui.Events;
using SMART.Gui.Interfaces;
using SMART.Gui.ViewModel;
using SMART.IOC;

namespace SMART.Gui.Controls.DiagramControl.View
{
    public class DiagramCanvas : Canvas, IExtensibleObject<DiagramCanvas>
    {
        #region EditorMode enum

        public enum EditorMode
        {
            PanAndZoom,
            CreateStatesAndTransitions,
            ZoomRect,
            NearestNeighbour
        }

        #endregion

        #region GridType enum

        public enum GridType
        {
            DotGrid,
            LineGrid,
            None
        } ;

        #endregion

        private CommandBinding copyCmdBinding;
        private CommandBinding cutCmdBinding;
        private CommandBinding pasteCmdBinding;
        private CommandBinding toggleGridLinesBinding;
        private CommandBinding redoCmdBinding;
        private CommandBinding undoCmdBinding;

        private DoubleAnimation scaleXAnim;
        private DoubleAnimation scaleYAnim;
        private DoubleAnimation transXAnim;
        private DoubleAnimation transYAnim;

        public ISelectionService SelectionService;
        private IEventService eventService;

        private bool isActive;

        public TransformGroup TransformView;
        public ScaleTransform ScaleView;
        public TranslateTransform TranslateView;

        private Storyboard zoomStoryboard;

        public DiagramCanvas()
        {
            Initialized += DiagramCanvas_Initialized;
            Loaded += DiagramCanvas_Loaded;
            Unloaded += DiagramCanvas_Unloaded;
        }

        public bool IsCreatingTransition { get; set; }

        public EditorMode EditMode { get; set; }

        public EditorMode PrevEditMode { get; set; }

        public DiagramView DiagramViewControl
        {
            get { return ItemsControl.GetItemsOwner(this) as DiagramView; }
        }

        public IDiagramViewModel ViewModel
        {
            get { return DiagramViewControl.DataContext as IDiagramViewModel; }
        }

        public IUndoService UndoService { get; set; }

        public int StateCount
        {
            get { return Items.Count; }
        }

        public bool IsDragEnabled { get; set; }
        public bool IsDragging { get; set; }

        #region IExtensibleObject<DiagramCanvas> Members

        public IExtensionCollection<DiagramCanvas> Extensions { get; private set; }

        #endregion

        public ObservableCollection<IDiagramItem> Items
        {
            get { return (DiagramViewControl.ItemsSource as ObservableCollection<IDiagramItem>); }
        }

        private void DiagramCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            //if (!isActive)
            {
                //isActive = true;
                UpdateLayout();
                ZoomToContent(true);
                DiagramViewControl.Focus();
            }
        }

        private void DiagramCanvas_Unloaded(object sender, RoutedEventArgs e)
        {
            if (isActive)
            {
                isActive = false;
                Deactivate();
            }
        }

        public void Activate()
        {
            AddCommandBinding(undoCmdBinding);
            AddCommandBinding(redoCmdBinding);
            AddCommandBinding(cutCmdBinding);
            AddCommandBinding(copyCmdBinding);
            AddCommandBinding(pasteCmdBinding);
            AddCommandBinding(toggleGridLinesBinding);

            ViewModel.DiagramCanvas = this;

            TranslateView.X = ViewModel.CanvasInfo.TranslateViewX;
            TranslateView.Y = ViewModel.CanvasInfo.TranslateViewY;

            ScaleView.ScaleX = ViewModel.CanvasInfo.ScaleViewX;
            ScaleView.ScaleY = ViewModel.CanvasInfo.ScaleViewY;

            DiagramViewControl.Focus();
        }

        public void Deactivate()
        {
            RemoveCommandBinding(undoCmdBinding);
            RemoveCommandBinding(redoCmdBinding);
            RemoveCommandBinding(cutCmdBinding);
            RemoveCommandBinding(copyCmdBinding);
            RemoveCommandBinding(pasteCmdBinding);
            RemoveCommandBinding(toggleGridLinesBinding);
        }

        private static void AddCommandBinding(CommandBinding commandBinding)
        {
            if (!Application.Current.MainWindow.CommandBindings.Contains(commandBinding))
            {
                Application.Current.MainWindow.CommandBindings.Add(commandBinding);
            }
        }

        private static void RemoveCommandBinding(CommandBinding commandBinding)
        {
            if (Application.Current.MainWindow == null) return;

            if (Application.Current.MainWindow.CommandBindings.Contains(commandBinding))
            {
                Application.Current.MainWindow.CommandBindings.Remove(commandBinding);
            }
        }

        private void DiagramCanvas_Initialized(object sender, EventArgs e)
        {
            EditMode = EditorMode.CreateStatesAndTransitions;
            PrevEditMode = EditorMode.CreateStatesAndTransitions;

            TransformView = new TransformGroup();
            TranslateView = new TranslateTransform();
            ScaleView = new ScaleTransform();
            RenderTransform = TransformView;

            TransformView.Children.Add(TranslateView);
            TransformView.Children.Add(ScaleView);
            NameScope.SetNameScope(this, new NameScope());

            RegisterName("ScaleView", ScaleView);
            RegisterName("transView", TranslateView);

            ScaleView.ScaleX = 1.0d;
            ScaleView.ScaleY = 1.0d;

            TranslateView.X = 0.0d;
            TranslateView.Y = 0.0d;

            SelectionService = new SelectionService(this);
            Resolver.RegisterInstance<ISelectionService>(SelectionService);

            eventService = Resolver.Resolve<IEventService>();
            eventService.GetEvent<ActivateModelEvent>().Subscribe(OnActivateModel);
            eventService.GetEvent<DeactivateModelEvent>().Subscribe(OnDeactivateModel);

            UndoService = new UndoService();

            IsItemsHost = true;
            Background = Brushes.Transparent;
            IsDragEnabled = true;

            Extensions = new ExtensionCollection<DiagramCanvas>(this)
                             {
                                 new PanAndZoomExtension(),
                                 new NodeCreatorExtension(),
                                 new SingleSelectionExtension(),
                                 new DragSelectionExtension(),
                                 new RubberbandSelectionExtension(),
                                 new ConnectionExtension(),
                                 new KeyboardInputExtension(),
                                 new NearestNeighbourExtension()
                             };

            CreateCommandBindings();
        }

        private void OnDeactivateModel(IModel model)
        {
            if (isActive && ViewModel != null && ViewModel.Id == model.Id)
            {
                isActive = false;
                Deactivate();
            }
        }

        private void OnActivateModel(IModel model)
        {
            if (!isActive && ViewModel != null && ViewModel.Id == model.Id)
            {
                if (ViewModel.IsFirstRun)
                {
                    UpdateLayout();
                    ZoomToContent(true);
                    //DiagramViewControl.Focus();
                    ViewModel.IsFirstRun = false;
                }
                isActive = true;
                Activate();
            }
        }

        private void CreateCommandBindings()
        {
            undoCmdBinding = new CommandBinding(ApplicationCommands.Undo, UndoCmdExecuted, UndoCmdCanExecute);
            redoCmdBinding = new CommandBinding(ApplicationCommands.Redo, RedoCmdExecuted, RedoCmdCanExecute);
            cutCmdBinding = new CommandBinding(ApplicationCommands.Cut, CutCmdExecuted, CutCmdCanExecute);
            copyCmdBinding = new CommandBinding(ApplicationCommands.Copy, CopyCmdExecuted, CopyCmdCanExecute);
            pasteCmdBinding = new CommandBinding(ApplicationCommands.Paste, PasteCmdExecuted, PasteCmdCanExecute);
            toggleGridLinesBinding = new CommandBinding(SmartCommands.ToggleGridLines, OnToggleGridLines,
                                                        OnCanToggleGridLines);
        }

        public void Pan(string direction)
        {
            switch (direction)
            {
                case "Up":
                    AnimatePan(TranslateView.X, TranslateView.Y + 10*ScaleView.ScaleY, 250);
                    break;
                case "Down":
                    AnimatePan(TranslateView.X, TranslateView.Y - 10*ScaleView.ScaleY, 250);
                    break;
                case "Left":
                    AnimatePan(TranslateView.X + 10*ScaleView.ScaleX, TranslateView.Y, 250);
                    break;
                case "Right":
                    AnimatePan(TranslateView.X - 10*ScaleView.ScaleX, TranslateView.Y, 250);
                    break;
            }
        }

        public void Zoom(string action)
        {
            switch (action)
            {
                case "All":
                    ZoomToContent(false);
                    break;
                case "Out":
                    AnimateZoom(ScaleView.ScaleX - 0.25, TranslateView.X, TranslateView.Y, 250);
                    break;
                case "In":
                    AnimateZoom(ScaleView.ScaleX + 0.25, TranslateView.X, TranslateView.Y, 250);
                    break;
                case "Rect":
                    PrevEditMode = EditMode;
                    EditMode = EditorMode.ZoomRect;
                    break;
            }
        }

        public void SetEditMode(EditorMode mode)
        {
            PrevEditMode = EditMode;
            EditMode = mode;
        }

        private static void PasteCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Xaml);
        }

        private void PasteCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            PasteClipboard();
        }

        private void CopyCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.SelectionCount > 0;
        }

        private void CopyCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
        }

        private void CutCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = SelectionService.SelectionCount > 0;
        }

        private void CutCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            CopyCurrentSelection();
            var command = new DiagramCommandGroup<RemoveElementCommand>(ViewModel, "Cut selection");
            ((List<ISelectable>) SelectionService.GetSelectedComponents()).ForEach(
                s => command.Add(new RemoveElementCommand(ViewModel, s)));
            ExecuteCommand(command);
            SelectionService.SetSelectedComponents(null);
        }

        private static void OnCanToggleGridLines(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnToggleGridLines(object sender, ExecutedRoutedEventArgs e)
        {
            ViewModel.ToggleGridLines();
        }

        private void UndoCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (UndoService != null && UndoService.CanUndo);
            e.Handled = true;
        }

        private void RedoCmdCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = (UndoService != null && UndoService.CanRedo);
            e.Handled = true;
        }

        private void UndoCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (UndoService != null && UndoService.CanUndo)
            {
                UndoService.Undo();
                e.Handled = true;
            }
        }

        private void RedoCmdExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            if (UndoService != null && UndoService.CanRedo)
            {
                UndoService.Redo();
                e.Handled = true;
            }
        }

        private void ExecuteCommand(IDiagramCommand command)
        {
            if (UndoService != null)
                UndoService.Execute(command);
            else
                command.Execute();
        }

        public void EditLabel(ShapeBaseViewModel item)
        {
            var command = new EditLabelCommand(item);
            ExecuteCommand(command);
            DiagramViewControl.Focus();
        }

        public void AddState(Point location)
        {
            var command = new AddStateCommand(ViewModel, location);
            ExecuteCommand(command);
        }

        public void AddTransition(IConnectable source, IConnectable destination)
        {
            var command = new AddTransitionCommand(ViewModel, source, destination);
            ExecuteCommand(command);
        }

        public void RemoveElement(ISelectable element)
        {
            var command = new RemoveElementCommand(ViewModel, element);
            ExecuteCommand(command);
        }

        public void RemoveManyElements(List<ISelectable> elements)
        {
            var command = new DiagramCommandGroup<RemoveElementCommand>(ViewModel, "Remove many elements");
            foreach (var element in elements)
            {
                if (element is StateViewModel)
                {
                    var state = ((StateViewModel) element).State;
                    if (state is StartState || state is StopState)
                        continue;
                }
                command.Add(new RemoveElementCommand(ViewModel, element));
            }
            //elements.ForEach(e => command.Add(new RemoveElementCommand(ViewModel, e)));
            ExecuteCommand(command);
        }

        public void UpdateStatePosition(IDraggable state, Point originalPosition, Point currentPosition)
        {
            var command = new MoveStateCommand(state as IConnectable, originalPosition, currentPosition);
            ExecuteCommand(command);
        }

        public void UpdateManyStatesPosition(List<DragSelectionExtension.DragItemHolder> states)
        {
            var command = new DiagramCommandGroup<MoveStateCommand>(ViewModel, "Move many states");
            states.ForEach(s => command.Add(new MoveStateCommand(s.Item, s.OriginalPosition, s.CurrentPosition)));
            ExecuteCommand(command);
        }

        public void ChangeSize(StateViewModel shape, Size originalSize, Size currentSize)
        {
            var command = new ChangeSizeCommand(shape, originalSize, currentSize);
            ExecuteCommand(command);
        }

        public void UpdateTransitionSource(TransitionViewModel transistion, IConnectable newSource)
        {
            var command = new ChangeTransitionSourceCommand(ViewModel, transistion, newSource);
            ExecuteCommand(command);
        }

        public void UpdateTransitionDestination(TransitionViewModel transistion, IConnectable newDestination)
        {
            var command = new ChangeTransitionDestinationCommand(ViewModel, transistion, newDestination);
            ExecuteCommand(command);
        }

        public void AddElement<T>(T element)
        {
            var uiElement = element as IDiagramItem;
            if (uiElement != null && !Items.Contains(uiElement))
            {
                Items.Add(uiElement);
            }
        }

        public void InsertComponent<T>(int position, T component) where T : IDiagramItem
        {
            InsertElement(position, component);
        }

        public void InsertElement<T>(int index, T element)
        {
            var uiElement = element as IDiagramItem;
            if (uiElement != null && !Items.Contains(uiElement))
            {
                Items.Insert(index, uiElement);
            }
        }

        public void RemoveElementFromDesigner<T>(T element)
        {
            var uiElement = element as IDiagramItem;

            if (uiElement != null && Items.Contains(uiElement))
            {
                Items.Remove(uiElement);
            }
        }

        public void ZoomToContent(bool skipZoomIfAlreadyInsideBounds)
        {
            if (Items == null)
                return;
            IConnectable firstItem = Items.OfType<IConnectable>().FirstOrDefault();
            if (firstItem == null)
                return;

            Rect boundingrect = GetBoundingRect(firstItem);
            double left = boundingrect.Left;
            double right = boundingrect.Right;
            double top = boundingrect.Bottom;
            double bottom = boundingrect.Top;
            Rect tempRect;

            foreach (IConnectable element in Items.OfType<IConnectable>())
            {
                tempRect = GetBoundingRect(element);

                if (left > tempRect.Left)
                    left = tempRect.Left;

                if (right < tempRect.Right)
                    right = tempRect.Right;

                if (top < tempRect.Bottom)
                    top = tempRect.Bottom;

                if (bottom > tempRect.Top)
                    bottom = tempRect.Top;
            }

            var zoomRect = new Rect(left, bottom, right - left, top - bottom);
            zoomRect.Inflate(25, 25);

            //if ((skipZoomIfAlreadyInsideBounds && !IsInsideBounds(zoomRect)) || !skipZoomIfAlreadyInsideBounds)
            {
                ZoomRect(zoomRect, skipZoomIfAlreadyInsideBounds);
            }
        }

        //private bool IsInsideBounds(Rect zoomRect)
        //{
        //    var controlBounds = GetBoundingBox(this, DiagramViewControl);
        //    if (zoomRect.Left > controlBounds.Left && zoomRect.Right < controlBounds.Right &&
        //        zoomRect.Top < controlBounds.Top && zoomRect.Bottom < controlBounds.Bottom)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;    
        //    }            
        //}

        public void ZoomRect(Rect zoomRect, bool skipZoomIfAlreadyInsideBounds)
        {
            if (zoomRect.Width > 0.0d && zoomRect.Height > 0.0d)
            {
                // calculate differences 
                double dx = (DiagramViewControl.ActualWidth - zoomRect.Width);
                double dy = (DiagramViewControl.ActualHeight - zoomRect.Height);

                // calculate new scale for zoom rectangle
                double scale;
                if (dx/DiagramViewControl.ActualWidth < dy/DiagramViewControl.ActualHeight)
                    scale = DiagramViewControl.ActualWidth/zoomRect.Width;
                else
                    scale = DiagramViewControl.ActualHeight/zoomRect.Height;

                if (skipZoomIfAlreadyInsideBounds && scale > 1)
                {
                    AnimatePan(-zoomRect.X + dx/2, -zoomRect.Y + dy/2, 500);
                }
                else
                {
                    AnimateZoom(scale, -zoomRect.X + dx/2, -zoomRect.Y + dy/2, 500);
                }
            }
        }

        public void AnimatePan(double panX, double panY, double time)
        {
            transXAnim = new DoubleAnimation(TranslateView.X, panX, new Duration(TimeSpan.FromMilliseconds(time)),
                                             FillBehavior.HoldEnd);
            transYAnim = new DoubleAnimation(TranslateView.Y, panY, new Duration(TimeSpan.FromMilliseconds(time)),
                                             FillBehavior.HoldEnd);

            if (zoomStoryboard != null)
            {
                zoomStoryboard.Completed -= zoomStoryboard_PanCompleted;
            }
            zoomStoryboard = new Storyboard();
            zoomStoryboard.Completed += zoomStoryboard_PanCompleted;
            zoomStoryboard.Children.Add(transXAnim);
            zoomStoryboard.Children.Add(transYAnim);

            Storyboard.SetTargetName(transXAnim, "transView");
            Storyboard.SetTargetName(transYAnim, "transView");

            Storyboard.SetTargetProperty(transXAnim, new PropertyPath(TranslateTransform.XProperty));
            Storyboard.SetTargetProperty(transYAnim, new PropertyPath(TranslateTransform.YProperty));

            zoomStoryboard.Begin(this);
        }

        public void AnimateZoom(double scale, double panX, double panY, double time)
        {
            if (scale < 0) scale = 0;

            ScaleView.CenterX = DiagramViewControl.ActualWidth/2;
            ScaleView.CenterY = DiagramViewControl.ActualHeight/2;

            scaleXAnim = new DoubleAnimation(ScaleView.ScaleX, scale, new Duration(TimeSpan.FromMilliseconds(time)),
                                             FillBehavior.HoldEnd);
            scaleYAnim = new DoubleAnimation(ScaleView.ScaleY, scale, new Duration(TimeSpan.FromMilliseconds(time)),
                                             FillBehavior.HoldEnd);
            transXAnim = new DoubleAnimation(TranslateView.X, panX, new Duration(TimeSpan.FromMilliseconds(time)),
                                             FillBehavior.HoldEnd);
            transYAnim = new DoubleAnimation(TranslateView.Y, panY, new Duration(TimeSpan.FromMilliseconds(time)),
                                             FillBehavior.HoldEnd);

            if (zoomStoryboard != null)
            {
                zoomStoryboard.Completed -= zoomStoryboard_Completed;
            }
            zoomStoryboard = new Storyboard();
            zoomStoryboard.Completed += zoomStoryboard_Completed;
            zoomStoryboard.Children.Add(scaleXAnim);
            zoomStoryboard.Children.Add(scaleYAnim);
            zoomStoryboard.Children.Add(transXAnim);
            zoomStoryboard.Children.Add(transYAnim);

            Storyboard.SetTargetName(scaleXAnim, "ScaleView");
            Storyboard.SetTargetName(scaleYAnim, "ScaleView");
            Storyboard.SetTargetName(transXAnim, "transView");
            Storyboard.SetTargetName(transYAnim, "transView");

            Storyboard.SetTargetProperty(scaleXAnim, new PropertyPath(ScaleTransform.ScaleXProperty));
            Storyboard.SetTargetProperty(scaleYAnim, new PropertyPath(ScaleTransform.ScaleYProperty));
            Storyboard.SetTargetProperty(transXAnim, new PropertyPath(TranslateTransform.XProperty));
            Storyboard.SetTargetProperty(transYAnim, new PropertyPath(TranslateTransform.YProperty));

            zoomStoryboard.Begin(this);
        }

        private void zoomStoryboard_Completed(object sender, EventArgs e)
        {
            TranslateView.X = transXAnim.To.Value;            
            TranslateView.Y = transYAnim.To.Value;
            ScaleView.ScaleX = scaleXAnim.To.Value;
            ScaleView.ScaleY = scaleYAnim.To.Value;

            UpdateViewModel();

            ScaleView.BeginAnimation(ScaleTransform.ScaleXProperty, null);
            ScaleView.BeginAnimation(ScaleTransform.ScaleYProperty, null);
            TranslateView.BeginAnimation(TranslateTransform.XProperty, null);
            TranslateView.BeginAnimation(TranslateTransform.YProperty, null);
            zoomStoryboard.Remove(this);
        }

        private void zoomStoryboard_PanCompleted(object sender, EventArgs e)
        {
            TranslateView.X = transXAnim.To.Value;
            TranslateView.Y = transYAnim.To.Value;

            UpdateViewModel();

            TranslateView.BeginAnimation(TranslateTransform.XProperty, null);
            TranslateView.BeginAnimation(TranslateTransform.YProperty, null);
            zoomStoryboard.Remove(this);
        }

        private Rect GetBoundingRect(IConnectable item)
        {
            return new Rect(item.Left, item.Top, item.Width, item.Height);
        }

        private Rect GetBoundingBox(FrameworkElement element, Visual containerWindow)
        {
            GeneralTransform transform = element.TransformToAncestor(containerWindow);
            Point topLeft = transform.Transform(new Point(0, 0));
            Point bottomRight = transform.Transform(new Point(element.ActualWidth, element.ActualHeight));
            return new Rect(topLeft, bottomRight);
        }

        public Point TranslatePointToView(Point point)
        {
            GeneralTransform transform = TransformToAncestor(DiagramViewControl);
            Point topLeft = transform.Transform(point);
            return topLeft;
            //var bottomRight = transform.Transform(new Point(element.ActualWidth, element.ActualHeight));
            //return new Rect(topLeft, bottomRight);
        }

        private void CopyCurrentSelection()
        {
            IEnumerable<StateViewModel> selectedStates =
                SelectionService.GetSelectedComponents().OfType<StateViewModel>();
            IEnumerable<TransitionViewModel> selectedConnections =
                SelectionService.GetSelectedComponents().OfType<TransitionViewModel>();

            XElement statesXML = SerializeStates(selectedStates);
            XElement transitionsXML = SerializeTransitions(selectedConnections);

            var root = new XElement("Root");
            root.Add(statesXML);
            root.Add(transitionsXML);

            root.Add(new XAttribute("OffsetX", 10));
            root.Add(new XAttribute("OffsetY", 10));

            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        private void PasteClipboard()
        {
            XElement root = LoadSerializedDataFromClipBoard();

            if (root == null)
                return;

            // create States
            var mappingOldToNewIDs = new Dictionary<Guid, Guid>();
            var newStates = new List<State>();
            IEnumerable<XElement> itemsXML = root.Elements("States").Elements("State");

            double offsetX = Double.Parse(root.Attribute("OffsetX").Value, CultureInfo.InvariantCulture);
            double offsetY = Double.Parse(root.Attribute("OffsetY").Value, CultureInfo.InvariantCulture);

            var command = new DiagramCommandGroup<IDiagramCommand>(ViewModel, "Paste selection");

            foreach (XElement itemXML in itemsXML)
            {
                var oldID = new Guid(itemXML.Element("ID").Value);
                Guid newID = Guid.NewGuid();
                mappingOldToNewIDs.Add(oldID, newID);

                State item = DeserializeState(itemXML, newID, offsetX, offsetY);

                command.Add(new PasteStateCommand(ViewModel, item));
                newStates.Add(item);
            }

            // create Transitions
            var newTransitions = new List<Transition>();
            IEnumerable<XElement> connectionsXML = root.Elements("Transitions").Elements("Transition");
            foreach (XElement connectionXML in connectionsXML)
            {
                var oldSourceID = new Guid(connectionXML.Element("SourceID").Value);
                var oldDestinationID = new Guid(connectionXML.Element("DestinationID").Value);

                if (mappingOldToNewIDs.ContainsKey(oldSourceID) && mappingOldToNewIDs.ContainsKey(oldDestinationID))
                {
                    Guid newSourceID = mappingOldToNewIDs[oldSourceID];
                    Guid newDestinationID = mappingOldToNewIDs[oldDestinationID];

                    Guid newID = Guid.NewGuid();
                    Transition item = DeserializeTransition(connectionXML, newID);
                    item.Source = newStates.Where(i => i.Id == newSourceID).FirstOrDefault();
                    item.Destination = newStates.Where(i => i.Id == newDestinationID).FirstOrDefault();
                    command.Add(new PasteTransitionCommand(ViewModel, item));
                    newTransitions.Add(item);
                }
            }

            ExecuteCommand(command);

            // Update selection
            var newSelection = new List<ISelectable>();
            IEnumerable<StateViewModel> newStateShapes =
                ViewModel.DiagramItems.OfType<StateViewModel>().Where(s => newStates.Contains(s.State));
            IEnumerable<TransitionViewModel> newTransitionShapes =
                ViewModel.DiagramItems.OfType<TransitionViewModel>().Where(s => newTransitions.Contains(s.Transition));

            newTransitionShapes.ForEach(newSelection.Add);
            newStateShapes.ForEach(newSelection.Add);

            SelectionService.SetSelectedComponents(newSelection);

            // update paste offset
            root.Attribute("OffsetX").Value = (offsetX + 10).ToString();
            root.Attribute("OffsetY").Value = (offsetY + 10).ToString();
            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Xaml, root);
        }

        private static XElement SerializeStates(IEnumerable<StateViewModel> states)
        {
            var serializedItems = new XElement("States",
                                               from item in states
                                               select new XElement("State",
                                                                   new XElement("Left", item.Left),
                                                                   new XElement("Top", item.Top),
                                                                   new XElement("Width", item.Width),
                                                                   new XElement("Height", item.Height),
                                                                   new XElement("Label", item.Name),
                                                                   new XElement("ID", item.State.Id)
                                                   )
                );

            return serializedItems;
        }

        private static XElement SerializeTransitions(IEnumerable<TransitionViewModel> transitions)
        {
            var serializedItems = new XElement("Transitions",
                                               from item in transitions
                                               select new XElement("Transition",
                                                                   new XElement("Label", item.Name),
                                                                   new XElement("SourceID", item.Transition.Source.Id),
                                                                   new XElement("DestinationID",
                                                                                item.Transition.Destination.Id),
                                                                   new XElement("ID", item.Transition.Id)
                                                   )
                );

            return serializedItems;
        }


        private static State DeserializeState(XContainer itemXML, Guid id, double OffsetX, double OffsetY)
        {
            double width = Double.Parse(itemXML.Element("Width").Value, CultureInfo.InvariantCulture);
            double height = Double.Parse(itemXML.Element("Height").Value, CultureInfo.InvariantCulture);
            double x = Double.Parse(itemXML.Element("Left").Value, CultureInfo.InvariantCulture) + OffsetX;
            double y = Double.Parse(itemXML.Element("Top").Value, CultureInfo.InvariantCulture) + OffsetY;
            string label = itemXML.Element("Label").Value;

            var state = new State
                            {
                                Id = id,
                                Size = new SmartSize(width, height),
                                Location = new SmartPoint(x, y),
                                Label = string.Format("Copy of {0}", label)
                            };
            return state;
        }

        private static Transition DeserializeTransition(XContainer itemXML, Guid id)
        {
            string label = itemXML.Element("Label").Value;

            var transition = new Transition()
                                 {
                                     Id = id,
                                     Label = string.Format("Copy of {0}", label),
                                 };
            return transition;
        }

        private static XElement LoadSerializedDataFromClipBoard()
        {
            if (Clipboard.ContainsData(DataFormats.Xaml))
            {
                var clipboardData = Clipboard.GetData(DataFormats.Xaml) as String;

                if (String.IsNullOrEmpty(clipboardData))
                    return null;
                return XElement.Load(new StringReader(clipboardData));
            }

            return null;
        }

        public void UpdateViewModel()
        {
            ViewModel.CanvasInfo.TranslateViewX = TranslateView.X;
            ViewModel.CanvasInfo.TranslateViewY = TranslateView.Y;

            ViewModel.CanvasInfo.ScaleViewX = ScaleView.ScaleX;
            ViewModel.CanvasInfo.ScaleViewY = ScaleView.ScaleY;
        }
    }
}