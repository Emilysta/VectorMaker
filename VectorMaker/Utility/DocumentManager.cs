using AvalonDock.Layout;
using Microsoft.Win32;
using System.Linq;
using System.Windows.Controls;
using VectorMaker.Pages;

namespace VectorMaker.Utility
{
    public static class DocumentManager
    {
        public static void OpenNewDocumentTab()
        {
            string header = "untilted.xaml";
            DrawingCanvas page = new DrawingCanvas();
            CreateDocument(page, header);
        }

        public static bool OpenExistingDocumentTab()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Scalable Vector Graphics (*.svg) | *.svg | Extensible Application Markup Language (*.xaml) | *.xaml";
            if (openFileDialog.ShowDialog() == true)
            {
                char[] splitters = { '/', '\\' };
                string header = openFileDialog.FileName.Split(splitters).Last();
                DrawingCanvas page = new DrawingCanvas(openFileDialog.FileName);
                CreateDocument(page, header);
                return true;
            }
            return false;
        }

        public static void RunOpenVisibilityCheck()
        {
            if (MainWindow.Instance.DocumentPaneGroup.Children.Count <= 0)
            {
                MainWindow.Instance.DockingManager.Visibility = System.Windows.Visibility.Hidden;
                MainWindow.Instance.NewDocumentFrame.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                MainWindow.Instance.DockingManager.Visibility = System.Windows.Visibility.Visible;
                MainWindow.Instance.NewDocumentFrame.Visibility = System.Windows.Visibility.Hidden;
            }

        }

        private static void CreateDocument(DrawingCanvas page, string header)
        {
            Frame frame = new Frame();
            frame.Content = page;
            LayoutDocument layoutDocument = new LayoutDocument();
            layoutDocument.Content = frame;
            layoutDocument.ContentId = header;
            layoutDocument.Title = header;
            layoutDocument.IsSelected = true;
            layoutDocument.Closed += (a,b) => RunOpenVisibilityCheck();
            MainWindow.Instance.DocumentPaneGroup.Children.Add(layoutDocument);
            RunOpenVisibilityCheck();
        }
    }
}
