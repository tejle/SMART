namespace SMART.Gui.Controls.DiagramControl.Services.Commands
{
    using Core.DomainModel;
    using System.Linq;

    using Interfaces;

    using Shapes;

    using ViewModel;

    public class PasteTransitionCommand : IDiagramCommand
    {
        private const string DefaultCommandTitle = "Paste Transition";

        public IDiagramViewModel ViewModel { get; set; }    
        public Transition TheTransition { get; set; }
        public string Title { get; set; }

        public PasteTransitionCommand(IDiagramViewModel viewModel, Transition transition) : this(viewModel, transition, null) { }

        public PasteTransitionCommand(IDiagramViewModel viewModel, Transition transition, string title)
        {
            this.ViewModel = viewModel;
            this.TheTransition = transition;
            this.Title = (!string.IsNullOrEmpty(title)) ? title : DefaultCommandTitle;
        }


        public void Execute()
        {
            this.TheTransition = this.ViewModel.AddTransitionToDomainModel(null, null, TheTransition);
        }

        public void Undo()
        {
            var state =
                    (from s in this.ViewModel.DiagramItems
                     where s is TransitionViewModel && (s as TransitionViewModel).Transition == this.TheTransition
                     select s).FirstOrDefault();
            if (state != null)
            {
                this.ViewModel.RemoveElementFromDomainModel(state as ISelectable);
            }
        }

        public void Redo()
        {
            this.TheTransition = this.ViewModel.AddTransitionToDomainModel(null, null, TheTransition);            
        }

    }
}