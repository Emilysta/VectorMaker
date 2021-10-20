using MahApps.Metro.Controls;
using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using VectorMaker.Pages;

namespace VectorMaker.Utility
{
    public class TabCloseExecutor : ICommand
    {

        public static Frame newDocumentFrame;
        public static MetroTabControl filesTabControl;
        public event EventHandler CanExecuteChanged;

        static TabCloseExecutor()
        {
            newDocumentFrame = MainWindow.Instance.NewDocumentFrame;
            filesTabControl = MainWindow.Instance.FilesTabControl;
        }
        public bool CanExecute(object parameter)
        {
            MetroTabItem tabItem = parameter as MetroTabItem;
            Frame frame = tabItem?.Content as Frame;
            DrawingCanvas drawingCanvas = frame?.Content as DrawingCanvas;
            if (drawingCanvas != null)
            {
                if (drawingCanvas.IsSaved)
                    return true;
                else
                    return false;
            }
            return false;
        }

        public void Execute(object parameter)
        {
            Trace.WriteLine("CloseButtonExecute");
            RunCloseVisibilityCheck();
        }

        private static void RunCloseVisibilityCheck()
        {
            if (filesTabControl.Items.Count <= 1)
            {
                newDocumentFrame.Visibility = System.Windows.Visibility.Visible;
                filesTabControl.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                newDocumentFrame.Visibility = System.Windows.Visibility.Hidden;
                filesTabControl.Visibility = System.Windows.Visibility.Visible;
            }
        }

    }
}
