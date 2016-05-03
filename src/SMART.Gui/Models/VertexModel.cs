using SMART.Gui.Providers;

namespace SMART.Gui.Models
{
    public class VertexModel //: DataModel
    {
        private IVertexProvider _vertexProvider;
//        private IVertex _vertex;

        public VertexModel(IVertexProvider vertexProvider)
        {
            _vertexProvider = vertexProvider;
        }

  //      protected override void OnActivated()
  //      {
  ////          base.OnActivated();

  //          FetchVertex();
  //      }

  //      private void FetchVertex()
  //      {
  //          _vertex = _vertexProvider.GetVertex();
  //      }

        public string Label { get;set;}// { return _vertex.Label; } set { _vertex.Label = value; } }

        public string[] Requirements { get; set; }// { return _vertex.Requirements; } set { _vertex.Requirements = value; } }
    }
}
