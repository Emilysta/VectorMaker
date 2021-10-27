using System.Windows.Controls;


namespace VectorMaker.PropertiesPanel
{
    public enum AlignRelative
    { 
        Page,
        Last,
        First
    }

    /// <summary>
    /// Interaction logic for ObjectAlignment.xaml
    /// </summary>
    /// 
    public partial class ObjectAlignment : UserControl
    {
        private AlignRelative m_alignRelative = AlignRelative.Page;
        public ObjectAlignment()
        {
            InitializeComponent();
        }

        private void AlignTop_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        private void AlignBottom_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        private void AlignLeft_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        private void AlignRight_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        private void AlignCenterVertical_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void AlignCenterHoriontal_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void DistributeHoriontal_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
        private void DistributeVertical_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void AlignRelativeToPage_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FirstToggleButton.IsChecked = false;
            LastToggleButton.IsChecked = false;
            m_alignRelative = AlignRelative.Page;
        }
        private void AlignRelativeToLast_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            FirstToggleButton.IsChecked = false;
            PageToggleButton.IsChecked = false;
            m_alignRelative = AlignRelative.Last;
        }

        private void AlignRelativeToFirst_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LastToggleButton.IsChecked = false;
            PageToggleButton.IsChecked = false;
            m_alignRelative = AlignRelative.First;
        }
    }
}
