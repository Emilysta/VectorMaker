using System;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Linq;
using VectorMaker.Drawables;
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
using VectorMaker.Intefaces;
using System.Xml;
using System.Collections.Specialized;

namespace VectorMaker.ViewModel
{
    internal class DrawingCanvasViewModel : DocumentViewModelBase
    {
        #region Fields
        private Point m_positionInCanvas;
        private Point m_startPositionInCanvas;
        private bool m_isFileToBeLoaded = false;
        private bool m_wasFirstDown = false;
        private FileType[] m_filters = new FileType[] { FileType.SVG, FileType.PNG, FileType.PDF, FileType.BMP, FileType.JPEG, FileType.TIFF };
        private string m_defaultExtension = "xaml";
        private Drawable m_drawableObject;
        private Shape m_drawableObjectShape;
        private PathSettings m_pathSettings = PathSettings.Default();
        private Canvas m_mainCanvas;
        private ObservableCollection<ResizingAdorner> m_selectedObjects = new ();
        private IMainWindowViewModel m_interfaceMainWindowVM;
        private ObservableCollection<LayerItemViewModel> m_layers;
        private int m_layersCount = 1;
        private LayerItemViewModel m_selectedLayer;
        #endregion

        #region Properties
        public Canvas MainCanvas
        {
            get => m_mainCanvas;
            set
            {
                m_mainCanvas = value;
                if (m_isFileToBeLoaded)
                    LoadFile();
                else
                {
                    m_mainCanvas.Children.Add(SelectedLayer.Layer);
                }
            }
        }
        public ObservableCollection<LayerItemViewModel> Layers
        {
            get => m_layers;
            set
            {
                if (m_layers != null)
                    m_layers.CollectionChanged -= LayersCollectionChanged;
                m_layers = value;
                m_layers.CollectionChanged += LayersCollectionChanged;
                OnPropertyChanged(nameof(Layers));
            }
        }

        public int LayersNumber
        {
            get => m_layersCount;
            set
            {
                m_layersCount = value;
                OnPropertyChanged(nameof(LayersNumber));
            }
        }

        public LayerItemViewModel SelectedLayer
        {
            get => m_selectedLayer;
            set
            {
                m_selectedLayer = value;
                OnPropertyChanged(nameof(SelectedLayer));
            }
        }

        public bool IgnoreDrawing => DrawableType == DrawableTypes.None;
        public bool IsOneObjectSelected
        {
            get
            {
                return SelectedObjects.Count == 1;
            }
        }
        public ObservableCollection<ResizingAdorner> SelectedObjects
        {
            get => m_selectedObjects;
            set
            {
                m_selectedObjects = value;
                OnPropertyChanged(nameof(SelectedObjects));
            }
        }
        public DrawableTypes DrawableType { get; set; }
        public static Configuration AppConfiguration => Configuration.Instance;
        protected override FileType[] Filters { get => m_filters; set => m_filters = value; }
        protected override string DefaultExtension { get => m_defaultExtension; set => m_defaultExtension = value; }
        #endregion

        #region Commands

        #region Actions Commands
        public ICommand UnionCommand { get; set; }
        public ICommand ExcludeCommand { get; set; }
        public ICommand XorCommand { get; set; }
        public ICommand IntersectCommand { get; set; }
        #endregion

        #region Eventc Commands
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
        public DrawingCanvasViewModel(IMainWindowViewModel mainWindowViewModel)
        {
            m_interfaceMainWindowVM = mainWindowViewModel;
            SetCommands();
            IsSaved = false;
            Layers = new ObservableCollection<LayerItemViewModel>() { new LayerItemViewModel(new Canvas(), 1, "Layer_1") };
            SelectedLayer = Layers[0];
        }

        public DrawingCanvasViewModel(string filePath, IMainWindowViewModel mainWindowViewModel)
        {
            m_interfaceMainWindowVM = mainWindowViewModel;
            FilePath = filePath;
            m_isFileToBeLoaded = true;
            Layers = new ObservableCollection<LayerItemViewModel>();
            SetCommands();
        }

        private void LayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                m_layersCount++;
        }
        #endregion

        #region Methods

        private void SetCommands()
        {
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
        private void LoadFile()
        {
            try
            {
                if (System.IO.Path.GetExtension(FilePath) == ".svg")
                {
                    XDocument document = SVG_XAML_Converter_Lib.SVG_To_XAML.ConvertSVGToXamlCode(FilePath);

                    if (document != null)
                    {
                        object path = XamlReader.Parse(document.ToString());
                        MainCanvas.Children.Add(path as UIElement); //toDo Check if MainCanvas
                        IsSaved = false; //toDo load layers
                        //LoadLayers();
                    }
                }
                else
                {
                    using (FileStream fileStream = File.Open(FilePath, FileMode.Open))
                    {
                        object loadedFile = XamlReader.Load(fileStream);
                        Canvas canvas = loadedFile as Canvas;
                        MainCanvas.Children.Add(loadedFile as UIElement);
                        LoadLayers(canvas);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }

        private void LoadLayers(Canvas loadedMainCanvas)
        {
            int i = 1;
            foreach(Canvas canvas in loadedMainCanvas.Children.OfType<Canvas>())
            {
                if((string)canvas.Tag == "Layer")
                {
                    LayerItemViewModel layer = new(canvas,i,canvas.Name);
                    layer.DeleteAction = (layer) =>
                    {
                        Layers.Remove(layer);
                        SelectedLayer = Layers.Last();
                        loadedMainCanvas.Children.Remove(layer.Layer);
                    };
                    Layers.Add(layer);
                    i++;
                }
            }
            LayersNumber = Layers.Count;
            if(Layers.Count!=0)
                SelectedLayer = Layers[0];
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
                IsSaved = false;
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
            if ((testResult.VisualHit as FrameworkElement).Parent is Canvas)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(testResult.VisualHit);
                ResizingAdorner adorner = new (testResult.VisualHit as UIElement, adornerLayer, m_mainCanvas);
                adornerLayer.Add(adorner);
                SelectedObjects.Add(adorner);
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
        }

        private void Union()
        {
            GeometryGroup geometryGroup = new();
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
            System.Windows.Shapes.Path path = new();
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
            ResizingAdorner newAdorner = new(path, adornerLayer, m_mainCanvas);
            adornerLayer.Add(newAdorner);
            SelectedObjects.Add(newAdorner);
        }
        private void Exclude()
        {
            throw new NotImplementedException();
        }
        private void Xor()
        {
            throw new NotImplementedException();
        }
        private void Intersect()
        {
            throw new NotImplementedException();
        }
        private void MouseLeftButtonDownHandler(MouseButtonEventArgs e)
        {
            if (!IgnoreDrawing)
            {
                if (!m_wasFirstDown)
                {
                    m_wasFirstDown = true;
                    m_startPositionInCanvas = m_positionInCanvas;
                    SetDrawableObject();
                    if (m_drawableObject != null)
                    {
                        m_drawableObjectShape = m_drawableObject.SetStartPoint(m_positionInCanvas);
                        SelectedLayer.Layer.Children.Add(m_drawableObjectShape);
                        //OnPropertyChanged("Children");
                        IsSaved = false;
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
                {
                    EndDrawing();
                    if (m_startPositionInCanvas.Equals(m_positionInCanvas))
                    {
                        SelectedLayer.Layer.Children.Remove(m_drawableObjectShape);
                        IsSaved = true;
                    }
                }

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
                        ChangeZIndex(false);
                        break;
                    }
                case Key.PageUp:
                    {
                        ChangeZIndex(true);
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

                Image image = new();
                BitmapImage bitmapImage = new();
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
                        SelectedLayer.Layer.Children.Add(image);
                        IsSaved = false;
                    }
                }
                catch (Exception exp)
                {
                    MessageBox.Show(exp.Message);
                }

            }
        }
        protected override void SaveFile()
        {
            if (!IsSaved)
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    bool result = OpenSaveDialog("Microsoft XAML File (*.xaml) | *.xaml", "untilted.xaml", out string filePath);
                    if (result == true)
                    {
                        FilePath = filePath;
                        IsSaved = true;
                        SaveStreamToXAML(FilePath);
                    }
                    return;
                }
                bool saved = SaveStreamToXAML(FilePath);
                if (saved)
                    IsSaved = true;
            }
            return;
        }
        protected override void CloseFile()
        {
            if (!IsSaved)
            {
                var result = MessageBox.Show(string.Format("Do you want to save changes " +
                    "for file '{0}'?", FileName), "VectorMaker",
                    MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Cancel)
                    return;

                if (result == MessageBoxResult.Yes)
                {
                    SaveFile();
                }
            }
            m_interfaceMainWindowVM.Close(this);
        }
        protected override void SaveFileAsPDF(string fullFilePath)
        {
            try
            {
                PrintDialog printDialog = new();
                //Nullable<bool> result = printDialog.ShowDialog();
                PrinterSettings printerSettings = new();
                printerSettings.PrinterName = "Microsoft Print to PDF";
                printDialog.PrintVisual(m_mainCanvas, "Save graphics as PDF");

            }
            catch (PrintDialogException e)
            {
                MessageBox.Show(e.Message);
            }
        }
        protected override void SaveFileAsPNG(string fullFilePath)
        {
            PngBitmapEncoder encoder = new();
            SaveWithSpecialBitmap(encoder, fullFilePath);
        }
        protected override void SaveFileAsBMP(string fullFilePath)
        {
            BmpBitmapEncoder encoder = new();
            SaveWithSpecialBitmap(encoder, fullFilePath);
        }
        protected override void SaveFileAsJPEG(string fullFilePath)
        {
            JpegBitmapEncoder encoder = new();
            SaveWithSpecialBitmap(encoder, fullFilePath);
        }
        protected override void SaveFileAsTIFF(string fullFilePath)
        {
            TiffBitmapEncoder encoder = new();
            SaveWithSpecialBitmap(encoder, fullFilePath);
        }
        private bool OpenSaveDialog(string filter, string fileName, out string filePath)
        {
            SaveFileDialog saveFileDialog = new();
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
            RenderTargetBitmap renderBitmap = new(300, 300, 96d, 96d, PixelFormats.Pbgra32);
            renderBitmap.Render(m_mainCanvas);
            using (FileStream fileStream = new(filePath, FileMode.Create))
            {
                bitmapEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                // save the data to the stream
                bitmapEncoder.Save(fileStream);
            }
        }
        private bool SaveStreamToXAML(string filePath)
        {
            try
            {
                using (FileStream fileStream = File.OpenWrite(filePath))
                {
                    fileStream.SetLength(0);
                    XmlTextWriter xmlTextWriter = new(fileStream, System.Text.Encoding.UTF8);
                    xmlTextWriter.Formatting = Formatting.Indented;
                    XamlWriter.Save(m_mainCanvas, xmlTextWriter);
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }
        #endregion
    }
}
