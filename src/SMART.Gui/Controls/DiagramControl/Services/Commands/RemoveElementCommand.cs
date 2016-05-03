namespace SMART.Gui.Controls.DiagramControl.Services.Commands
{
    using System.Linq;
    using System.Windows;

    using Core.DomainModel;

    using Interfaces;

    using Shapes;

    using ViewModel;

    public class RemoveElementCommand : IDiagramCommand
    {
        private const string DefaultCommandTitle = "Remove State";

        public IDiagramViewModel ViewModel { get; set; }
        public ISelectable Element { get; set; }
        public string Title { get; set; }

        public RemoveElementCommand(IDiagramViewModel viewModel, ISelectable element) : this(viewModel, element, null) { }

        public RemoveElementCommand(IDiagramViewModel viewModel, ISelectable element, string title)
        {
            this.ViewModel = viewModel;
            this.Element = element;
            this.Title = (!string.IsNullOrEmpty(title)) ? title : DefaultCommandTitle;
        }


        public void Execute()
        {
            this.ViewModel.RemoveElementFromDomainModel(this.Element);
        }

        public void Undo()
        {
            if (this.Element is StateViewModel)
            {
                var theStateShape = this.Element as StateViewModel;
                var location = new Point(theStateShape.Left, theStateShape.Top);
                this.ViewModel.AddStateToDomainModel(location, theStateShape.State);
                foreach (var transition in theStateShape.State.Transitions)
                {
                    this.ViewModel.AddTransitionToDomainModel(
                            this.GetStateShapeFromCollection(transition.Source),
                            this.GetStateShapeFromCollection(transition.Destination),
                            transition);
                }
            }

            if (this.Element is TransitionViewModel)
            {
                var theTransition = this.Element as TransitionViewModel;
                this.ViewModel.AddTransitionToDomainModel(theTransition.Source as StateViewModel, theTransition.Target as StateViewModel, theTransition.Transition);
            }
        }

        public void Redo()
        {
            this.ViewModel.RemoveElementFromDomainModel(this.Element);
        }

        private StateViewModel GetStateShapeFromCollection(State state)
        {
            var theState =
                    (from i in this.ViewModel.DiagramItems where i is StateViewModel && (i as StateViewModel).State == state select i)
                            .FirstOrDefault();
            return theState as StateViewModel;
        }

    }
}