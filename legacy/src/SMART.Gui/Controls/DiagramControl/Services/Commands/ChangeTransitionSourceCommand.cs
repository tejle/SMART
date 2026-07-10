namespace SMART.Gui.Controls.DiagramControl.Services.Commands
{
    using Interfaces;

    using ViewModel;
    using SMART.Gui.Controls.DiagramControl.Shapes;

    public class ChangeTransitionSourceCommand : IDiagramCommand
    {
        private const string DefaultCommandTitle = "Change Transition Source";

        public IDiagramViewModel ViewModel { get; set; }

        public TransitionViewModel transition { get; set; }
        public IConnectable OldSource { get; set; }
        public IConnectable NewSource { get; set; }
        public string Title { get; set; }

        public ChangeTransitionSourceCommand(IDiagramViewModel viewModel, TransitionViewModel transition, IConnectable newSource) : 
            this(viewModel, transition, newSource, null) { }

        public ChangeTransitionSourceCommand(IDiagramViewModel viewModel, TransitionViewModel transition, IConnectable newSource, string title)
        {
            this.ViewModel = viewModel;
            this.transition = transition;
            this.OldSource = transition.Source;
            this.NewSource = newSource;
            this.Title = (!string.IsNullOrEmpty(title)) ? title : DefaultCommandTitle;
        }


        public void Execute()
        {
            ViewModel.UpdateTransitionSource(transition.Transition, NewSource);
        }

        public void Undo()
        {            
            ViewModel.UpdateTransitionSource(transition.Transition, OldSource);
            transition.Source = OldSource;
        }

        public void Redo()
        {            
            ViewModel.UpdateTransitionSource(transition.Transition, NewSource);
            transition.Source = NewSource;
        }
    }
}