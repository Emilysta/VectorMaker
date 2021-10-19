using System.Windows;
using System.Windows.Controls;
using VectorMaker.Utility;

namespace VectorMaker.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy NewDocumentPage.xaml
    /// </summary>
    public partial class NewDocumentPage : Page
    {
        TabItem parentTabItem;
        public NewDocumentPage(TabItem item)
        {
            InitializeComponent();
            parentTabItem = item;
        }

        private void OpenDocument_Click(object sender, RoutedEventArgs e)
        {
           if(TabControlManager.OpenExistingDocumentTab())
           {
                RemoveParentTabFromControl();
           }
            else
            {
                //toDo warning with file error or path
            } 
        }

        private void NewDocument_Click(object sender, RoutedEventArgs e)
        {
            TabControlManager.OpenNewDocumentTab();
            RemoveParentTabFromControl();
        }

        private void RemoveParentTabFromControl()
        {
            MainWindow.Instance.FilesTabControl.Items.Remove(parentTabItem);
            MainWindow.Instance.FilesTabControl.Items.Refresh();
            parentTabItem = null;
        }
    }
}
