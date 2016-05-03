
using System.Collections.Generic;
using SMART.Gui.Controls.DiagramControl.View.KeyboardStrategies;

namespace SMART.Gui.Controls.DiagramControl.View
{
    using System.ServiceModel;
    using System.Windows.Input;
    using System;

    public class KeyboardInputExtension : IExtension<DiagramCanvas>
    {
        private DiagramCanvas view;
        private DiagramView itemHost;

        private Dictionary<Key, Action<DiagramCanvas, object, KeyEventArgs>> inputMapping;

        public KeyboardInputExtension()
        {
            inputMapping = new Dictionary<Key, Action<DiagramCanvas, object, KeyEventArgs>>
                               {
                                   {Key.Delete, DeleteStrategy.Execute},
                                   {Key.F2, RenameStrategy.Execute}
                               };
        }

        public void Attach(DiagramCanvas owner)
        {
            view = owner;
            itemHost = view.DiagramViewControl;
            itemHost.PreviewKeyUp += this.itemHost_PreviewKeyUp;
        }

        void itemHost_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (inputMapping.ContainsKey(e.Key))
                inputMapping[e.Key](view, sender, e);
        }

        public void Detach(DiagramCanvas owner)
        {
            itemHost.PreviewKeyUp -= this.itemHost_PreviewKeyUp;
        }
    }
}