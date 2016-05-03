namespace SMART.Gui.Controls.DiagramControl.Helpers
{
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Provides additional functionality alongside standard VisualTreeHelper.
    /// </summary>
    public static class VisualTreeHelperEx
    {
        /// <summary>
        /// Returns current object or ony parent object that expose the type provided.
        /// Very useful when accessing the main container for the ControlTemplate or for any elements within the ControlTemplate.
        /// </summary>
        /// <typeparam name="T">The type of the object to be searched.</typeparam>
        /// <param name="obj">Starting object to search for parents or itself.</param>
        /// <returns>Returns current object if it exposes the required type or any parent from the upper levels or null.</returns>
        public static DependencyObject GetParent<T>(DependencyObject obj)
        {
            if (obj is T) return obj;

            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            if (parent == null)
                return null;
            else if (!(parent is T))
                return GetParent<T>(parent);

            return parent;
        }

        public static T GetChild<T>(DependencyObject dependencyObject) where T : DependencyObject
        {
            if (dependencyObject == null) return default(T);
            if (dependencyObject is T) return (T)dependencyObject;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
            {
                var child = GetChild<T>(VisualTreeHelper.GetChild(dependencyObject, i));
                if (child != null) return child;
            }
            return null;
        }

    }
}