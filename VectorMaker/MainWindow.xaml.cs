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
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Linq;

namespace VectorMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ObservableCollection<DrawingCanvas> Documents { get; set; }
        
        public void SetDrawableType(DrawableTypes type)
        {
            foreach (DrawingCanvas canvas in Documents)
            {
                canvas.ViewModel.DrawableType = type;
            }
        }

        public static MainWindow Instance { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
            Instance = this;
            Documents = new ObservableCollection<DrawingCanvas>();
            Documents.Add(new DrawingCanvas());
            this.DataContext = this;
            //DocumentManager.RunOpenVisibilityCheck();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            Instance = null;
            App.Current.Shutdown();
        }

        private void DrawRectangleButton_Click(object sender, RoutedEventArgs e)
        {
            SetDrawableType(DrawableTypes.Rectangle);
        }

        private void DrawEllipseButton_Click(object sender, RoutedEventArgs e)
        {
            SetDrawableType(DrawableTypes.Ellipse);
        }

        private void DrawLineButton_Click(object sender, RoutedEventArgs e)
        {
            SetDrawableType(DrawableTypes.Line);
        }

        private void SelectItemButton_Click(object sender, RoutedEventArgs e)
        {
            SetDrawableType(DrawableTypes.None);
        }

        private void DrawPolylineButton_Click(object sender, RoutedEventArgs e)
        {
            SetDrawableType(DrawableTypes.PolyLine);
        }

        private void DrawPolygonButton_Click(object sender, RoutedEventArgs e)
        {
            SetDrawableType(DrawableTypes.Polygon);
        }

        private void UnionButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame frame = DockingManager.ActiveContent as Frame;
            //DrawingCanvas drawingCanvas = frame.Content as DrawingCanvas;
            //drawingCanvas.Union();
        }

        private void NewDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            Documents.Add(new DrawingCanvas());
        }

        private void OpenDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Scalable Vector Graphics (*.svg) | *.svg | Extensible Application Markup Language (*.xaml) | *.xaml";
            if (openFileDialog.ShowDialog() == true)
                Documents.Add(new DrawingCanvas(openFileDialog.FileName));
        }

        private void SaveDocumentButton_Click(object sender, RoutedEventArgs e)
        {
           // m_drawingCanvas.SaveFile();
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
            //Frame frame = DockingManager.ActiveContent as Frame;
            //DrawingCanvas drawingCanvas = frame.Content as DrawingCanvas;
            //drawingCanvas.SaveAsPNG();
        }

        private void SaveAsBMPDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            //m_drawingCanvas.SaveAsPDF();
            LayoutContent layoutContent = DocumentPaneGroup.SelectedContent;
            //DocumentPaneGroup.Active
        }

        private void SaveAsPDFDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame frame = DockingManager.ActiveContent as Frame;
            //DrawingCanvas drawingCanvas = frame.Content as DrawingCanvas;
            //drawingCanvas.SaveAsPDF();
        }

        private void TransformToolMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //if (TransformTool.IsHidden)
            //    TransformTool.Show();
            //else if (TransformTool.IsVisible)
            //    TransformTool.IsActive = true;
            //else
            //    TransformTool.AddToLayout(DockingManager, AnchorableShowStrategy.Right | AnchorableShowStrategy.Most);
        }

        private void PropertiesToolMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //if (PropertiesTool.IsHidden)
            //    PropertiesTool.Show();
            //else if (PropertiesTool.IsVisible)
            //    PropertiesTool.IsActive = true;
            //else
            //    PropertiesTool.AddToLayout(DockingManager, AnchorableShowStrategy.Right | AnchorableShowStrategy.Most);
        }

        private void ColorPickerMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //if (ColorPicker.IsHidden)
            //    ColorPicker.Show();
            //else if (ColorPicker.IsVisible)
            //    ColorPicker.IsActive = true;
            //else
            //    ColorPicker.AddToLayout(DockingManager, AnchorableShowStrategy.Right | AnchorableShowStrategy.Most);
        }

        private void AlignmentToolMenuItem_Click(object sender, RoutedEventArgs e)
        {
            //if (AlignmentTool.IsHidden)
            //    AlignmentTool.Show();
            //else if (AlignmentTool.IsVisible)
            //    AlignmentTool.IsActive = true;
            //else
            //    AlignmentTool.AddToLayout(DockingManager, AnchorableShowStrategy.Right | AnchorableShowStrategy.Most);
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

        public void ChangeColor()
        {
            System.Windows.Media.Color accentColor = ColorsReference.magentaBaseColor;
            Application.Current.Resources[AvalonDock.Themes.VS2013.Themes.ResourceKeys.ControlAccentColorKey] = accentColor;
            Application.Current.Resources[AvalonDock.Themes.VS2013.Themes.ResourceKeys.ControlAccentBrushKey] = new SolidColorBrush(accentColor);
        }
    }
}

