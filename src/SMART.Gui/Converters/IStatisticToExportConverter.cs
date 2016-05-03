
using SMART.Core.Interfaces;
using SMART.Core.Interfaces.Metadata;

namespace SMART.Gui.Converters
{
    using System;
    using System.ComponentModel.Composition;
    using System.Globalization;
    using System.Linq;
    using System.Windows;
    using System.Windows.Data;

    [ValueConversion(typeof(IStatistic), typeof(Export<IStatistic, IStatisticMetadata>))]
    public class IStatisticToExportConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null) return DependencyProperty.UnsetValue;
            if (value == null) return DependencyProperty.UnsetValue;

            var exportCollectionView = (parameter as CollectionViewSource);
            if (exportCollectionView == null) return DependencyProperty.UnsetValue;

            var exportCollection = (exportCollectionView.Source as System.Collections.ObjectModel.ObservableCollection<Export<IStatistic, IStatisticMetadata>>);
            var o = exportCollection == null ? DependencyProperty.UnsetValue 
                        : exportCollection.FirstOrDefault(e => e.GetExportedObject().Equals(value)) ?? DependencyProperty.UnsetValue;
            return o;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            var export = value as Export<IStatistic, IStatisticMetadata>;

            var o = export == null ? DependencyProperty.UnsetValue
                        : export.GetExportedObject() ?? DependencyProperty.UnsetValue;
            return o;
        }
    }
}