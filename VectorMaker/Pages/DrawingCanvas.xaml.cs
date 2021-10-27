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
using System.IO;
using System.Drawing.Printing;

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
                catch(Exception e)
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
                        if(Keyboard.Modifiers == ModifierKeys.Control)
                        {
                            SaveFile();
                        }
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
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    saveFileDialog.Filter = "Microsoft XAML File (*.xaml) | *.xaml";
                    saveFileDialog.FileName = "untilted.xaml";
                    Nullable<bool> result = saveFileDialog.ShowDialog();
                    if (result == true)
                    {
                        m_filePath = saveFileDialog.FileName;
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
                   XamlWriter.Save(MainCanvas,fileStream);
                }
                return true;
            }
            catch(Exception e)
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
            catch(PrintDialogException e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }

        public bool SaveAsPNG()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            saveFileDialog.Filter = "Portable Network Graphics (*.png) | *.png";
            saveFileDialog.FileName = "untilted.png";
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                RenderTargetBitmap renderBitmap = new RenderTargetBitmap((int)Page.Width, (int)Page.Height, 96d, 96d, PixelFormats.Pbgra32);
                renderBitmap.Render(MainCanvas);
                using (FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    // Use png encoder for our data
                    PngBitmapEncoder encoder = new PngBitmapEncoder();
                    // push the rendered bitmap to it
                    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                    // save the data to the stream
                    encoder.Save(fileStream);
                }
            }
            return false;
        }
    }
}
