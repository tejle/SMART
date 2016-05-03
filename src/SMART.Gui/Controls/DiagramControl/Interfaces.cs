namespace SMART.Gui.Controls.DiagramControl
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;

    using Core.DomainModel;

    using Services.Commands;

    public interface IConnectable
    {
        bool CanConnect { get; }
        Point Location { get; }
        double Left { get; set; }
        double Top { get; set; }
        double Width { get; set; }
        double Height { get; set; }
        State State { get; set; }
        event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>
    /// Provides the drag and drop support for diagram objects
    /// </summary>
    public interface IDraggable
    {
        bool CanDrag { get; }
        bool IsDragging { get; set; }
    }

    public interface ISelectable
    {
        bool Selected { get; set; }
        void Select();
        void Unselect();
        bool IsDimmed { get; set; }
        bool IsCurrent { get; set; }
        int VisitCount { get; set; }
    }

    public interface IEditable
    {
        bool IsInEditMode { get; set; }
        string OldText { get; set; }
    }

    public interface IConnection
    {
        Point StartPoint { get; set; }
        Point EndPoint { get; set; }
    }

    public interface IDiagramItem
    {                
        void Refresh();
    }

    public interface IUndoService
    {
        void Execute(IDiagramCommand command);

        void Undo();

        void Redo();

        bool CanUndo { get; }

        bool CanRedo { get; }

        List<string> UndoTitles { get; }
    }
}