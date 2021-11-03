using System.Windows;
using System.Windows.Controls;
using VectorMaker.ViewModel;

namespace VectorMaker.TemplateSelectors
{
    public class LayoutPaneTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FileViewTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is DrawingCanvasViewModel)
                return FileViewTemplate;
            return base.SelectTemplate(item, container);
        }
    }
}
