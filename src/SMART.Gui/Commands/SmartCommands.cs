namespace SMART.Gui.Commands
{
    using System.Windows.Input;

    public static class SmartCommands
    {
        private static readonly LayoutCommand layoutCommand = new LayoutCommand();
        private static readonly ToggleGridLinesCommand toggleGridLinesCommand = new ToggleGridLinesCommand();
        private static readonly UndoCommand undoCommand = new UndoCommand();
        private static readonly MinimizeCommand minimizeCommand = new MinimizeCommand();
        private static readonly MaximizeCommand maximizeCommand = new MaximizeCommand();
        private static readonly DeleteModelCommand deleteModelCommand = new DeleteModelCommand();

        public static RoutedUICommand LayoutCommand { get { return layoutCommand; } }
        public static RoutedUICommand ToggleGridLines { get { return toggleGridLinesCommand; } }
        public static RoutedActionCommand UndoCommand { get { return undoCommand; } }
        public static RoutedActionCommand Minimize { get { return minimizeCommand; } }
        public static RoutedActionCommand Maximize { get { return maximizeCommand; } }
        public static RoutedUICommand DeleteModel { get { return deleteModelCommand; } }

    }
}