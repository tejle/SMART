﻿namespace SMART.Gui.Controls.DiagramControl.Services.Commands
{
    using System.Linq;
    using System.Windows;

    using Core.DomainModel;

    using Interfaces;

    using Shapes;

    using ViewModel;

    public class AddStateCommand : IDiagramCommand
    {
        private const string DefaultCommandTitle = "Add State";

        public IDiagramViewModel ViewModel { get; set; }
    
        public Point Location { get; set; }
        public State TheState { get; set; }
        public string Title { get; set; }

        public AddStateCommand(IDiagramViewModel viewModel, Point location) : this(viewModel, location, null) { }

        public AddStateCommand(IDiagramViewModel viewModel, Point location, string title)
        {
            this.ViewModel = viewModel;
            this.Location = location;
            this.Title = (!string.IsNullOrEmpty(title)) ? title : DefaultCommandTitle;
        }


        public void Execute()
        {
            this.TheState = this.ViewModel.AddStateToDomainModel(this.Location);
        }

        public void Undo()
        {
            var state =
                    (from s in this.ViewModel.DiagramItems
                     where s is StateViewModel && (s as StateViewModel).State == this.TheState
                     select s).FirstOrDefault();
            if (state != null)
            {
                this.ViewModel.RemoveElementFromDomainModel(state as ISelectable);
            }
        }

        public void Redo()
        {
            this.ViewModel.AddStateToDomainModel(this.Location, this.TheState);
        }

    }
}