using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;
using System;

namespace SMART.Gui.View
{
    public class ViewBase : ContentControl
    {
        private Storyboard fadeInStoryBoard;
        private DoubleAnimation fadeInAnimation;

        public ViewBase()
        {
            NameScope.SetNameScope(this, new NameScope());
            RegisterName("view", this);
            Loaded += ViewBaseLoaded;
        }

        void ViewBaseLoaded(object sender, RoutedEventArgs e)
        {
            fadeInAnimation = new DoubleAnimation(0, 1, new Duration(TimeSpan.FromMilliseconds(500)), FillBehavior.HoldEnd);
            fadeInStoryBoard = new Storyboard();
            fadeInStoryBoard.Children.Add(fadeInAnimation);
            fadeInStoryBoard.Completed += fadeInStoryBoard_Completed;
            Storyboard.SetTargetName(fadeInAnimation, "view");
            Storyboard.SetTargetProperty(fadeInAnimation, new PropertyPath(OpacityProperty));
            fadeInStoryBoard.Begin(this);
        }

        void fadeInStoryBoard_Completed(object sender, EventArgs e)
        {
            Opacity = fadeInAnimation.To.Value;
            fadeInStoryBoard.Remove(this);
        }
    }
}
