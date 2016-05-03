using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SMART.Gui.ViewModel.TestcaseViewModels;
using Syncfusion.Windows.Tools.Controls;

namespace SMART.Gui.View.TestcaseViews
{
    /// <summary>
    /// Interaction logic for TestcaseModelView.xaml
    /// </summary>
    public partial class TestcaseModelView
    {
        private readonly TestcaseModelViewModel viewModel;

        public TestcaseModelView(TestcaseModelViewModel viewModel)
        {
            DataContext = this.viewModel = viewModel;

            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            diagramModel.Nodes.SourceCollection = viewModel.States;
            diagramModel.Connections.SourceCollection = viewModel.Transitions;
            
            DockingManager.SetState(this, DockState.Document);            
        }
    }
}
