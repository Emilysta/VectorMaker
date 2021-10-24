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
using VectorMaker.Utility;
using System.Windows.Documents;
using AvalonDock;
using System.Linq;
using AvalonDock.Layout;

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
            Instance = this;
            TabControlManager.RunOpenVisibilityCheck();
            ChangeColor();
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
            DrawableType = DrawableTypes.SelectionTool;
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

        private void NewDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            TabControlManager.OpenNewDocumentTab();
        }
        private void OpenDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            if (!TabControlManager.OpenExistingDocumentTab())
            {
                //toDo info
            }
        }

        private void SaveDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame frame = FilesTabControl.SelectedContent as Frame;
            //DrawingCanvas drawingCanvas = frame?.Content as DrawingCanvas;
            //if (drawingCanvas != null)
            //{
            //    if (!drawingCanvas.SaveToFile())
            //    {
            //        //toDo open Dialog with warning
            //    }
            //}
        }

        private void SaveAllDocumentsButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void TransformToolMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (TransformTool.IsHidden)
                TransformTool.Show();
            else if (TransformTool.IsVisible)
                TransformTool.IsActive = true;
            else
                TransformTool.AddToLayout(DockingManager, AnchorableShowStrategy.Right | AnchorableShowStrategy.Most);
        }

        private void PropertiesToolMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (PropertiesTool.IsHidden)
                PropertiesTool.Show();
            else if (PropertiesTool.IsVisible)
                PropertiesTool.IsActive = true;
            else
                PropertiesTool.AddToLayout(DockingManager, AnchorableShowStrategy.Right | AnchorableShowStrategy.Most);
        }

        private void ColorPickerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ColorPicker.IsHidden)
                ColorPicker.Show();
            else if (ColorPicker.IsVisible)
                ColorPicker.IsActive = true;
            else
                ColorPicker.AddToLayout(DockingManager, AnchorableShowStrategy.Right | AnchorableShowStrategy.Most);
        }

        public void ChangeColor()
        {
            System.Windows.Media.Color accentColor = ColorsReference.magentaBaseColor;
            Application.Current.Resources[AvalonDock.Themes.VS2013.Themes.ResourceKeys.ControlAccentColorKey] = accentColor;
            Application.Current.Resources[AvalonDock.Themes.VS2013.Themes.ResourceKeys.ControlAccentBrushKey] = new SolidColorBrush(accentColor);
        }
    }
}

