using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SMART.Core.Interfaces;

namespace SMART.Gui.ViewModel.TestcaseViewModels
{
    using System.Collections.ObjectModel;

    using Controls;

    public class TestcaseModelViewModel : ViewModelBase, ITestcaseViewModel
    {
        private readonly ITestcase testcase;

        private ObservableCollection<SmartNode> states;

        public ObservableCollection<SmartNode> States
        {
            get
            {
                if (states == null)
                {
                    states = new ObservableCollection<SmartNode>(from v in testcase.ExecutableModel.States select new SmartNode(v));
                }
                return states;
            }
        }

        private ObservableCollection<SmartLineConnector> transitions;

        public ObservableCollection<SmartLineConnector> Transitions
        {
            get
            {
                if (transitions == null)
                    transitions = new ObservableCollection<SmartLineConnector>(from e in testcase.ExecutableModel.Transitions
                                                                         select new SmartLineConnector(
                                                                             (from v in States where v.localState == e.Source select v).First(),
                                                                             (from v in States where v.localState == e.Destination select v).First(), e));
                return transitions;
            }
        }

        public TestcaseModelViewModel(ITestcase testcase)
        {
            this.testcase = testcase;
        }


        public override string Icon
        {
            get { return Constants.GRAPH_ICON_URL; }
        }
    }
}
