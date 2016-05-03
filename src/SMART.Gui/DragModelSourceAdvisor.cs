using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Xml;
using System.Xml.Linq;
using FluidKit.Helpers.DragDrop;
using SMART.Gui.Controls.DiagramControl.Helpers;
using SMART.Gui.ViewModel;

namespace SMART.Gui
{
    public class DragModelSourceAdvisor : IDragSourceAdvisor
    {
        private static DataFormat SupportedFormat = DataFormats.GetDataFormat("SMART");
        private UIElement _sourceUI;


        public DragDropEffects SupportedEffects
        {
            get { return DragDropEffects.Copy | DragDropEffects.Move; }
        }

        public UIElement SourceUI
        {
            get { return _sourceUI; }
            set { _sourceUI = value; }
        }

        public DataObject GetDataObject(UIElement draggedElt)
        {
            var viewModel = (draggedElt as FrameworkElement).DataContext as ProjectModelViewModel;

            var data = new DataObject(SupportedFormat.Name, SerializeModel(viewModel));

            return data;
        }

        public void FinishDrag(UIElement draggedElt, DragDropEffects finalEffects)
        {
            //if ((finalEffects & DragDropEffects.Move) == DragDropEffects.Move)
            //{
            //    Panel panel = SourceUI as Panel;
            //    if (panel != null)
            //    {
            //        panel.Children.Remove(draggedElt);
            //    }
            //}
        }

        public bool IsDraggable(UIElement dragElt)
        {
            return (dragElt as FrameworkElement).DataContext is ProjectModelViewModel;            
        }

        public UIElement GetTopContainer()
        {
            return Application.Current.MainWindow.Content as UIElement;
        }

        private XElement SerializeModel(ProjectModelViewModel model)
        {
            var root = new XElement("Root", new XElement("Id", model.Id), new XElement("Name", model.Name));

            return root;
        }

    }

    public class DropModelTargetAdvisor : IDropTargetAdvisor
    {
        private static DataFormat SupportedFormat = DataFormats.GetDataFormat("SMART");
        private bool _applyMouseOffset;
        private UIElement _targetUI;

        private ListBoxItem currentItem;

        #region IDropTargetAdvisor Members

        public bool IsValidDataObject(IDataObject obj)
        {
            return obj.GetDataPresent(SupportedFormat.Name);
        }

        public void OnDropCompleted(IDataObject obj, Point dropPoint)
        {
            var model = ExtractElement(obj);
            var id = new Guid(model.Element("Id").Value);

            var parent = VisualTreeHelperEx.GetParent<ListBoxItem>(TargetUI) as ListBoxItem;
            var viewModel = parent.DataContext as ProjectScenarioViewModel;
            viewModel.AddExistingModel(id);
            
            if (currentItem != null)
            {
                currentItem.Background = new SolidColorBrush(Colors.Transparent);
                currentItem.IsSelected = true;
            }
        }

        public UIElement TargetUI
        {
            get { return _targetUI; }
            set { _targetUI = value; }
        }

        public bool ApplyMouseOffset
        {
            get { return _applyMouseOffset; }
        }

        public UIElement GetVisualFeedback(IDataObject obj)
        {            
            if (currentItem != null)
            {
                currentItem.Background = new SolidColorBrush(Colors.Transparent);
            }
            currentItem = VisualTreeHelperEx.GetParent<ListBoxItem>(TargetUI) as ListBoxItem;
            currentItem.Background = new SolidColorBrush(Colors.LimeGreen);            
            
            var viewModel = ExtractElement(obj);
            var border = new Border();
            var text = new TextBlock();
            border.Child = text;
            text.Text = viewModel.Element("Name").Value;
            text.HorizontalAlignment = HorizontalAlignment.Center;
            text.VerticalAlignment = VerticalAlignment.Center;

            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(4);
            border.BorderBrush = new SolidColorBrush(Color.FromArgb(50, 0, 0, 0));
            border.Background = new SolidColorBrush(Colors.White);
            border.Width = 128;
            border.Height = 96;
            border.Opacity = 0.5;
            border.IsHitTestVisible = false;

            var anim = new DoubleAnimation(0.75, new Duration(TimeSpan.FromMilliseconds(500)));
            anim.From = 0.25;
            anim.AutoReverse = true;
            anim.RepeatBehavior = RepeatBehavior.Forever;
            border.BeginAnimation(UIElement.OpacityProperty, anim);

            return border;
        }

        public UIElement GetTopContainer()
        {
            return Application.Current.MainWindow.Content as UIElement;
        }

        #endregion

        private XElement ExtractElement(IDataObject obj)
        {
            return obj.GetData("SMART") as XElement;
        }
    }
}
