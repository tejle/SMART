using System;
using System.Windows;
using System.Windows.Controls;

namespace Wpf.Controls
{
    public class SmartTreeViewItem : TreeViewItem
    {
        public Guid ID
        {
            get { return (Guid)this.GetValue(IDProperty); }
            set { this.SetValue(IDProperty, value); }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SmartTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SmartTreeViewItem;
        }

        public static readonly DependencyProperty IDProperty = DependencyProperty.Register(
    "ID", typeof(Guid), typeof(SmartTreeViewItem), new PropertyMetadata(Guid.Empty));

    }
}
