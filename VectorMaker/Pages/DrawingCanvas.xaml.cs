using System.Windows.Controls;
using VectorMaker.Drawables;
using System.Xml.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Markup;
using Microsoft.Win32;
using System;
using System.Windows.Input;
using VectorMaker.Utility;
using System.Windows.Media;
using System.Windows.Documents;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Drawing.Printing;
using System.Linq;
using FileStream = System.IO.FileStream;
using File = System.IO.File;
using FileMode = System.IO.FileMode;

namespace VectorMaker.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy Page1.xaml
    /// </summary>
    public partial class DrawingCanvas : Page, INotifyPropertyChanged
    {
        private Canvas m_mainCanvas;
        private Point m_positionInCanvas;
        private List<Shape> m_listOfShapes;
        private bool m_wasFirstDown = false;
        private Drawable m_drawableObject;
        private PathSettings m_pathSettings;
        private bool m_isSaved = true;
        private string m_filePath;
        private ObservableCollection<ResizingAdorner> m_selectedObjects;
        public ViewportController ViewportController;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ResizingAdorner> SelectedObjects => m_selectedObjects;


        public bool IsSaved => m_isSaved;

        public DrawingCanvas()
        {
            InitializeComponent();
            SetProperties();
        }

        public DrawingCanvas(string fileName)
        {
            InitializeComponent();
            SetProperties();
            m_filePath = fileName;
            if (System.IO.Path.GetExtension(fileName) == ".svg")
            {
                XDocument document = SVG_XAML_Converter_Lib.SVG_To_XAML.ConvertSVGToXamlCode(fileName);

                if (document != null)
                {
                    object path = XamlReader.Parse(document.ToString());
                    m_mainCanvas.Children.Add(path as UIElement);
                    m_isSaved = false;
                }
            }
            else
            {
                try
                {
                    using (FileStream fileStream = File.Open(fileName, FileMode.Open))
                    {
                        object loadedFile = XamlReader.Load(fileStream);
                        //object path = XamlReader.Parse(loadedFile.ToString());
                        m_mainCanvas.Children.Add(loadedFile as UIElement);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }

            }
        }

        private void SetProperties()
        {
            m_mainCanvas = MainCanvas;
            //m_mainCanvasBorder = MainCanvasBorder;
            m_positionInCanvas = new Point(0, 0);
            m_listOfShapes = new List<Shape>();
            m_pathSettings = new PathSettings();
            ViewportController = new ViewportController(ScaleParent, ZoomScrollViewer);
            m_selectedObjects = new ObservableCollection<ResizingAdorner>();
            m_selectedObjects.CollectionChanged += MainWindow.Instance.ObjectTransforms.SelectedObjectsChanged;
        }

        private void MainCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            EndDrawing();
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!MainWindow.Instance.IgnoreDrawingGrometries)
            {
                if (!m_wasFirstDown)
                {
                    m_wasFirstDown = true;
                    SetDrawableObject();
                    if (m_drawableObject != null)
                    {
                        var path = m_drawableObject.SetStartPoint(m_positionInCanvas);
                        m_listOfShapes.Add(path);
                        m_mainCanvas.Children.Add(path);
                        m_isSaved = false;
                        //Trace.WriteLine(m_positionInCanvas);
                    }
                }
                else
                {
                    m_drawableObject.AddPointToCollection();
                }
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!MainWindow.Instance.IgnoreDrawingGrometries && m_drawableObject != null)
            {
                if (MainWindow.Instance.DrawableType != DrawableTypes.Polygon &&
                    MainWindow.Instance.DrawableType != DrawableTypes.PolyLine)
                    EndDrawing();
            }
            else if (MainWindow.Instance.DrawableType != DrawableTypes.None)
                SelectionTest(e.GetPosition(m_mainCanvas));
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            m_positionInCanvas.X = e.GetPosition(m_mainCanvas).X;
            m_positionInCanvas.Y = e.GetPosition(m_mainCanvas).Y;
            if (m_wasFirstDown && m_drawableObject != null)
            {
                m_drawableObject.SetValueOfPoint(m_positionInCanvas);
                m_mainCanvas.InvalidateVisual();
            }
        }

        private void ZoomScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ViewportController.ScrollViewerChanged(e);
        }

        private void ZoomScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ViewportController.MouseWheel(e);
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void File_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                Image image = new Image();
                BitmapImage bitmapImage = new BitmapImage();
                try
                {
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(files[0]);
                    bitmapImage.EndInit();

                    if (bitmapImage != null)
                    {
                        Point point = e.GetPosition(MainCanvas);
                        image.Source = bitmapImage;
                        image.Stretch = Stretch.Fill;
                        image.Width = 200;
                        image.RenderTransform = new TranslateTransform(point.X, point.Y);
                        MainCanvas.Children.Add(image);
                        MainCanvas.InvalidateVisual();
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }

            }
        }

        private void MainCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    {
                        DeleteSelectedObjects();
                        break;
                    }
                case Key.Enter:
                    {
                        EndDrawing();
                        break;
                    }
                case Key.S:
                    {
                        if (Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            SaveFile();
                        }
                        break;
                    }
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    {
                        if (m_drawableObject != null)
                            m_drawableObject.IsControlKey = true;
                        break;
                    }
                case Key.PageDown:
                    {
                        ChangeZIndex(true);
                        break;
                    }
                case Key.PageUp:
                    {
                        ChangeZIndex(false);
                        break;
                    }

            }
        }
        private void MainCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.LeftCtrl:
                case Key.RightCtrl:
                    {
                        if (m_drawableObject != null)
                            m_drawableObject.IsControlKey = false;
                        break;
                    }
            }
        }

        private void DeleteSelectedObjects()
        {
            if (m_selectedObjects.Count > 0)
            {
                foreach (ResizingAdorner resizingAdorner in m_selectedObjects)
                {
                    resizingAdorner.RemoveFromAdornerLayer();
                    MainCanvas.Children.Remove(resizingAdorner.AdornedElement);
                }
                m_selectedObjects.Clear();
            }
        }
        private void ChangeZIndex(bool isIncreasing)
        {
            if (m_selectedObjects.Count > 0)
            {
                foreach (ResizingAdorner resizingAdorner in m_selectedObjects)
                {
                    if (isIncreasing)
                        Canvas.SetZIndex(resizingAdorner.AdornedElement, Canvas.GetZIndex(resizingAdorner.AdornedElement) + 1);
                    else
                        Canvas.SetZIndex(resizingAdorner.AdornedElement, Canvas.GetZIndex(resizingAdorner.AdornedElement) - 1);
                }
            }
        }

        private void EndDrawing()
        {
            if (m_drawableObject != null)
            {
                m_wasFirstDown = false;
                m_drawableObject.EndDrawing();
                m_drawableObject = null;
            }
        }

        private void SetDrawableObject()
        {
            switch (MainWindow.Instance.DrawableType)
            {
                case DrawableTypes.Ellipse:
                    {
                        m_drawableObject = new DrawableEllipse(m_pathSettings);
                        break;
                    }
                case DrawableTypes.Rectangle:
                    {
                        m_drawableObject = new DrawableRectangle(m_pathSettings);
                        break;
                    }
                case DrawableTypes.Line:
                    {
                        m_drawableObject = new DrawableLine(m_pathSettings);
                        break;
                    }
                case DrawableTypes.None:
                    {
                        m_drawableObject = null;
                        break;
                    }
                case DrawableTypes.PolyLine:
                    {
                        m_drawableObject = new DrawablePolyline(m_pathSettings);
                        break;
                    }
                case DrawableTypes.Polygon:
                    {
                        m_drawableObject = new DrawablePolygon(m_pathSettings);
                        break;
                    }
            }
        }

        private void SelectionTest(Point e)
        {
            if (!(Keyboard.Modifiers == ModifierKeys.Shift))
            {
                foreach (ResizingAdorner adorner in m_selectedObjects)
                {
                    adorner.RemoveFromAdornerLayer();
                }
                m_selectedObjects.Clear();
            }

            VisualTreeHelper.HitTest(MainCanvas, null,
                HitTestResultCallback,
                new PointHitTestParameters(e));
        }

        private HitTestResultBehavior HitTestResultCallback(HitTestResult result)
        {
            PointHitTestResult testResult = result as PointHitTestResult;
            if ((testResult.VisualHit as FrameworkElement).Parent != ScaleParent)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(testResult.VisualHit);
                ResizingAdorner adorner = new ResizingAdorner(testResult.VisualHit as UIElement, adornerLayer, MainCanvas);
                adornerLayer.Add(adorner);
                m_selectedObjects.Add(adorner);
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
        }

        public bool SaveFile()
        {
            if (!m_isSaved)
            {
                if (m_filePath == "")
                {
                    bool result = OpenSaveDialog("Microsoft XAML File (*.xaml) | *.xaml", "untilted.xaml", out string filePath);
                    if (result == true)
                    {
                        m_filePath = filePath;
                        m_isSaved = true;
                        return SaveStreamToXAML(m_filePath);
                    }
                    return false;
                }
                return SaveStreamToXAML(m_filePath);
            }
            return true;
        }

        private bool SaveStreamToXAML(string filePath)
        {
            try
            {
                using (FileStream fileStream = File.OpenWrite(filePath))
                {
                    XamlWriter.Save(MainCanvas, fileStream);
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        public bool SaveAsPDF()
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                //Nullable<bool> result = printDialog.ShowDialog();
                PrinterSettings printerSettings = new PrinterSettings();
                printerSettings.PrinterName = "Microsoft Print to PDF";

                printDialog.PrintVisual(MainCanvas, "Save graphics as PDF");
                return true;
            }
            catch (PrintDialogException e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        public bool SaveAsPNG()
        {
            bool result = OpenSaveDialog("Portable Network Graphics (*.png) | *.png", "untilted.png", out string filePath);
            if (result == true)
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                SaveWithSpecialBitmap(encoder, filePath);

            }
            return false;
        }

        public bool SaveAsBMP()
        {
            bool result = OpenSaveDialog("Bitmap file (*.bmp) | *.bmp", "untilted.bmp", out string filePath);
            if (result == true)
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                SaveWithSpecialBitmap(encoder, filePath);
            }
            return false;
        }

        public bool SaveAsJPG()
        {
            bool result = OpenSaveDialog("JPEG (*.jpeg) | *.jpeg", "untilted.jpeg", out string filePath);
            if (result == true)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                SaveWithSpecialBitmap(encoder, filePath);
            }
            return false;
        }

        public bool SaveAsTIFF()
        {
            bool result = OpenSaveDialog("TIFF (*.tiff) | *.tiff", "untilted.tiff", out string filePath);
            if (result == true)
            {
                TiffBitmapEncoder encoder = new TiffBitmapEncoder();
                SaveWithSpecialBitmap(encoder, filePath);
            }
            return false;
        }

        private bool OpenSaveDialog(string filter, string fileName, out string filePath)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            saveFileDialog.Filter = filter;
            saveFileDialog.FileName = fileName;
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                filePath = saveFileDialog.FileName;
                return true;
            }
            filePath = "";
            return false;
        }

        private void SaveWithSpecialBitmap(BitmapEncoder bitmapEncoder, string filePath)
        {
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)Page.Width, (int)Page.Height, 96d, 96d, PixelFormats.Pbgra32);
            renderBitmap.Render(MainCanvas);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                bitmapEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                bitmapEncoder.Save(fileStream);
            }
        }

        public void Union()
        {
            GeometryGroup geometryGroup = new GeometryGroup();
            foreach(ResizingAdorner adorner in  m_selectedObjects)
            {
                Shape shape = adorner.AdornedElement as Shape;
                Geometry geometry = shape?.RenderedGeometry;
                geometry.Transform = shape.RenderTransform;
                if(geometry !=null)
                {
                    geometryGroup.Children.Add(geometry);
                }
                else
                {
                    MessageBox.Show("One of selected objects is a group, please ungroup first. Operation terminated");
                    return;
                }
            }
            Path path = new Path();
            path.Data = geometryGroup;
            geometryGroup.FillRule = FillRule.EvenOdd;
            Shape lastShape = m_selectedObjects.Last().AdornedElement as Shape;
            
            path.Style = lastShape.Style;
            path.Fill = lastShape.Fill;
            path.Stroke = lastShape.Stroke;
            //path.RenderTransform = lastShape.RenderTransform;
            MainCanvas.Children.Add(path);
            DeleteSelectedObjects();
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(path);
            ResizingAdorner newAdorner = new ResizingAdorner(path, adornerLayer, MainCanvas);
            adornerLayer.Add(newAdorner);
            m_selectedObjects.Add(newAdorner);
        }

        public void Exclude()
        {

        }

        public void Xor()
        {

        }

        public void Intersect()
        {

        }
    }
}
