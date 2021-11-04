using System;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Linq;
using VectorMaker.Drawables;
using VectorMaker.Model;
using FileStream = System.IO.FileStream;
using File = System.IO.File;
using FileMode = System.IO.FileMode;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using VectorMaker.Utility;
using System.Windows.Shapes;
using System.Windows.Documents;
using System.Drawing.Printing;
using System.Linq;
using VectorMaker.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace VectorMaker.ViewModel
{
    public class DrawingCanvasViewModel : NotifyPropertyChangedBase
    {
        public string Title => Model.FileName;
        public DrawingCanvasModel Model { get; set; } = new DrawingCanvasModel();
        private Point m_positionInCanvas;
        private bool m_wasFirstDown = false;
        private Drawable m_drawableObject;
        private PathSettings m_pathSettings = PathSettings.Default();
        private Canvas m_mainCanvas;
        private ObservableCollection<ResizingAdorner> m_selectedObjects = new ObservableCollection<ResizingAdorner>();

        public Canvas MainCanvas
        {
            get => m_mainCanvas;
            set { m_mainCanvas = value; }
        }
        public bool IgnoreDrawing => DrawableType == DrawableTypes.None;

        public bool IsOneObjectSelected
        {
            get
            {
                return SelectedObjects.Count == 1 ? true : false;
            }
        }

        public ObservableCollection<ResizingAdorner> SelectedObjects
        {
            get => m_selectedObjects;
            set
            {
                m_selectedObjects = value;
                OnPropertyChanged("SelectedObjects");
            }
        }

        public DrawableTypes DrawableType { get; set; }

        #region Commands

        #region SaveCommands
        public ICommand SaveCommand { get; set; }
        public ICommand SaveAsBMPCommand { get; set; }
        public ICommand SaveAsPDFCommand { get; set; }
        public ICommand SaveAsJPEGCommand { get; set; }
        public ICommand SaveAsSVGCommand { get; set; }
        public ICommand SaveAsPNGCommand { get; set; }
        public ICommand SaveAsTIFFCommand { get; set; }

        #endregion

        public ICommand UnionCommand { get; set; }
        public ICommand ExcludeCommand { get; set; }
        public ICommand XorCommand { get; set; }
        public ICommand IntersectCommand { get; set; }

        #region Events
        public ICommand MouseLeftUpCommand { get; set; }
        public ICommand MouseLeftDownCommand { get; set; }
        public ICommand MouseMoveCommand { get; set; }
        public ICommand MouseLeaveCommand { get; set; }
        public ICommand PreviewKeyDownCommand { get; set; }
        public ICommand PreviewKeyUpCommand { get; set; }
        public ICommand DropCommand { get; set; }
        #endregion

        #endregion

        #region Constructors
        public DrawingCanvasViewModel()
        {
            SetCommands();
        }

        public DrawingCanvasViewModel(string filePath)
        {
            Model.FilePath = filePath;
            LoadFile(filePath);
            SetCommands();
        }
        #endregion
        private void SetCommands()
        {
            SaveCommand = new CommandBase((obj) => SaveAsBMP());
            SaveAsBMPCommand = new CommandBase((obj) => SaveAsBMP());
            SaveAsPDFCommand = new CommandBase((obj) => SaveAsPDF());
            SaveAsJPEGCommand = new CommandBase((obj) => SaveAsJPG());
            //SaveAsSVGCommand = new CommandBase((obj) => SaveAsBMP());
            SaveAsPNGCommand = new CommandBase((obj) => SaveAsPNG());
            SaveAsTIFFCommand = new CommandBase((obj) => SaveAsTIFF());

            UnionCommand = new CommandBase((obj) => Union());
            ExcludeCommand = new CommandBase((obj) => Exclude());
            XorCommand = new CommandBase((obj) => Xor());
            IntersectCommand = new CommandBase((obj) => Intersect());

            MouseLeftUpCommand = new CommandBase((obj) => MouseLeftButtonUpHandler(obj as MouseButtonEventArgs));
            MouseLeftDownCommand = new CommandBase((obj) => MouseLeftButtonDownHandler(obj as MouseButtonEventArgs));
            MouseMoveCommand = new CommandBase((obj) => MouseMoveHandler(obj as MouseEventArgs));
            MouseLeaveCommand = new CommandBase((obj) => MouseLeaveHandler(obj as MouseEventArgs));
            PreviewKeyDownCommand = new CommandBase((obj) => KeyDownHandler(obj as KeyEventArgs));
            PreviewKeyUpCommand = new CommandBase((obj) => KeyUpHandler(obj as KeyEventArgs));
            DropCommand = new CommandBase((obj) => FileDropHandler(obj as DragEventArgs));
        }
        private void LoadFile(string filePath)
        {
            try
            {
                if (System.IO.Path.GetExtension(filePath) == ".svg")
                {
                    XDocument document = SVG_XAML_Converter_Lib.SVG_To_XAML.ConvertSVGToXamlCode(filePath);

                    if (document != null)
                    {
                        object path = XamlReader.Parse(document.ToString());
                        MainCanvas.Children.Add(path as UIElement);
                        Model.IsSaved = false;
                    }
                }
                else
                {
                    using (FileStream fileStream = File.Open(filePath, FileMode.Open))
                    {
                        object loadedFile = XamlReader.Load(fileStream);
                        MainCanvas.Children.Add(loadedFile as UIElement);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        private void DeleteSelectedObjects()
        {
            //if (m_selectedObjects.Count > 0)
            //{
            //    foreach (ResizingAdorner resizingAdorner in m_selectedObjects)
            //    {
            //        resizingAdorner.RemoveFromAdornerLayer();
            //        MainCanvas.Children.Remove(resizingAdorner.AdornedElement);
            //    }
            //    m_selectedObjects.Clear();
            //}
        }
        private void ChangeZIndex(bool isIncreasing)
        {
            //if (m_selectedObjects.Count > 0)
            //{
            //    foreach (ResizingAdorner resizingAdorner in m_selectedObjects)
            //    {
            //        if (isIncreasing)
            //            Canvas.SetZIndex(resizingAdorner.AdornedElement, Canvas.GetZIndex(resizingAdorner.AdornedElement) + 1);
            //        else
            //            Canvas.SetZIndex(resizingAdorner.AdornedElement, Canvas.GetZIndex(resizingAdorner.AdornedElement) - 1);
            //    }
            //}
        }
        public void EndDrawing()
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
            switch (DrawableType)
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
                foreach (ResizingAdorner adorner in SelectedObjects)
                {
                    adorner.RemoveFromAdornerLayer();
                }
                SelectedObjects.Clear();
            }

            VisualTreeHelper.HitTest(m_mainCanvas, null,
                HitTestResultCallback,
                new PointHitTestParameters(e));
        }
        private HitTestResultBehavior HitTestResultCallback(HitTestResult result)
        {
            PointHitTestResult testResult = result as PointHitTestResult;
            if ((testResult.VisualHit as FrameworkElement).Parent == m_mainCanvas)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(testResult.VisualHit);
                ResizingAdorner adorner = new ResizingAdorner(testResult.VisualHit as UIElement, adornerLayer, m_mainCanvas);
                adornerLayer.Add(adorner);
                SelectedObjects.Add(adorner);
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
        }
        private bool SaveFile()
        {
            if (!Model.IsSaved)
            {
                if (string.IsNullOrEmpty(Model.FilePath))
                {
                    bool result = OpenSaveDialog("Microsoft XAML File (*.xaml) | *.xaml", "untilted.xaml", out string filePath);
                    if (result == true)
                    {
                        Model.FilePath = filePath;
                        Model.IsSaved = true;
                        return SaveStreamToXAML(Model.FilePath);
                    }
                    return false;
                }
                return SaveStreamToXAML(Model.FilePath);
            }
            return true;
        }
        private bool SaveStreamToXAML(string filePath)
        {
            try
            {
                using (FileStream fileStream = File.OpenWrite(filePath))
                {
                    XamlWriter.Save(m_mainCanvas, fileStream);
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }
        private bool SaveAsPDF()
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();
                //Nullable<bool> result = printDialog.ShowDialog();
                PrinterSettings printerSettings = new PrinterSettings();
                printerSettings.PrinterName = "Microsoft Print to PDF";

                printDialog.PrintVisual(m_mainCanvas, "Save graphics as PDF");
                return true;
            }
            catch (PrintDialogException e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }
        private bool SaveAsPNG()
        {
            bool result = OpenSaveDialog("Portable Network Graphics (*.png) | *.png", "untilted.png", out string filePath);
            if (result == true)
            {
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                SaveWithSpecialBitmap(encoder, filePath);

            }
            return false;
        }
        private bool SaveAsBMP()
        {
            bool result = OpenSaveDialog("Bitmap file (*.bmp) | *.bmp", "untilted.bmp", out string filePath);
            if (result == true)
            {
                BmpBitmapEncoder encoder = new BmpBitmapEncoder();
                SaveWithSpecialBitmap(encoder, filePath);
            }
            return false;
        }
        private bool SaveAsJPG()
        {
            bool result = OpenSaveDialog("JPEG (*.jpeg) | *.jpeg", "untilted.jpeg", out string filePath);
            if (result == true)
            {
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                SaveWithSpecialBitmap(encoder, filePath);
            }
            return false;
        }
        private bool SaveAsTIFF()
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
            //toDo values of width ang height 
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(300, 300, 96d, 96d, PixelFormats.Pbgra32);
            renderBitmap.Render(m_mainCanvas);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                bitmapEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                bitmapEncoder.Save(fileStream);
            }
        }
        private void Union()
        {
            GeometryGroup geometryGroup = new GeometryGroup();
            foreach (ResizingAdorner adorner in SelectedObjects)
            {
                Shape shape = adorner.AdornedElement as Shape;
                Geometry geometry = shape?.RenderedGeometry;
                geometry.Transform = shape.RenderTransform;
                if (geometry != null)
                {
                    geometryGroup.Children.Add(geometry);
                }
                else
                {
                    MessageBox.Show("One of selected objects is a group, please ungroup first. Operation terminated");
                    return;
                }
            }
            System.Windows.Shapes.Path path = new ();
            path.Data = geometryGroup;
            geometryGroup.FillRule = FillRule.EvenOdd;
            Shape lastShape = SelectedObjects.Last().AdornedElement as Shape;

            path.Style = lastShape.Style;
            path.Fill = lastShape.Fill;
            path.Stroke = lastShape.Stroke;
            //path.RenderTransform = lastShape.RenderTransform;
            MainCanvas.Children.Add(path);
            DeleteSelectedObjects();
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(path);
            ResizingAdorner newAdorner = new ResizingAdorner(path, adornerLayer, m_mainCanvas);
            adornerLayer.Add(newAdorner);
            SelectedObjects.Add(newAdorner);
        }
        private void Exclude()
        {

        }
        private void Xor()
        {

        }
        private void Intersect()
        {

        }
        private void MouseLeftButtonDownHandler(MouseButtonEventArgs e)
        {
            if (!IgnoreDrawing)
            {
                if (!m_wasFirstDown)
                {
                    m_wasFirstDown = true;
                    SetDrawableObject();
                    if (m_drawableObject != null)
                    {
                        var path = m_drawableObject.SetStartPoint(m_positionInCanvas);
                        MainCanvas.Children.Add(path);
                        OnPropertyChanged("Children");
                        Model.IsSaved = false;
                        //Trace.WriteLine(m_positionInCanvas);
                    }
                }
                else
                {
                    m_drawableObject.AddPointToCollection();
                }
            }
        }
        private void MouseLeftButtonUpHandler(MouseButtonEventArgs e)
        {
            if (!IgnoreDrawing)
            {
                if (DrawableType != DrawableTypes.Polygon &&
                    DrawableType != DrawableTypes.PolyLine)
                    EndDrawing();
            }
            else 
                SelectionTest(e.GetPosition(m_mainCanvas));
        }
        private void MouseMoveHandler(MouseEventArgs e)
        {
            m_positionInCanvas.X = e.GetPosition(m_mainCanvas).X;
            m_positionInCanvas.Y = e.GetPosition(m_mainCanvas).Y;
            if (m_wasFirstDown && m_drawableObject != null)
            {
                m_drawableObject.SetValueOfPoint(m_positionInCanvas);
            }
        }
        private void MouseLeaveHandler(MouseEventArgs e)
        {

        }
        private void KeyDownHandler(KeyEventArgs key)
        {
            switch (key.Key)
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
        private void KeyUpHandler(KeyEventArgs key)
        {
            switch (key.Key)
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
        private void FileDropHandler(DragEventArgs e)
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
                        Point point = e.GetPosition(m_mainCanvas);
                        image.Source = bitmapImage;
                        image.Stretch = Stretch.Fill;
                        image.Width = 200;
                        image.RenderTransform = new TranslateTransform(point.X, point.Y);
                        MainCanvas.Children.Add(image);
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }

            }
        }
    }
}
