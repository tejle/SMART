using SMART.Gui.Providers;

namespace SMART.Gui.Models
{
    public class EdgeModel //: DataModel
    {
        //private IEdgeProvider _edgeProvider;
//        private IEdge _edge;

        public EdgeModel(IEdgeProvider edgeProvider)
        {
            //_edgeProvider = edgeProvider;
        }

  //      protected override void OnActivated()
  //      {
  ////          base.OnActivated();

  //          FetchEdge();
  //      }

    //    private void FetchEdge()
    //    {
    ////        _edge = _edgeProvider.GetEdge();
    //    }

        public string Label { get; set;}// { return _edge.Label; } set { _edge.Label = value; } }

        public string[] Requirements { get; set; }// { return _edge.Requirements; } set { _edge.Requirements = value; } }

        public string Action { get; set; }// { return _edge.Action; } set { _edge.Action = value; } }

        public string[] Parameters { get; set; }// { return _edge.Parameters; } set { } }

        public string Guard { get; set; }// { return _edge.Guard; } set { _edge.Guard = value; } }
    }
}
