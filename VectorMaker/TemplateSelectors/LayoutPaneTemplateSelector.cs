using System.Windows;
using System.Windows.Controls;
using VectorMaker.Pages;

namespace VectorMaker.TemplateSelectors
{
    public class LayoutPaneTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FileViewTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            if (item is DrawingCanvas)
                return FileViewTemplate;
            return base.SelectTemplate(item, container);
        }
    }
}
