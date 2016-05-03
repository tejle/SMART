namespace SMART.Gui.Controls.DiagramControl.Services.Commands
{
    using System.Collections.Generic;

    using Interfaces;

    using ViewModel;

    public class DiagramCommandGroup<T> : List<T>, IDiagramCommand where T : IDiagramCommand
    {
        private readonly IDiagramViewModel ViewModel;

        private const string DefaultCommandTitle = "Undo many things";

        public string Title { get; set; }

        public DiagramCommandGroup(IDiagramViewModel viewModel)
            : this(viewModel, null)
        {
        }

        public DiagramCommandGroup(IDiagramViewModel viewModel, string title)
        {
            ViewModel = viewModel;            
            this.Title = (!string.IsNullOrEmpty(title)) ? title : DefaultCommandTitle;
        }


        public void Execute()
        {
            ViewModel.BeginModelBatch();
            this.ForEach(c => c.Execute());
            ViewModel.EndModelBatch();
        }

        public void Undo()
        {
            ViewModel.BeginModelBatch();
            this.ForEach(c => c.Undo());
            ViewModel.EndModelBatch();
        }

        public void Redo()
        {
            ViewModel.BeginModelBatch();
            this.ForEach(c => c.Redo());
            ViewModel.EndModelBatch();
        }
    }

}
