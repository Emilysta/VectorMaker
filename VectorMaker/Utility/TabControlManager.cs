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
            MetroTabItem newItem = new MetroTabItem();
            newItem.Header = "untilted.svg";
            DrawingCanvas page = new DrawingCanvas();

            Frame tabItemFrame = new Frame();
            tabItemFrame.Content = page;
            newItem.Content = tabItemFrame;

            MainWindow.Instance.FilesTabControl.Items.Add(newItem);
        }

        public static bool OpenExistingDocumentTab()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Scalable Vector Graphics (*.svg) | *.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                char[] splitters = { '/', '\\' };
                MetroTabItem newItem = new MetroTabItem();
                newItem.Header = openFileDialog.FileName.Split(splitters).Last();
                DrawingCanvas page = new DrawingCanvas(openFileDialog.FileName);
                //Trace.WriteLine(openFileDialog.FileName);
                Frame tabItemFrame = new Frame();
                tabItemFrame.Content = page;
                newItem.Content = tabItemFrame;
                MainWindow.Instance.FilesTabControl.Items.Add(newItem);
                return true;
            }
            return false;
        }
    }
}
