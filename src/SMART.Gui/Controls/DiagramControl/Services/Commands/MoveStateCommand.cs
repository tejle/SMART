namespace SMART.Gui.Controls.DiagramControl.Services.Commands
{
    using System.Windows;

    using ViewModel;

    public class MoveStateCommand : IDiagramCommand
    {
        private const string DefaultCommandTitle = "Move State";

        public ModelViewModel ViewModel { get; set; }

        public Point OriginalLocation { get; set; }
        public Point Location { get; set; }
        public IConnectable TheState { get; set; }
        public string Title { get; set; }

        public MoveStateCommand(IConnectable state, Point originalPosition, Point currentPosition) : this(state, originalPosition, currentPosition, null) { }

        public MoveStateCommand(IConnectable state, Point originalPosition, Point currentPosition, string title)
        {
            this.TheState = state;
            this.OriginalLocation = originalPosition;
            this.Location = currentPosition;
            this.Title = (!string.IsNullOrEmpty(title)) ? title : DefaultCommandTitle;
        }


        public void Execute()
        {
            // Do nothing, the state already has its new location            
        }

        public void Undo()
        {
            this.TheState.Top = this.OriginalLocation.Y;
            this.TheState.Left = this.OriginalLocation.X;
        }

        public void Redo()
        {
            this.TheState.Top = this.Location.Y;
            this.TheState.Left = this.Location.X;
        }
    }
}