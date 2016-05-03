using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SMART.Gui.Interfaces;
using SMART.Gui.ViewModel;

namespace SMART.Gui.View
{
    /// <summary>
    /// Interaction logic for ModelDesignerView.xaml
    /// </summary>
    public partial class ModelDesignerView
    {
        private const ModifierKeys Modifier = ModifierKeys.Control | ModifierKeys.Shift;

        private ModelDesignerViewModel viewModel;

        public ModelDesignerView()
        {
            InitializeComponent();

            Loaded += ModelDesignerView_Loaded;
            PreviewKeyDown += ModelDesignerView_PreviewKeyDown;
        }

        void ModelDesignerView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {                
                if ((Keyboard.Modifiers == Modifier))
                {
                        viewModel.PreviousModel();
                }
                else if ((Keyboard.Modifiers & (ModifierKeys.Control)) > 0)
                {
                        viewModel.NextModel();
                }
            }
        }

        void ModelDesignerView_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = DataContext as ModelDesignerViewModel;
            if (viewModel != null)
            {
                viewModel.View = this;
                viewModel.ViewLoaded();
            }
        }

        private void ContentPresenter_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {           
            if (sender is ListBoxItem)
            {
                viewModel.CurrentModel = (sender as ListBoxItem).DataContext as ModelViewModel;
                (sender as ListBoxItem).IsSelected = true;
                e.Handled = true;
            }
        }

    }
}
