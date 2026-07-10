using System;
using System.Windows;
using System.Windows.Input;
using SMART.Core.Interfaces.Services;
using SMART.Gui.Commands;
using SMART.Gui.Controls;
using SMART.Gui.ViewModel;

namespace SMART.Gui.View
{
    public partial class MainView
    {
        private ApplicationViewModel ViewModel { get; set; }
 
        public MainView(ApplicationViewModel viewModel)
        {
            DataContext = ViewModel = viewModel;
            InitializeComponent();
            
            RoutedActionCommand.Bind(this);
            Loaded += MainViewLoaded;
        }

        void MainViewLoaded(object sender, RoutedEventArgs e)
        {
            // Handle TitleBar button commands (min/restore/close)
            CommandBindings.Add(new CommandBinding(SmartCommands.Minimize, OnCommand, OnCanCommand));
            CommandBindings.Add(new CommandBinding(SmartCommands.Maximize, OnCommand, OnCanCommand));
            CommandBindings.Add(new CommandBinding(ApplicationCommands.Close, OnCommand, OnCanCommand));
        }

        private static void OnCanCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void OnCommand(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Command == SmartCommands.Minimize)
                WindowState = WindowState.Minimized;

            else if (e.Command == SmartCommands.Maximize)
            {
                WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }

            else if (e.Command == ApplicationCommands.Close)
                Close();
        }
    }
}
