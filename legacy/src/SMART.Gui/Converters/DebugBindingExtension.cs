using System;
using System.Windows.Markup;

namespace SMART.Gui.Converters
{
    public class DebugBindingExtension : MarkupExtension
    {
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return new DebugConvertor();
        }
    }
}