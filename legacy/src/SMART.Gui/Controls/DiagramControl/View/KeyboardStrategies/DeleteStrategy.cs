using System.Collections.Generic;
using System.Windows.Input;
using SMART.Core.DomainModel;
using SMART.Gui.ViewModel;

namespace SMART.Gui.Controls.DiagramControl.View.KeyboardStrategies
{
    public static class DeleteStrategy
    {
        public static void Execute(DiagramCanvas view, object sender, KeyEventArgs e)
        {
            if (view.IsCreatingTransition) return;
            
            var selectionService = view.SelectionService;// Resolver.Resolve<ISelectionService>();

            var selected = selectionService.GetSelectedComponents() as List<ISelectable>;
            if (selected != null)
            {
                if (selected.Count == 0)
                    return;

                if (selected.Count == 1)
                {                    
                    if (selected[0] is IEditable)
                    {                       
                        if (selected[0] is StateViewModel)
                        {
                            var state = ((StateViewModel) selected[0]).State;
                            if (state is StartState || state is StopState)
                                return;                            
                        }
                        if (((IEditable) selected[0]).IsInEditMode) return;
                    }
                    view.RemoveElement(selected[0]);
                }
                else
                {
                    view.RemoveManyElements(selected);
                }
            }
        }
    }
}
