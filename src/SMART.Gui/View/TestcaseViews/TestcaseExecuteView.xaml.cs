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

namespace SMART.Gui.View
{
    /// <summary>
    /// Interaction logic for TestcaseExecuteView.xaml
    /// </summary>
    public partial class TestcaseExecuteView
    {
        private readonly TestcaseExecuteViewModel viewModel;

        public TestcaseExecuteView(TestcaseExecuteViewModel viewModel)
        {
            DataContext = this.viewModel = viewModel;
            
            InitializeComponent();
        }
    }
}
