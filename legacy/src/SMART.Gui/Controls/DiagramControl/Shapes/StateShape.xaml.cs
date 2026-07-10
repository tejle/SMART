namespace SMART.Gui.Controls.DiagramControl.Shapes
{
    using System.Windows.Controls;
    using System.Windows.Input;
    using Helpers;

    using View;

    using ViewModel;

    /// <summary>
    /// Interaction logic for StateShape.xaml
    /// </summary>
    public partial class StateShape
    {
        private StateViewModel ViewModel;

        public DiagramView ItemHost { get { return ItemsControl.GetItemsOwner(this) as DiagramView; } }

        public DiagramCanvas TheCanvas { get { return VisualTreeHelperEx.GetParent<DiagramCanvas>(this) as DiagramCanvas; } }

        public StateShape()
        {
            InitializeComponent();

     
            this.Loaded += this.StateShape_Loaded;            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

                var textBox = this.Template.FindName("PART_TextBox", this) as TextBox;
                if (textBox != null)
                {
                    textBox.PreviewKeyUp += this.textBox_PreviewKeyUp;
                    textBox.LostFocus += this.textBox_LostFocus;
                }
        }

        void textBox_LostFocus(object sender, System.Windows.RoutedEventArgs e)
        {
            if (ViewModel != null)
            {
                ViewModel.IsInEditMode = false;
                if (TheCanvas != null)
                {
                    TheCanvas.EditLabel(ViewModel);
                }
            }
        }

        private void textBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (ViewModel != null)
            {
                if (e.Key == Key.Enter)
                {
                    ViewModel.IsInEditMode = false;
                    TheCanvas.EditLabel(ViewModel);
                }
                if (e.Key == Key.Escape)
                {
                    ViewModel.IsInEditMode = false;
                    ViewModel.Name = ViewModel.OldText;
                }
            }
        }

    
        void StateShape_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is StateViewModel)
            {
                ViewModel = DataContext as StateViewModel;
                ViewModel.View = this;
                ViewModel.ViewLoaded();
            }

        }
        
        public void Delete()
        {            
            TheCanvas.RemoveElement(ViewModel);
        }
    }
}
