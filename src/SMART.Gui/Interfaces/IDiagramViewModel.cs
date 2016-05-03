using SMART.Core.Interfaces;
using SMART.Gui.Controls.DiagramControl.View;

namespace SMART.Gui.Interfaces
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows;

    using Commands;

    using Controls.DiagramControl;
    using Core.DomainModel;

    public interface IDiagramViewModel
    {
        Guid Id { get; }

        IModel Model { get; }

        ObservableCollection<IDiagramItem> DiagramItems { get; }

        IDiagramItem CurrentItem { get; set; }

        RoutedActionCommand Delete { get; set; }

        DiagramCanvas DiagramCanvas { get; set; }

        bool IsFirstRun { get; set; }

        bool IsGrayed { get; set; }

        CanvasData CanvasInfo { get; set; }

        void BeginModelBatch();

        void EndModelBatch();

        State AddStateToDomainModel(Point location);

        State AddStateToDomainModel(Point location, State state);

        State AddStateToDomainModel(Point location, State state, string label);

        Transition AddTransitionToDomainModel(IConnectable source, IConnectable destination);

        Transition AddTransitionToDomainModel(IConnectable source, IConnectable destination, Transition transition);

        void RemoveElementFromDomainModel(ISelectable element);

        void ToggleGridLines();

        void LayoutModel(string type);

        void UpdateTransitionSource(Transition transition, IConnectable newSource);

        void UpdateTransitionTarget(Transition transition, IConnectable newTarget);
    }

    public class CanvasData
    {
        public double TranslateViewX { get; set; }
        public double TranslateViewY { get; set; }

        public double ScaleViewX { get; set; }
        public double ScaleViewY { get; set; }
    }
}
