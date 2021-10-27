using System.Windows;
using System.Windows.Media;
using VectorMaker.Drawables;
using System;
using System.Windows.Shapes;
using VectorMaker.Utility;
using AvalonDock.Layout;
using System.Windows.Controls;
using VectorMaker.Pages;
using System.Diagnostics;

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
        private DrawingCanvas m_drawingCanvas;
        
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
            Instance = this;
            DocumentManager.RunOpenVisibilityCheck();
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

        private void DrawPolylineButton_Click(object sender, RoutedEventArgs e)
        {
            m_ignoreDrawingGeometries = false;
            m_drawableType = DrawableTypes.PolyLine;
        }

        private void DrawPolygonButton_Click(object sender, RoutedEventArgs e)
        {
            m_ignoreDrawingGeometries = false;
            m_drawableType = DrawableTypes.Polygon;
        }

        private void NewDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            DocumentManager.OpenNewDocumentTab();
        }

        private void OpenDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            if (!DocumentManager.OpenExistingDocumentTab())
            {
                //toDo info
            }
        }

        private void SaveDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            m_drawingCanvas.SaveFile();
        }

        private void SaveAllDocumentsButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void SaveAsSVGDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            //m_drawingCanvas.SaveAsPDF();
        }
        private void SaveAsPNGDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            Frame frame = DockingManager.ActiveContent as Frame;
            DrawingCanvas drawingCanvas = frame.Content as DrawingCanvas;
            drawingCanvas.SaveAsPNG();
        }
        private void SaveAsBMPDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            //m_drawingCanvas.SaveAsPDF();
            LayoutContent layoutContent = DocumentPaneGroup.SelectedContent;
            //DocumentPaneGroup.Active
        }

        private void SaveAsPDFDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            Frame frame = DockingManager.ActiveContent as Frame;
            DrawingCanvas drawingCanvas = frame.Content as DrawingCanvas;
            drawingCanvas.SaveAsPDF();
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

        private void MinimizeApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeRestoreApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            if (MaximizeApplicationButton.IsChecked == true)
            {
                WindowState = WindowState.Maximized;
                MaximizeApplicationButton.IconKind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.WindowRestoreRegular;
            }
            else
            {
                WindowState = WindowState.Normal;
                MaximizeApplicationButton.IconKind = MahApps.Metro.IconPacks.PackIconFontAwesomeKind.WindowMaximizeRegular;
            }
        }

        private void ExitApplicationButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}

