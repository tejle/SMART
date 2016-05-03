using System.Linq;
using System.Text;

namespace SMART.Gui.Converters
{
    using System.Collections;
    using System.Windows;
    using Syncfusion.Windows.Diagram;

    public class SmartDiagramModel : DiagramModel
    {

        public SmartDiagramModel() : base()
        {
           

        }
        
        /// <summary>
        /// IsSwanky Dependency Property
        /// </summary>
        public static readonly DependencyProperty MyNodesProperty = DependencyProperty.Register("MyNodes", 
                typeof(IEnumerable), typeof(SmartDiagramModel), new UIPropertyMetadata(new PropertyChangedCallback(SmartDiagramModel.OnItemSourceChanged)));


        
        public IEnumerable MyNodes
        {
            get { return GetValue(MyNodesProperty) as IEnumerable; }
            set
            {
                Nodes.SourceCollection = value;
                SetValue(MyNodesProperty, value);                                
            }
        }

        public IEnumerable MyConnections { get { return Connections.SourceCollection; } set { Connections.SourceCollection = value; } }

        private static void OnItemSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SmartDiagramModel model = d as SmartDiagramModel;
            if (e.NewValue != e.OldValue)
            {
                model.MyNodes = e.NewValue as IEnumerable;
//                model.Nodes.SourceCollection = e.NewValue as IEnumerable;
            }

        }
    }
}
