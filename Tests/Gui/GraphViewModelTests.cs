using SMART.Core.DomainModel;
using SMART.Core.Services;

namespace SMART.Test.Gui
{
    using SMART.Core.Interfaces.Services;
    using SMART.Gui.ViewModel;
    
    using NUnit.Framework;
    using IOC;
    using Moq;
    using SMART.Gui.Controls.DiagramControl.Shapes;

    [TestFixture]
    public class ModelViewModelTests    
    {
        //[Test]
        //public void add_StateShape_to_Nodes_collection_should_call_ModelService_AddExistingState()
        //{
        //    Model graph = new Model();
        //    var graphService = new Mock<IModelService>();
        //    var newState = new StateShape(Resolver.Resolve<StateViewModel>());

        //    graphService.Setup(s => s.AddExistingState(graph, newState.ViewModel.State)).Verifiable();            
            
        //    var viewModel = new ModelViewModel(graph, graphService.Object);
        //    viewModel.Nodes.Add(newState);
        //    graphService.Verify();
        //}

    }
}
