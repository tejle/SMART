using System;

namespace SMART.Gui.Providers
{
    public class VertexProvider : IVertexProvider
    {
        private Guid _graphId;
        private Guid _id;

        public VertexProvider(Guid graphId, Guid id)
        {
            _graphId = graphId;
            _id = id;
        }
        #region IVertexProvider Members

        //public SMART.Core.Model.IVertex GetVertex()
        //{
        //    return App.CurrentProject.Graphs.Find(g => g.ID.Equals(_graphId)).Vertices.Find(v => v.ID.Equals(_id));
        //}

        #endregion
    }
}
