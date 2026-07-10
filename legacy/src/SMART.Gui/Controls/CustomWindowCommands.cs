using System.Windows.Input;

namespace SMART.Gui.Controls
{
    public class CustomWindowCommands
    {
        public static readonly ICommand MinimizeWindow = new RoutedCommand("MinimizeWindowCommand", typeof(CustomWindowCommands));
        public static readonly ICommand MaximizeWindow = new RoutedCommand("MaximizeWindowCommand", typeof(CustomWindowCommands));
    }
}
