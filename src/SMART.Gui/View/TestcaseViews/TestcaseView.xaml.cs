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
using SMART.Gui.View.TestcaseViews;

namespace SMART.Gui.View
{
    /// <summary>
    /// Interaction logic for TestcaseView.xaml
    /// </summary>
    public partial class TestcaseView
    {
        private readonly TestcaseViewModel viewModel;

        public TestcaseView(TestcaseViewModel viewModel)
        {
            DataContext = this.viewModel = viewModel;
            
            InitializeComponent();

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            CreateViews();
        }

        private void CreateViews()
        {
            foreach (ITestcaseViewModel model in viewModel.ViewModels)
            {
                FrameworkElement view = null;

                if (model is TestcaseConfigViewModel)
                {
                    view = new TestcaseConfigView(model as TestcaseConfigViewModel);
                    
                }

                if (model is TestcaseExecuteViewModel)
                {
                    view = new TestcaseExecuteView(model as TestcaseExecuteViewModel);
                }

                if (model is TestcaseModelViewModel)
                {
                    view = new TestcaseModelView(model as TestcaseModelViewModel);
                }

                if (view != null)
                {
                    testcaseDock.Children.Add(view);
                }
            }
        }
    }
}
