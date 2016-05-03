using System;
using System.Windows.Markup;

namespace SMART.Gui.Converters
{
    public class ObjectToBoolBindingExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new ObjectToBoolConvertor();
        }
    }
}