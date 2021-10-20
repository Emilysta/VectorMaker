using System.Diagnostics;
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
        public NewDocumentPage()
        {
            InitializeComponent();
        }

        private void OpenDocument_Click(object sender, RoutedEventArgs e)
        {
            if (!TabControlManager.OpenExistingDocumentTab())
            {
                Trace.WriteLine("brlbelr");
                //toDo warning with file error or path
            }
        }

        private void NewDocument_Click(object sender, RoutedEventArgs e)
        {
            TabControlManager.OpenNewDocumentTab();
        }
    }
}
