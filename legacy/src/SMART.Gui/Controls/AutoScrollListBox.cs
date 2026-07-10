using System;

namespace SMART.Gui.Controls
{
    using System.Collections.Specialized;
    using System.Windows;
    using System.Windows.Controls;

    public class ListBoxExtensions : DependencyObject
    {

        public static bool GetIsAutoscroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsAutoscrollProperty);
        }

        public static void SetIsAutoscroll(DependencyObject obj, bool value)
        {
            obj.SetValue(IsAutoscrollProperty, value);
        }

        public static readonly DependencyProperty IsAutoscrollProperty =
            DependencyProperty.RegisterAttached("IsAutoscroll", typeof(bool), typeof(ListBoxExtensions), new UIPropertyMetadata(default(bool), OnIsAutoscrollChanged));

        private static void OnIsAutoscrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var val = (bool)e.NewValue;
            var lb = d as ListBox;
            var ic = lb.Items;
            var data = ic.SourceCollection as INotifyCollectionChanged;

            var autoscroller = new NotifyCollectionChangedEventHandler(
                (s1, e1) =>
                {
                    object selectedItem = default(object);
                    switch (e1.Action)
                    {
                        case NotifyCollectionChangedAction.Add:
                        case NotifyCollectionChangedAction.Move: selectedItem = e1.NewItems[e1.NewItems.Count - 1]; break;
                        case NotifyCollectionChangedAction.Remove: if (ic.Count < e1.OldStartingIndex) { selectedItem = ic[e1.OldStartingIndex - 1]; } else if (ic.Count > 0) selectedItem = ic[0]; break;
                        case NotifyCollectionChangedAction.Reset: if (ic.Count > 0) selectedItem = ic[0]; break;
                    }

                    if (selectedItem != default(object))
                    {
                        ic.MoveCurrentTo(selectedItem);
                        lb.ScrollIntoView(selectedItem);
                    }
                });

            if (val) data.CollectionChanged += autoscroller;
            else data.CollectionChanged -= autoscroller;

        }
    }

}
