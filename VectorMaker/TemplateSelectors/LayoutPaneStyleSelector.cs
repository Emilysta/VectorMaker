using System.Windows;
using System.Windows.Controls;
using VectorMaker.ViewModel;

namespace VectorMaker.TemplateSelectors
{
    /// <summary>
    /// This class is style selector for elements that can be docked in MainWindow.
    /// </summary>
    public class LayoutPaneStyleSelector : StyleSelector
    {
        /// <summary>
        /// Style for <see cref="ToolBaseViewModel"/>
        /// </summary>
        public Style ToolStyle { get; set; }
        /// <summary>
        /// Style for <see cref="DrawingCanvasViewModel"/>
        /// </summary>
        public Style DrawingCanvasStyle { get; set; }

        /// <summary>
        /// Overridden method of <see cref="StyleSelector.SelectStyle(object, DependencyObject)"/>
        /// <returns>Returns: <see cref="Style"/> for specific document</returns>
        /// </summary>
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ToolBaseViewModel)
                return ToolStyle;

            if (item is DrawingCanvasViewModel)
                return DrawingCanvasStyle;

            return base.SelectStyle(item, container);
        }
    }
}
