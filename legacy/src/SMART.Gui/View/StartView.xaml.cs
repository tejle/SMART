using System.Windows.Media.Animation;

namespace SMART.Gui.View
{
    /// <summary>
    /// Interaction logic for StartView.xaml
    /// </summary>
    public partial class StartView
    {
        public StartView()
        {
            InitializeComponent();
            Loaded += StartView_Loaded;
            Unloaded += StartView_Unloaded;   
        }

        void StartView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var animation = Resources["OnMainBackgroundLoaded"] as Storyboard;
            if (animation != null)
            {
                animation.Begin(this, true);
            }
        }

        void StartView_Unloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var animation = Resources["OnMainBackgroundLoaded"] as Storyboard;
            if (animation != null)
            {
                animation.Remove(this);
            }

        }
   }
}
