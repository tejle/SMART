using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wpf.Controls
{
    public class SmartTreeView : TreeView
    {
        protected override void OnContextMenuOpening(ContextMenuEventArgs e)
        {
            base.OnContextMenuOpening(e);

            DependencyObject iSender = e.OriginalSource as DependencyObject;

            while (iSender != null && !(iSender is SmartTreeViewItem))
            {
                iSender = VisualTreeHelper.GetParent(iSender);
            }
            if (iSender is SmartTreeViewItem)
            {
                (iSender as SmartTreeViewItem).IsSelected = true;
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new SmartTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is SmartTreeViewItem;
        }
    }
}
