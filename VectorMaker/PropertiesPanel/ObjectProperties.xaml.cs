using System.Windows.Controls;
using System.Windows.Media;


namespace VectorMaker.PropertiesPanel
{
    /// <summary>
    /// Interaction logic for ObjectProperties.xaml
    /// </summary>
    public partial class ObjectProperties : Page
    {
        public ObjectProperties()
        {

            //var icon = this.IconKind;

            //string buttonContent = this.GetValue(m_content).ToString();
            //Trace.WriteLine(iconKind);
            //Trace.WriteLine(buttonContent);
            //if (iconKind != PackIconMaterialDesignKind.None.ToString()
            //    && buttonContent == "")
            //{
            //    IconColumnSpan = 2;
            //}
            //else if (iconKind == PackIconMaterialDesignKind.None.ToString()
            //    && buttonContent != "")
            //{
            //    ContentColumnSpan = 2;
            //}
            //else if (iconKind != PackIconMaterialDesignKind.None.ToString()
            //     && buttonContent != "")
            //{
            //    ContentColumn = 1;
            //    IconHeight = (int)(IconHeight * 0.8f);
            //    IconWidth = (int)(IconWidth * 0.8f);
            //    FontSize = FontSize * 0.8;
            //}
            InitializeComponent();

        }

        private void ObjectPropertiesTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
                (item as TabItem).Background = ColorsReference.selectedTabItemBackground;
            foreach (var item in e.RemovedItems)
                (item as TabItem).Background = ColorsReference.notSelectedTabItemBackground;
        }
    }
}
