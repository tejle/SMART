namespace SMART.Gui.Controls.DiagramControl.Services.Commands
{
    using Shapes;

    using ViewModel;

    public class EditLabelCommand : IDiagramCommand
    {
        private const string DefaultCommandTitle = "Move State";

        public ModelViewModel ViewModel { get; set; }

        public string OriginalLabel { get; set; }
        public string Label { get; set; }
        public ShapeBaseViewModel Item { get; set; }
        public string Title { get; set; }

        public EditLabelCommand(ShapeBaseViewModel item) : this(item, null) { }

        public EditLabelCommand(ShapeBaseViewModel item, string title)
        {
            this.Item = item;
            this.OriginalLabel = item.OldText;
            this.Label = item.Name;
            this.Title = (!string.IsNullOrEmpty(title)) ? title : DefaultCommandTitle;
        }

        public void Execute()
        {
            // Do nothing, the item already have the new label
        }

        public void Undo()
        {
            this.Item.Name = this.OriginalLabel;
        }

        public void Redo()
        {
            this.Item.Name = this.Label;
        }
    }
}