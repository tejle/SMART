using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SMART.Gui.ViewModel;

namespace SMART.Gui.View
{
    /// <summary>
    /// Interaction logic for ProjectView.xaml
    /// </summary>
    public partial class ProjectView
    {
        public ProjectView()
        {
            InitializeComponent();

            Loaded += ProjectView_Loaded;
        }

        void ProjectView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = DataContext as IViewModel;
            if (viewModel != null)
            {
                viewModel.ViewLoaded();
            }
        }

        private void PART_TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            var control = sender as Control;
            if(control == null) return;
            var viewmodel = control.DataContext as IEditableViewModel;
            if(viewmodel == null) return;

            if(e.Key == Key.Enter)
            {
                viewmodel.IsEditMode = false;
            }
            else if (e.Key == Key.Escape)
            {
                viewmodel.IsEditMode = false;
            }
            
        }

        private void PART_TextBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            var viewmodel = control.DataContext as IEditableViewModel;
            if (viewmodel == null) return;

            viewmodel.IsEditMode = false;
        }

        private void PART_TextBox_Loaded(object sender, RoutedEventArgs e)
        {
            var control = sender as Control;
            if (control == null) return;
            var viewmodel = control.DataContext as IEditableViewModel;
            if (viewmodel == null) return;
            if (viewmodel.StartInEditMode) viewmodel.IsEditMode = true;
        }
    }
}
