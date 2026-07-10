namespace SMART.Gui.Controls.DiagramControl.Services.Commands
{
    using Shapes;

    using ViewModel;
    using System.Windows;

    public class ChangeSizeCommand : IDiagramCommand
    {
        private const string DefaultCommandTitle = "Move State";

        public ModelViewModel ViewModel { get; set; }

        public Size OriginalSize { get; set; }
        public Size Size { get; set; }
        public StateViewModel Item { get; set; }
        public string Title { get; set; }

        public ChangeSizeCommand(StateViewModel item, Size originalSize, Size currentSize) : this(item, originalSize, currentSize, null) { }

        public ChangeSizeCommand(StateViewModel item, Size originalSize, Size currentSize, string title)
        {
            this.Item = item;
            this.OriginalSize = originalSize;
            this.Size = currentSize;
            this.Title = (!string.IsNullOrEmpty(title)) ? title : DefaultCommandTitle;
        }

        public void Execute()
        {
            // Do nothing, the item already have the new size
        }

        public void Undo()
        {
            this.Item.Width = OriginalSize.Width;
            this.Item.Height = OriginalSize.Height;
        }

        public void Redo()
        {
            this.Item.Width = Size.Width;
            this.Item.Height = Size.Height;
        }
    }
}