using System;

namespace SMART.Gui.Providers
{
    public class EdgeProvider //: IEdgeProvider
    {
        private Guid _graphId;
        private Guid _id;

        public EdgeProvider(Guid graphId, Guid id)
        {
            _graphId = graphId;
            _id = id;
        }
        #region IEdgeProvider Members

        //        public SMART.Core.Model.IEdge GetEdge()
//        {
////            return App.CurrentProject.Graphs.Find(g => g.ID.Equals(_graphId)).Edges.Find(e => e.ID.Equals(_id));
//        }

        #endregion
    }
}
