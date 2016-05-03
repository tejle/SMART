namespace SMART.Gui.View
{
  using ViewModel;
  /// <summary>
  /// Interaction logic for ModelView.xaml
  /// </summary>
  public partial class ModelView
  {
    public ModelView()
    {
      InitializeComponent();

      Loaded += new System.Windows.RoutedEventHandler(ModelView_Loaded);
    }

    void ModelView_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
      (DataContext as IViewModel).View = this;

      (DataContext as IViewModel).ViewLoaded();
    }

    public ModelView(ModelViewModel viewModel)
    {
      //DataContext = viewModel;
      //InitializeComponent();

    }

    //protected override void OnInitialized(EventArgs e)
    //{
    //    base.OnInitialized(e);

    //    //foreach (var node in viewModel.Nodes)
    //    //{
    //    //    diagramModel.Nodes.Add(node);
    //    //}

    //    //foreach (var connector in viewModel.Connectors)
    //    //{
    //    //    diagramModel.Connections.Add(connector);
    //    //}

    //    diagramModel.Nodes.SourceCollection = viewModel.Nodes;
    //    diagramModel.Connections.SourceCollection = viewModel.Connectors;

    //    diagramModel.Connections.CollectionChanged += Connections_CollectionChanged;            
    //}

    //private void Connections_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    //{
    //    if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems.Count == 1 && e.NewItems[0] is SmartLineConnector)
    //    {
    //        var newConnector = e.NewItems[0] as SmartLineConnector;
    //        if (newConnector != null)
    //        {
    //            diagramModel.Connections.Remove(newConnector);
    //            viewModel.AddTransition(newConnector.HeadNode, newConnector.TailNode);
    //        }
    //    }
    //}

    //void diagramView_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    //{
    //    if (Keyboard.Modifiers == ModifierKeys.Control)
    //    {                
    //        ((DiagramPage)diagramView.Page).ConnectorType = ConnectorType.Straight;
    //        diagramView.EnableConnection = true;
    //    }
    //    //diagramView.EnableConnection = (diagramView.SelectionList.Count == 1);
    //    //((DiagramPage)diagramView.Page).ConnectorType = ConnectorType.Bezier;
    //    //if (diagramView.EnableConnection)
    //    //    ((DiagramPage)diagramView.Page).ConnectorType = ConnectorType.Straight;

    //    wasSelected = (diagramView.SelectionList.Count > 0);
    //    base.OnPreviewMouseLeftButtonDown(e);
    //}

    //void diagramView_MouseLeftButtonUp(object sender, MouseButtonEventArgs evtArgs)
    //{

    //    if (evtArgs.ClickCount == 1)
    //    {
    //        if (!wasSelected && Keyboard.Modifiers != ModifierKeys.Control)
    //        {
    //            viewModel.AddState(evtArgs.GetPosition(sender as IInputElement));
    //            wasSelected = false;
    //            evtArgs.Handled = true;
    //        }                            
    //    }
    //    else
    //    {
    //        evtArgs.Handled = false;
    //        base.OnMouseLeftButtonUp(evtArgs);                
    //    }
    //}
    //void diagramView_NodeDragEnd(object sender, NodeRoutedEventArgs evtArgs)        
    //{        
    //    viewModel.SetStateLocation(evtArgs.Node as SmartNode, new Point(evtArgs.Node.OffsetX, evtArgs.Node.OffsetY));       
    //}        
    //void diagramView_ConnectorLabelChanged(object sender, LabelConnRoutedEventArgs evtArgs)        
    //{        
    //    viewModel.SetTransitionLabel(evtArgs.Connector as SmartLineConnector, evtArgs.OldLabelValue, evtArgs.NewLabelValue);       
    //}        
    //void diagramView_NodeLabelChanged(object sender, LabelRoutedEventArgs evtArgs)       
    //{       
    //    viewModel.SetStateLabel(evtArgs.Node as SmartNode, evtArgs.OldLabelValue, evtArgs.NewLabelValue);       
    //}        
    //private void diagramView_KeyDown(object sender, KeyEventArgs e)       
    //{      
    //    if (e.Key == Key.F2)      
    //    {        
    //        if (diagramView.SelectionList.Count == 1 && diagramView.SelectionList[0] is ISmartDiagramElement)      
    //        {        
    //            (diagramView.SelectionList[0] as ISmartDiagramElement).SetInEditMode();                      
    //        }     
    //    }      
    //}       
    //private void diagramView_ConnectorDragEnd(object sender, ConnDragEndRoutedEventArgs evtArgs)        
    //{                 
    //}    
  }

}
