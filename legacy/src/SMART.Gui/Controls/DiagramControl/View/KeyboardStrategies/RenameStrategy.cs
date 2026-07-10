using SMART.Core.DomainModel;
using SMART.Gui.ViewModel;

namespace SMART.Gui.Controls.DiagramControl.View.KeyboardStrategies
{
    using System.Windows.Input;

    public static class RenameStrategy
    {
        public static void Execute(DiagramCanvas view, object sender, KeyEventArgs e)
        {
            var selectionService = view.SelectionService;// Resolver.Resolve<ISelectionService>();

            if (selectionService.GetSelectedComponents().Count == 0)
                return;

            var selected = selectionService.GetSelectedComponents();
            foreach (var item in selected)
            {
                if (item is IEditable)
                {
                    if (item is StateViewModel)
                    {
                        var state = ((StateViewModel)item).State;
                        if (state is StartState || state is StopState)
                            return;
                    }
                    ((IEditable)item).IsInEditMode = true;
                }
            }
        }
    }
}