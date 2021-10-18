using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using VectorMaker.Drawables;
using System;
using System.Windows.Shapes;
using System.Windows.Controls;
using VectorMaker.Pages;
using MahApps.Metro.Controls;

namespace VectorMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DrawableTypes m_drawableType;
        private bool m_ignoreDrawingGeometries = true;
        private Observable<Path> m_selectedObject = null;

        public DrawableTypes DrawableType
        {
            get
            {
                return m_drawableType;
            }
            set => m_drawableType = value;
        }

        public bool IgnoreDrawingGrometries => m_ignoreDrawingGeometries;
        public Path SelectedObejct
        {
            get
            {
                return m_selectedObject.ObserwableObject;
            }
            set
            {
                m_selectedObject.ObserwableObject = value;
            }
        }

        public string SelectedObjectString { get; set; } = "No selected Obejct";

        public static MainWindow Instance { get; private set; }
        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            m_selectedObject = new Observable<Path>(SelectionOfObject);
            MetroTabItem newItem = new MetroTabItem();
            newItem.CloseButtonEnabled = false;
            newItem.Header = "New Document";
            NewDocumentPage page = new NewDocumentPage(newItem);            
            Frame tabItemFrame = new Frame();
            tabItemFrame.Content = page;
            newItem.Content = tabItemFrame;

            FilesTabControl.Items.Add(newItem);
            FilesTabControl.Items.Refresh();
            Instance = this;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Instance = null;
            App.Current.Shutdown();
            
        }

        private void DrawRectangleButton_Click(object sender, RoutedEventArgs e)
        {
            m_ignoreDrawingGeometries = false;
            m_drawableType = DrawableTypes.Rectangle;
        }

        private void DrawEllipseButton_Click(object sender, RoutedEventArgs e)
        {
            m_ignoreDrawingGeometries = false;
            m_drawableType = DrawableTypes.Ellipse;
        }

        private void DrawLineButton_Click(object sender, RoutedEventArgs e)
        {
            m_ignoreDrawingGeometries = false;
            m_drawableType = DrawableTypes.Line;
        }

        private void SelectItemButton_Click(object sender, RoutedEventArgs e)
        {
            DrawableType = DrawableTypes.None;
            m_ignoreDrawingGeometries = true;
        }

        private void SelectionOfObject(object sender, PropertyChangedEventArgs args)
        {
            Trace.WriteLine("DebugLogOfSelection" + m_selectedObject.ObserwableObject.Data);
            switch (m_selectedObject.ObserwableObject.Data)
            {
                case RectangleGeometry:
                    {
                        SelectedObjectString = "Rectangle/Square";
                        break;
                    }
                case LineGeometry:
                    {
                        SelectedObjectString = "Line";
                        break;
                    }
                case EllipseGeometry:
                    {
                        SelectedObjectString = "Ellipse/Cricle";
                        break;
                    }
                case null:
                    {
                        SelectedObjectString = "No Object Selected";
                        break;
                    }
            }
        }

        private void FilesTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            foreach (var item in e.AddedItems)
                (item as TabItem).Background = ColorsReference.selectedTabItemBackground;
            foreach (var item in e.RemovedItems)
                (item as TabItem).Background = ColorsReference.notSelectedTabItemBackground;
        }
    }
}
 
