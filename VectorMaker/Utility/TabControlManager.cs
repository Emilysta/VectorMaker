using MahApps.Metro.Controls;
using Microsoft.Win32;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using VectorMaker.Pages;

namespace VectorMaker.Utility
{
    public static class TabControlManager
    {
        public static void OpenNewDocumentTab()
        {
            string header = "untilted.svg";
            DrawingCanvas page = new DrawingCanvas();
            CreateAndAddTabItem(page, header);
        }

        public static bool OpenExistingDocumentTab()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Scalable Vector Graphics (*.svg) | *.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                char[] splitters = { '/', '\\' };
                string header = openFileDialog.FileName.Split(splitters).Last();
                DrawingCanvas page = new DrawingCanvas(openFileDialog.FileName);

                //Trace.WriteLine(openFileDialog.FileName);
                CreateAndAddTabItem(page, header);
                return true;
            }
            return false;
        }

        public static void RunOpenVisibilityCheck()
        {
            if (TabCloseExecutor.filesTabControl.Items.Count >= 1)
            {
                TabCloseExecutor.newDocumentFrame.Visibility = System.Windows.Visibility.Hidden;
                TabCloseExecutor.filesTabControl.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                TabCloseExecutor.newDocumentFrame.Visibility = System.Windows.Visibility.Visible;
                TabCloseExecutor.filesTabControl.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private static void CreateAndAddTabItem(DrawingCanvas page, string header)
        {
            page.Width = double.NaN;
            page.Height = double.NaN;
            MetroTabItem newItem = new MetroTabItem();
            newItem.Header = header;
            Frame tabItemFrame = new Frame();
            tabItemFrame.Content = page;
            newItem.Content = tabItemFrame;
            MainWindow.Instance.FilesTabControl.Items.Add(newItem);
            RunOpenVisibilityCheck();
            MainWindow.Instance.FilesTabControl.SelectedItem = newItem;
        }
    }
}
