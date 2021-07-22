using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VectorMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SolidColorBrush m_selectedTabItemBackground;
        public MainWindow()
        {
            InitializeComponent();
            m_selectedTabItemBackground = (SolidColorBrush)Application.Current.FindResource("TabItemBackgroundSelectedColor");
        }

        private void TabControl1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            foreach (var item in e.AddedItems)
                (item as TabItem).Background = m_selectedTabItemBackground;
            //(item as TabItem).Background = new SolidColorBrush(Color.FromArgb(128,128,128,128));
            foreach (var item in e.RemovedItems)
                (item as TabItem).Background = new SolidColorBrush(Color.FromArgb(0,0,0,8));
        }
    }
}
