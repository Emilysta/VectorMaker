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
using System.Linq;
using VectorMaker.Commands;
using System.Collections.ObjectModel;
using VectorMaker.Intefaces;
using System.Xml;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.Windows.Documents;
using System.IO.Packaging;
using System.IO;
using ShapeDef = System.Windows.Shapes;
using System.Printing;

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
        private ShapeDef.Shape m_drawableObjectShape;
        private Canvas m_mainCanvas;
        private ObservableCollection<ResizingAdorner> m_selectedObjects = new();
        private IMainWindowViewModel m_interfaceMainWindowVM;
        private ObservableCollection<LayerItemViewModel> m_layers;
        private int m_layersCount = 1;
        private LayerItemViewModel m_selectedLayer;
        private ShapePopupViewModel m_shapePopup;
        private Style m_textBoxStyle;
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

        public double Width { get; set; } = 500;
        public double Height { get; set; } = 500;

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

        public ShapePopupViewModel ShapePopupObject
        {
            get => m_shapePopup;
            set
            {
                m_shapePopup = value;
                OnPropertyChanged(nameof(ShapePopupObject));
            }
        }
        #endregion

        #region Commands

        public ICommand MouseLeftUpCommand { get; set; }
        public ICommand MouseLeftDownCommand { get; set; }
        public ICommand MouseMoveCommand { get; set; }
        public ICommand MouseLeaveCommand { get; set; }
        public ICommand PreviewKeyDownCommand { get; set; }
        public ICommand PreviewKeyUpCommand { get; set; }
        public ICommand DropCommand { get; set; }
        public ICommand GroupCommand { get; set; }
        public ICommand UngroupCommand { get; set; }

        public ICommand FlipVerticalCommand { get; set; }
        public ICommand FlipHorizontalCommand { get; set; }
        public ICommand RotateClockwiseCommand { get; set; }
        public ICommand RotateCounterclockwiseCommand { get; set; }

        #endregion

        #region Constructors
        public DrawingCanvasViewModel(IMainWindowViewModel mainWindowViewModel, double width, double height)
        {
            m_interfaceMainWindowVM = mainWindowViewModel;
            SetCommands();
            IsSaved = false;
            Layers = new ObservableCollection<LayerItemViewModel>() { new LayerItemViewModel(new Canvas(), 1, "Layer_1") };
            SelectedLayer = Layers[0];
            ShapePopupObject = new ShapePopupViewModel(mainWindowViewModel);
            Width = width;
            Height = height;
            m_textBoxStyle = (Style)Application.Current.Resources["TextBoxTransparent"];
        }

        public DrawingCanvasViewModel(string filePath, IMainWindowViewModel mainWindowViewModel)
        {
            m_interfaceMainWindowVM = mainWindowViewModel;
            FilePath = filePath;
            m_isFileToBeLoaded = true;
            Layers = new ObservableCollection<LayerItemViewModel>();
            SetCommands();
            m_textBoxStyle = (Style)Application.Current.Resources["TextBoxTransparent"];
        }

        private void LayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                m_layersCount++;
        }
        #endregion

        #region Methods
        public void CombineTwoGeometries(GeometryCombineMode mode)
        {
            if (SelectedObjects.Count == 2)
            {
                CombinedGeometry combinedGeometry = new CombinedGeometry();
                foreach (ResizingAdorner adorner in SelectedObjects)
                {
                    ShapeDef.Shape shape = adorner.AdornedElement as ShapeDef.Shape;
                    if (shape == null)
                        continue;

                    Geometry geometry = shape.RenderedGeometry;
                    if (geometry != null)
                    {
                        geometry.Transform = shape.RenderTransform;
                        if (combinedGeometry.Geometry1.IsEmpty())
                            combinedGeometry.Geometry1 = geometry;
                        else if (combinedGeometry.Geometry2.IsEmpty())
                            combinedGeometry.Geometry2 = geometry;
                    }
                    else
                    {
                        MessageBox.Show("One of selected objects is a group, please ungroup first. Operation terminated");
                        return;
                    }
                }

                combinedGeometry.GeometryCombineMode = mode;
                ShapeDef.Shape lastShape = SelectedObjects.Last().AdornedElement as ShapeDef.Shape;
                ShapeDef.Path path = new ShapeDef.Path()
                {
                    Data = combinedGeometry,
                    Style = lastShape.Style,
                    Fill = lastShape.Fill,
                    Stroke = lastShape.Stroke
                };

                Matrix geoTransform = combinedGeometry.Geometry1.Transform.Value;
                Matrix geoTransform2 = combinedGeometry.Geometry2.Transform.Value;
                Matrix translate = Matrix.Identity;

                translate.OffsetX = Math.Min(geoTransform.OffsetX, geoTransform2.OffsetX);
                translate.OffsetY = Math.Min(geoTransform.OffsetY, geoTransform2.OffsetY);


                combinedGeometry.Geometry1.Transform = new MatrixTransform(RemoveTranslation(geoTransform, translate));
                combinedGeometry.Geometry2.Transform = new MatrixTransform(RemoveTranslation(geoTransform2, translate));
                path.RenderTransform = new MatrixTransform(translate);

                DeleteSelectedObjects();
                SelectedLayer.Layer.Children.Add(path);
                CreateEditingAdorner(path);
            }
        }
        public void GroupObjects()
        {
            if (SelectedObjects.Count > 1)
            {
                Grid grid = new Grid();
                grid.Tag = "group";
                double xMin = double.MaxValue;
                double yMin = double.MaxValue;
                double xMax = double.MinValue;
                double yMax = double.MinValue;

                foreach (ResizingAdorner adorner in SelectedObjects)
                {
                    UIElement uIElement = adorner.AdornedElement;
                    xMin = Math.Min(xMin, uIElement.RenderTransform.Value.OffsetX);
                    yMin = Math.Min(yMin, uIElement.RenderTransform.Value.OffsetY);
                    xMax = Math.Max(xMax, uIElement.RenderTransform.Value.OffsetX + uIElement.RenderSize.Width);
                    yMax = Math.Max(yMax, uIElement.RenderTransform.Value.OffsetY + uIElement.RenderSize.Height);
                    SelectedLayer.Layer.Children.Remove(adorner.AdornedElement);
                    adorner.RemoveFromAdornerLayer();
                    grid.Children.Add(adorner.AdornedElement);
                }
                SelectedObjects.Clear();

                Rect rect = new Rect(xMin, yMin,xMax-xMin,yMax-yMin);
                grid.Width = rect.Width;
                grid.Height = rect.Height;
                grid.RenderSize = rect.Size;
                MatrixTransform matrixTransform = new MatrixTransform(1, 0, 0, 1, xMin, yMin);
                grid.RenderTransform = matrixTransform;
                foreach(UIElement child in grid.Children)
                {
                    child.RenderTransform = new MatrixTransform(RemoveTranslation(child.RenderTransform.Value, matrixTransform.Value));
                }
                SelectedLayer.Layer.Children.Add(grid);
                grid.Background = Brushes.Transparent;
                CreateEditingAdorner(grid);
            }
            else
            {
                MessageBox.Show("not enough objects selected");
            }
        }
        public void UngroupObjects()
        {
            if (SelectedObjects.Count == 1 && SelectedObjects[0].AdornedElement is Grid)
            {
                Grid group = SelectedObjects[0].AdornedElement as Grid;
                SelectedObjects[0].RemoveFromAdornerLayer();
                UIElementCollection collection = group.Children;
                while(collection.Count>0)
                {
                    UIElement child = collection[0];
                    collection.Remove(child);
                    SelectedLayer.Layer.Children.Add(child);
                    child.RenderTransform = new MatrixTransform(RestoreTranslation(child.RenderTransform.Value, group.RenderTransform.Value));
                    CreateEditingAdorner(child);
                }
                SelectedLayer.Layer.Children.Remove(group);
            }
            else
            {
                MessageBox.Show("too many object ora Selected Object is not a group");
            }
        }
        private void SetCommands()
        {
            MouseLeftUpCommand = new CommandBase((obj) => MouseLeftButtonUpHandler(obj as MouseButtonEventArgs));
            MouseLeftDownCommand = new CommandBase((obj) => MouseLeftButtonDownHandler(obj as MouseButtonEventArgs));
            MouseMoveCommand = new CommandBase((obj) => MouseMoveHandler(obj as MouseEventArgs));
            MouseLeaveCommand = new CommandBase((obj) => MouseLeaveHandler(obj as MouseEventArgs));
            PreviewKeyDownCommand = new CommandBase((obj) => KeyDownHandler(obj as KeyEventArgs));
            PreviewKeyUpCommand = new CommandBase((obj) => KeyUpHandler(obj as KeyEventArgs));
            DropCommand = new CommandBase((obj) => FileDropHandler(obj as DragEventArgs));
            GroupCommand = new CommandBase((_) => GroupObjects());
            UngroupCommand = new CommandBase((_) => UngroupObjects());
            FlipHorizontalCommand = new CommandBase((_) => Flip());
            FlipVerticalCommand = new CommandBase((_) => Flip(true));
            RotateClockwiseCommand = new CommandBase((_) => Rotate());
            RotateCounterclockwiseCommand = new CommandBase((_) => Rotate(false));
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
            foreach (Canvas canvas in loadedMainCanvas.Children.OfType<Canvas>())
            {
                if ((string)canvas.Tag == "Layer")
                {
                    LayerItemViewModel layer = new(canvas, i, canvas.Name);
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
            if (Layers.Count != 0)
                SelectedLayer = Layers[0];
        }
        private void DeleteSelectedObjects()
        {
            if (m_selectedObjects.Count > 0)
            {
                foreach (ResizingAdorner resizingAdorner in m_selectedObjects)
                {
                    resizingAdorner.RemoveFromAdornerLayer();
                    SelectedLayer.Layer.Children.Remove(resizingAdorner.AdornedElement);
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
                        m_drawableObject = new DrawableEllipse();
                        break;
                    }
                case DrawableTypes.Rectangle:
                    {
                        m_drawableObject = new DrawableRectangle();
                        break;
                    }
                case DrawableTypes.Line:
                    {
                        m_drawableObject = new DrawableLine();
                        break;
                    }
                case DrawableTypes.None:
                    {
                        m_drawableObject = null;
                        break;
                    }
                case DrawableTypes.PolyLine:
                    {
                        m_drawableObject = new DrawablePolyline();
                        break;
                    }
                case DrawableTypes.Polygon:
                    {
                        m_drawableObject = new DrawablePolygon();
                        break;
                    }
                case DrawableTypes.Text:
                    {
                        PathSettings pathSettings = new PathSettings();
                        pathSettings.Fill = Brushes.Transparent;
                        pathSettings.Stroke = Brushes.White;
                        pathSettings.StrokeThickness = 1;
                        m_drawableObject = new DrawableRectangle(pathSettings);
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
            if ((testResult.VisualHit as FrameworkElement !=null) && (testResult.VisualHit as FrameworkElement).Parent == SelectedLayer.Layer)
            {
                CreateEditingAdorner(testResult.VisualHit as UIElement);
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
        }

        private void CreateEditingAdorner(UIElement uiElement)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(uiElement);
            ResizingAdorner adorner = new(uiElement, adornerLayer, m_mainCanvas);
            adornerLayer.Add(adorner);
            SelectedObjects.Add(adorner);
        }

        private Matrix RemoveTranslation(Matrix target, Matrix translateToRemove)
        {
            target.OffsetX -= translateToRemove.OffsetX;
            target.OffsetY -= translateToRemove.OffsetY;
            return target;
        }

        private Matrix RestoreTranslation(Matrix target, Matrix translateToRemove)
        {
            target.OffsetX += translateToRemove.OffsetX;
            target.OffsetY += translateToRemove.OffsetY;
            return target;
        }

        private void Flip(bool isVertical = false)
        {
            foreach(ResizingAdorner adorner in SelectedObjects)
            {
                UIElement element = adorner.AdornedElement;
                Transform transform = element.RenderTransform;
                Matrix matrix = transform.Value;
                matrix.ScaleAtPrepend(isVertical ? 1 : -1, isVertical ? -1 : 1,element.RenderSize.Width/2,element.RenderSize.Height/2);
                element.RenderTransform = new MatrixTransform(matrix);
            }
        }

        private void Rotate(bool isClockwise = true)
        {
            foreach (ResizingAdorner adorner in SelectedObjects)
            {
                UIElement element = adorner.AdornedElement;
                Transform transform = element.RenderTransform;
                Matrix matrix = transform.Value;
                matrix.RotateAtPrepend(isClockwise ? 90 : -90, element.RenderSize.Width / 2, element.RenderSize.Height / 2);
                element.RenderTransform = new MatrixTransform(matrix);
            }
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
                        m_drawableObjectShape = m_drawableObject.StartDrawing(m_positionInCanvas);
                        SelectedLayer.Layer.Children.Add(m_drawableObjectShape);
                        IsSaved = false;
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
                if(DrawableType == DrawableTypes.Text)
                {
                    SelectedLayer.Layer.Children.Remove(m_drawableObjectShape);
                    Matrix matrix = m_drawableObjectShape.RenderTransform.Value;
                    TextBox textBox = new TextBox();
                    textBox.RenderTransform = new MatrixTransform(matrix);
                    textBox.RenderSize = m_drawableObjectShape.RenderSize;
                    textBox.TextWrapping= TextWrapping.Wrap;
                    textBox.AcceptsReturn = true;
                    textBox.AcceptsTab = true;
                    textBox.UndoLimit = 10;
                    textBox.Style = m_textBoxStyle;
                    textBox.Text = "Text";
                    SelectedLayer.Layer.Children.Add(textBox);
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
            //throw new NotImplementedException();
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
            //switch (key.Key)
            //{
            //    case Key.LeftCtrl:
            //    case Key.RightCtrl:
            //        {
            //            if (m_drawableObject != null)
            //                m_drawableObject.IsControlKey = false;
            //            break;
            //        }
            //}
            Trace.WriteLine("Not implemented");
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
        protected override bool SaveFile()
        {
            if (!IsSaved)
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    bool result = OpenSaveDialog("Microsoft XAML File (*.xaml) | *.xaml", "untilted.xaml", out string filePath);
                    if (result == true)
                    {
                        FilePath = filePath;
                        bool saved2 = SaveStreamToXAML(FilePath);
                        IsSaved = saved2;
                    }
                    return IsSaved;
                }
                bool saved = SaveStreamToXAML(FilePath);
                if (saved)
                    IsSaved = true;
            }
            return IsSaved;
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
                    if (SaveFile())
                        m_interfaceMainWindowVM.Close(this);
                    return;
                }
            }
            m_interfaceMainWindowVM.Close(this);

        }
        protected override void PrintFile()
        {
            PrintDialog printDialog = new PrintDialog();
            bool result = (bool)printDialog.ShowDialog();
            if(result)
            {
                printDialog.PrintVisual(MainCanvas, "VectorMaker print image");
            }
        }
        protected override void SaveFileAsPDF(string fullFilePath)
        {
            PrintTicket printTicket = new PrintTicket();
            printTicket.OutputColor = OutputColor.Color;
            printTicket.PageMediaSize = new PageMediaSize(m_mainCanvas.ActualWidth, m_mainCanvas.ActualHeight);
            printTicket.PageMediaType = PageMediaType.Photographic;
            printTicket.PageBorderless = PageBorderless.Borderless;
            printTicket.PageOrientation = PageOrientation.Portrait;
            printTicket.CopyCount = 1;
            printTicket.OutputQuality = OutputQuality.Photographic;
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                Package package = Package.Open(memoryStream, FileMode.Create);
                XpsDocument xpsDocument = new XpsDocument(package);
                XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                VisualsToXpsDocument visToXps = (VisualsToXpsDocument)xpsWriter.CreateVisualsCollator(printTicket, printTicket);
                visToXps.BeginBatchWrite();
                //visToXps.Write(m_mainCanvas,printTicket);
                visToXps.Write(m_mainCanvas);
                visToXps.EndBatchWrite();
                xpsDocument.Close();
                package.Close();
                var xpsToPDF = PdfSharp.Xps.XpsModel.XpsDocument.Open(memoryStream);
                PdfSharp.Xps.XpsConverter.Convert(xpsToPDF, fullFilePath, 0);
                memoryStream.Dispose();
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
            RenderTargetBitmap renderBitmap = new((int)m_mainCanvas.Width, (int)m_mainCanvas.Height, 96d, 96d, PixelFormats.Pbgra32);
            Size size = new Size(m_mainCanvas.Width, m_mainCanvas.Height);
            VisualBrush sourceBrush = new VisualBrush(m_mainCanvas);
            sourceBrush.Stretch = Stretch.None;
            DrawingVisual drawingVisual = new();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            using (drawingContext)
            {
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(size));
            }
            renderBitmap.Render(m_mainCanvas);
            using (FileStream fileStream = new(filePath, FileMode.Create))
            {
                bitmapEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));
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
                    xmlTextWriter.WriteComment(Configuration.Instance.GetMetadataFromConfig());
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
