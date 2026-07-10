using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Data;
using SMART.Core.DomainModel;

using SMART.Gui.Controls;

namespace SMART.Gui.Converters
{
    public class StateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value == null)
                return null;
            var coll = value as ListCollectionView;
            var list = new List<SmartNode>();
            foreach(State state in coll)
            {
                list.Add(new SmartNode(state));
            }
            return new ObservableCollection<SmartNode>(list);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}