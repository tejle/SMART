namespace SMART.Gui.Converters
{
    using System.Windows;
    using System.Windows.Controls;

    using Core.Metadata;

    using ViewModel;

    public class ConfigValueTemplateSelector : DataTemplateSelector
    {        
        public DataTemplate AssemblyTemplate { get; set; }
        public DataTemplate ClassTemplate { get; set; }
        public DataTemplate TextTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is ConfigSettingViewModel)
            {
                var cs = item as ConfigSettingViewModel;
                switch (cs.Editor)
                {
                    case ConfigEditor.Assembly:
                        return AssemblyTemplate;
                    case ConfigEditor.Class:
                        return ClassTemplate;
                    case ConfigEditor.Text:
                        return TextTemplate;
                    default:
                        return TextTemplate;
                }                
            }

            return base.SelectTemplate(item, container);
        }
    }
}
