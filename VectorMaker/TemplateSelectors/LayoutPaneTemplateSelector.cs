using System.Windows;
using System.Windows.Controls;
using VectorMaker.ViewModel;

namespace VectorMaker.TemplateSelectors
{
    /// <summary>
    /// This class is template selector for elements that can be docked in MainWindow.
    /// </summary>
    public class LayoutPaneTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Template for <see cref="DrawingCanvasViewModel"/>
        /// </summary>
        public DataTemplate FileViewTemplate { get; set; }
        /// <summary>
        /// Template for <see cref="ObjectTransformsViewModel"/>
        /// </summary>
        public DataTemplate ObjectTransformsViewTemplate { get; set; }
        /// <summary>
        /// Template for <see cref="ObjectAlignmentViewModel"/>
        /// </summary>
        public DataTemplate ObjectAlignmentViewTemplate { get; set; }
        /// <summary>
        /// Template for <see cref="ObjectPropertiesViewModel"/>
        /// </summary>
        public DataTemplate ObjectPropertiesViewTemplate { get; set; }
        /// <summary>
        /// Template for <see cref="DrawingLayersToolViewModel"/>
        /// </summary>
        public DataTemplate DrawingLayersToolViewTemplate { get; set; }

        /// <summary>
        /// Overridden method of <see cref="DataTemplateSelector.SelectTemplate(object, DependencyObject)"/>
        /// <returns>Returns: <see cref="DataTemplate"/> for specific object when docking</returns>
        /// </summary>
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DrawingCanvasViewModel)
                return FileViewTemplate;

            if (item is ObjectTransformsViewModel)
                return ObjectTransformsViewTemplate;

            if (item is ObjectPropertiesViewModel)
                return ObjectPropertiesViewTemplate;

            if (item is ObjectAlignmentViewModel)
                return ObjectAlignmentViewTemplate;

            if (item is DrawingLayersToolViewModel)
                return DrawingLayersToolViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }
}
