namespace SMART.Gui.Controls.DiagramControl.Services
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using System.Linq;

    using Commands;

    public class UndoService : IUndoService
    {
        public Stack<IDiagramCommand> UndoCommands { get; set; }

        public Stack<IDiagramCommand> RedoCommands { get; set; }

        public List<string> UndoTitles
        {
            get
            {
                return (from u in UndoCommands select u.Title).ToList();
            }
        }

        public bool CanUndo { get { return UndoCommands.Count > 0; } }

        public bool CanRedo { get { return RedoCommands.Count > 0; } }

        public void OnExecuteUndo(object sender, ExecutedRoutedEventArgs e)
        {
            Undo();
        }

        public void OnCanExecuteUndo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanUndo;
        }

        public void OnExecuteRedo(object sender, ExecutedRoutedEventArgs e)
        {
            Redo();
        }

        public void OnCanExecuteRedo(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = CanRedo;
        }

        public UndoService()
        {
            UndoCommands = new Stack<IDiagramCommand>();
            
            //UndoTitles = new ObservableCollection<string>();
            RedoCommands = new Stack<IDiagramCommand>();
            //RedoTitles = new ObservableCollection<string>();
        }

        public void Execute(IDiagramCommand command)
        {
            if (command == null) return;
            // Execute command
            command.Execute();
            // Push command to undo history

            UndoCommands.Push(command);
            //UndoTitles.Insert(0, command.Title);
            // Clear the redo history upon adding new undo entry. This is a typical logic for most applications
            RedoCommands.Clear();
            //RedoTitles.Clear();            
        }

        public void Undo()
        {
            if (CanUndo)
            {
                var command = UndoCommands.Pop();
                command.Undo();
                //UndoTitles.RemoveAt(0);
                RedoCommands.Push(command);
                //RedoTitles.Insert(0, command.Title);
            }
        }

        public void Redo()
        {
            if (CanRedo)
            {
                var command = RedoCommands.Pop();
                //RedoTitles.RemoveAt(0);
                //Execute(command);
                if (command != null)
                {
                    command.Redo();

                    UndoCommands.Push(command);
                    //UndoTitles.Insert(0, command.Title);
                }
            }
        }

        public void ClearUndoHistory()
        {
            UndoCommands.Clear();
            //UndoTitles.Clear();
        }

        public void ClearRedoHistory()
        {
            RedoCommands.Clear();
            //RedoTitles.Clear();
        }

        public void ClearHistory()
        {
            ClearRedoHistory();
            ClearUndoHistory();
        }
    }
}
