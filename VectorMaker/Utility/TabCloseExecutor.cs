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
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            MetroTabItem tabItem = parameter as MetroTabItem;
            Frame frame = tabItem.Content as Frame;
            DrawingCanvas drawingCanvas = frame?.Content as DrawingCanvas;
            if (drawingCanvas != null)
            {
                if (drawingCanvas.IsSaved)
                    return true;
                else
                    return false;
            }
            return true;
        }

        public void Execute(object parameter)
        {
            Trace.WriteLine("CloseButtonExecute");
        }
    }
}
