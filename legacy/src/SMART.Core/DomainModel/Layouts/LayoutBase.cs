using System;
using SMART.Core.Interfaces;

namespace SMART.Core.DomainModel.Layouts
{
    public abstract class LayoutBase : ILayout
    {
        public IModel Model { get; set; }
        public event EventHandler LayoutComplete;
        public abstract void BeginLayout();

        public LayoutBase(IModel model)
        {
            //Model = new LayoutModel(model);
            Model = model;
            model.States.ForEach(s=>s.Tags.Clear());
            model.Transitions.ForEach(t=> t.Tags.Clear());
        }
    }
}