namespace SMART.Gui.Controls.DiagramControl.Services.Commands
{
    using System.Linq;
    using System.Windows;

    using Core.DomainModel;

    using Interfaces;

    using Shapes;

    using ViewModel;

    public class PasteStateCommand : IDiagramCommand
    {
        private const string DefaultCommandTitle = "Paste State";

        public IDiagramViewModel ViewModel { get; set; }
    
        public Point Location { get; set; }
        public State TheState { get; set; }
        public string Title { get; set; }

        public PasteStateCommand(IDiagramViewModel viewModel, State state) : this(viewModel, state, null) { }

        public PasteStateCommand(IDiagramViewModel viewModel, State state, string title)
        {
            this.ViewModel = viewModel;
            TheState = state;
            Location = new Point(state.Location.X, state.Location.Y);
            this.Title = (!string.IsNullOrEmpty(title)) ? title : DefaultCommandTitle;
        }


        public void Execute()
        {
            this.ViewModel.AddStateToDomainModel(this.Location, this.TheState);            
        }

        public void Undo()
        {
            var state =
                    (from s in this.ViewModel.DiagramItems
                     where s is StateViewModel && (s as StateViewModel).State == this.TheState
                     select s).FirstOrDefault();
            if (state != null)
            {
                this.ViewModel.RemoveElementFromDomainModel(state as ISelectable);
            }
        }

        public void Redo()
        {
            this.ViewModel.AddStateToDomainModel(this.Location, this.TheState);
        }

    }
}