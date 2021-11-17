using System.Windows;
using System.Windows.Controls;
using VectorMaker.ViewModel;

namespace VectorMaker.TemplateSelectors
{
    public class LayoutPaneTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FileViewTemplate { get; set; }
        public DataTemplate ObjectTransformsViewTemplate { get; set; }
        public DataTemplate ObjectAlignmentViewTemplate { get; set; }
        public DataTemplate ObjectPropertiesViewTemplate { get; set; }
        public DataTemplate DrawingLayersToolViewTemplate { get; set; }

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
