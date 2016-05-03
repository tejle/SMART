namespace SMART.Gui.Controls.DiagramControl.Services.Commands
{
    using Interfaces;

    using Shapes;

    using ViewModel;

    public class ChangeTransitionDestinationCommand : IDiagramCommand
    {
        private const string DefaultCommandTitle = "Change Transition Source";

        public IDiagramViewModel ViewModel { get; set; }

        public TransitionViewModel transition { get; set; }
        public IConnectable OldDestination { get; set; }
        public IConnectable NewDestination { get; set; }
        public string Title { get; set; }

        public ChangeTransitionDestinationCommand(IDiagramViewModel viewModel, TransitionViewModel transition, IConnectable newDestination) :
            this(viewModel, transition, newDestination, null) { }

        public ChangeTransitionDestinationCommand(IDiagramViewModel viewModel, TransitionViewModel transition, IConnectable newDestination, string title)
        {
            this.ViewModel = viewModel;
            this.transition = transition;
            this.OldDestination = transition.Target;
            this.NewDestination = newDestination;
            this.Title = (!string.IsNullOrEmpty(title)) ? title : DefaultCommandTitle;
        }


        public void Execute()
        {            
            ViewModel.UpdateTransitionTarget(transition.Transition, NewDestination);
            transition.Target = NewDestination;
        }

        public void Undo()
        {            
            ViewModel.UpdateTransitionTarget(transition.Transition, OldDestination);
            transition.Target = OldDestination;
        }

        public void Redo()
        {            
            ViewModel.UpdateTransitionTarget(transition.Transition, NewDestination);
            transition.Target = NewDestination;
        }
    }
}