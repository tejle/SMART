namespace SMART.Gui.Controls.DiagramControl.Services.Commands
{
    using Core.DomainModel;
    using System.Linq;

    using Interfaces;

    using Shapes;

    using ViewModel;

    public class AddTransitionCommand : IDiagramCommand
    {
        private const string DefaultCommandTitle = "Add State";

        public IDiagramViewModel ViewModel { get; set; }
        public IConnectable Source { get; set; }
        public IConnectable Destination { get; set; }
    
        public Transition TheTransition { get; set; }
        public string Title { get; set; }

        public AddTransitionCommand(IDiagramViewModel viewModel, IConnectable source, IConnectable destination) : this(viewModel, source, destination, null) { }

        public AddTransitionCommand(IDiagramViewModel viewModel, IConnectable source, IConnectable destination, string title)
        {
            this.ViewModel = viewModel;
            this.Source = source;
            this.Destination = destination;
            this.Title = (!string.IsNullOrEmpty(title)) ? title : DefaultCommandTitle;
        }


        public void Execute()
        {
            this.TheTransition = this.ViewModel.AddTransitionToDomainModel(this.Source, this.Destination);
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
            this.ViewModel.AddTransitionToDomainModel(this.Source, this.Destination, this.TheTransition);      
        }

    }
}