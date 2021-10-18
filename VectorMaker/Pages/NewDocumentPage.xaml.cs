using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;


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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Scalable Vector Graphics (*.svg) | *.svg";
            if (openFileDialog.ShowDialog() == true)
            {
                char[] splitters = { '/', '\\' };
                MetroTabItem newItem = new MetroTabItem();
                newItem.Header = openFileDialog.FileName.Split(splitters).Last();
                DrawingCanvas page = new DrawingCanvas(openFileDialog.FileName);
                Trace.WriteLine(openFileDialog.FileName);
                Frame tabItemFrame = new Frame();
                tabItemFrame.Content = page;
                newItem.Content = tabItemFrame;

                MainWindow.Instance.FilesTabControl.Items.Add(newItem);

                RemoveParentTabFromControl();
            }
        }

        private void NewDocument_Click(object sender, RoutedEventArgs e)
        {
            MetroTabItem newItem = new MetroTabItem();
            newItem.Header = "untilted.svg";
            DrawingCanvas page = new DrawingCanvas();

            Frame tabItemFrame = new Frame();
            tabItemFrame.Content = page;
            newItem.Content = tabItemFrame;

            MainWindow.Instance.FilesTabControl.Items.Add(newItem);
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
