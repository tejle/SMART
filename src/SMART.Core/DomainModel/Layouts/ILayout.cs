using System;

namespace SMART.Core.DomainModel.Layouts
{
    public interface ILayout
    {
        event EventHandler LayoutComplete;
        void BeginLayout();
    }
}