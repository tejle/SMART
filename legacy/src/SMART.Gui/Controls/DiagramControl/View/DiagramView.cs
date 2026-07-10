namespace SMART.Gui.Controls.DiagramControl.View
{
    using Helpers;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class DiagramView : ItemsControl
    {

        public DiagramView()
        {
            var itemsPanelTemplateFactory = new FrameworkElementFactory(typeof(DiagramCanvas));
            ItemsPanel = new ItemsPanelTemplate(itemsPanelTemplateFactory);
            BorderThickness = new Thickness(0);
            FocusVisualStyle = null;
            ClipToBounds = true;

            var itemContainerStyle = new Style();
            itemContainerStyle.Setters.Add(new Setter(Canvas.LeftProperty, new Binding("Left")));
            itemContainerStyle.Setters.Add(new Setter(Canvas.TopProperty, new Binding("Top")));
            ItemContainerStyle = itemContainerStyle;
        }

        public DiagramCanvas DiagramCanvas
        {
            get
            {
                return VisualTreeHelperEx.GetChild<DiagramCanvas>(this);
            }
        }
        
    }
}