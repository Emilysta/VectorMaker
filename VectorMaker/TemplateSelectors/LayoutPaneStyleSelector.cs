using System.Windows;
using System.Windows.Controls;
using VectorMaker.ViewModel;

namespace VectorMaker.TemplateSelectors
{
    public class LayoutPaneStyleSelector : StyleSelector
    {
        public Style ToolStyle { get; set; }

        public Style DrawingCanvasStyle { get; set; }

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
