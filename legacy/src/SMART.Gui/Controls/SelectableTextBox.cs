using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMART.Gui.Controls
{
    using System.Windows;
    using System.Windows.Controls;

    public static class SelectableTextBox
    {
        #region SelectAllOnClick attached property
        public static readonly DependencyProperty SelectAllOnInputProperty =
            DependencyProperty.RegisterAttached("SelectAllOnInput", typeof(bool), typeof(SelectableTextBox),
                new FrameworkPropertyMetadata((bool)false,
                    FrameworkPropertyMetadataOptions.None,
                    new PropertyChangedCallback(OnSelectAllOnClickChanged)));

        public static bool GetSelectAllOnInput(DependencyObject d)
        {
            return (bool)d.GetValue(SelectAllOnInputProperty);
        }

        public static void SetSelectAllOnInput(DependencyObject d, bool value)
        {
            d.SetValue(SelectAllOnInputProperty, value);
        }
        #endregion

        /// <summary>
        /// Handles changes to the SelectAllOnClick property.
        /// </summary>
        private static void OnSelectAllOnClickChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender as TextBox != null && (bool)e.NewValue)
            {
                //((TextBox)sender).AddHandler(UIElement.MouseUpEvent, new RoutedEventHandler(OnSelectAllText), true);
                //((TextBox)sender).AddHandler(UIElement.MouseDownEvent, new RoutedEventHandler(OnSelectAllText));
                ((TextBox)sender).AddHandler(UIElement.GotFocusEvent, new RoutedEventHandler(OnSelectAllText));
            }
            else if (sender as TextBox != null && !(bool)e.NewValue)
            {
                //((TextBox)sender).RemoveHandler(UIElement.MouseUpEvent, new RoutedEventHandler(OnSelectAllText));
                //((TextBox)sender).RemoveHandler(UIElement.MouseDownEvent, new RoutedEventHandler(OnSelectAllText));
                ((TextBox)sender).RemoveHandler(UIElement.GotFocusEvent, new RoutedEventHandler(OnSelectAllText));
            }
        }

        /// <summary>
        /// Handler that select all TextBox's text
        /// </summary>
        private static void OnSelectAllText(object sender, RoutedEventArgs e)
        {
            if (sender as TextBox != null)
            {
                var tb = (TextBox) sender;
                if (!tb.SelectionLength.Equals(tb.Text.Length)) // Everything is already selected
                {
                    ((TextBox)sender).SelectAll();  
                    
                    //tb.Select(0,0);
                }
                else
                {
                    
                }                
            }
        }
    }

}
