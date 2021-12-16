using System;
using System.Windows;
using VectorMaker.Drawables;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VectorMaker.Utility;
using System.Linq;
using VectorMaker.Commands;
using System.Collections.ObjectModel;
using VectorMaker.Intefaces;
using System.Diagnostics;
using System.Windows.Documents;
using ShapeDef = System.Windows.Shapes;
using System.Windows.Shapes;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Threading;

namespace VectorMaker.ViewModel
{
    internal class DrawingCanvasViewModel : DrawingDocumentViewModelBase
    {
        #region Fields
        private Style m_textBoxStyle;

        /*Drawing*/
        private Point m_positionInCanvas;
        private Point m_startPositionInCanvas;
        private bool m_wasFirstDown = false;
        private Drawable m_drawableObject;
        private ShapeDef.Shape m_drawableObjectShape;

        /*Selection*/
        private ObservableCollection<ResizingAdorner> m_selectedObjects = new();

        /*Layers*/
        private LayerItemViewModel m_selectedLayer;
        private long m_countOfObjectsInLayer = 0;

        /*Aditional popups*/
        private ShapePopupViewModel m_shapePopup;
        private FileSettingPopupViewModel m_fileSettingPopup;
        #endregion

        #region Properties
        public ObservableCollection<ResizingAdorner> SelectedObjects
        {
            get => m_selectedObjects;
            set
            {
                m_selectedObjects = value;
                OnPropertyChanged(nameof(SelectedObjects));
            }
        }
        public override LayerItemViewModel SelectedLayer
        {
            get => m_selectedLayer;
            set
            {
                m_selectedLayer = value;
                OnPropertyChanged(nameof(SelectedLayer));
            }
        }
        public long TestCount
        {
            get => m_countOfObjectsInLayer;
            set
            {
                m_countOfObjectsInLayer = value;
                OnPropertyChanged(nameof(TestCount));
            }
        }

        public override bool IsMetadataToSave { get => base.IsMetadataToSave; set => base.IsMetadataToSave = value; }
        /*Drawing*/
        public bool IgnoreDrawing => DrawableType == DrawableTypes.None;
        public DrawableTypes DrawableType { get; set; }

        /*Aditional popups*/
        public ShapePopupViewModel ShapePopupObject
        {
            get => m_shapePopup;
            set
            {
                m_shapePopup = value;
                OnPropertyChanged(nameof(ShapePopupObject));
            }
        }
        public FileSettingPopupViewModel FileSettingPopupObject
        {
            get => m_fileSettingPopup;
            set
            {
                m_fileSettingPopup = value;
                OnPropertyChanged(nameof(FileSettingPopupObject));
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
        public DrawingCanvasViewModel(IMainWindowViewModel mainWindowViewModel, double width, double height) : base(mainWindowViewModel, width, height)
        {
            SetNecessaryData();

            IsSaved = false;
            Layers = new ObservableCollection<LayerItemViewModel>() { new LayerItemViewModel(new Canvas(), 1, "Layer_1") };
            SelectedLayer = Layers[0];
        }

        public DrawingCanvasViewModel(string filePath, IMainWindowViewModel mainWindowViewModel) : base(mainWindowViewModel)
        {
            SetNecessaryData();

            FilePath = filePath;
            m_isFileToBeLoaded = true;
            Layers = new ObservableCollection<LayerItemViewModel>();
        }

        private void SetNecessaryData()
        {
            SetCommands();
            m_textBoxStyle = (Style)Application.Current.Resources["TextBoxTransparent"];

            ShapePopupObject = new ShapePopupViewModel(this);
            FileSettingPopupObject = new FileSettingPopupViewModel(this);
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
                Trace.WriteLine($"Count Selected Objects: {SelectedObjects.Count}, starting opertaion");
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                Canvas canvas = new Canvas();
                canvas.Tag = "group";
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
                    canvas.Children.Add(adorner.AdornedElement);
                }
                SelectedObjects.Clear();

                Rect rect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);
                canvas.Width = rect.Width;
                canvas.Height = rect.Height;
                canvas.RenderSize = rect.Size;
                MatrixTransform matrixTransform = new MatrixTransform(1, 0, 0, 1, xMin, yMin);
                canvas.RenderTransform = matrixTransform;
                foreach (UIElement child in canvas.Children)
                {
                    child.RenderTransform = new MatrixTransform(RemoveTranslation(child.RenderTransform.Value, matrixTransform.Value));
                }
                SelectedLayer.Layer.Children.Add(canvas);
                canvas.Background = Brushes.Transparent;
                CreateEditingAdorner(canvas);
                stopWatch.Stop();
                Trace.WriteLine($"grouping Time: {stopWatch.Elapsed.TotalMilliseconds}");
            }
            else
            {
                MessageBox.Show("not enough objects selected");
            }
        }
        public void UngroupObjects()
        {
            if (SelectedObjects.Count == 1 && SelectedObjects[0].AdornedElement is Canvas)
            {
                Trace.WriteLine($"Count Selected Objects: {SelectedObjects.Count}, starting opertaion");
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                Canvas group = SelectedObjects[0].AdornedElement as Canvas;
                SelectedObjects[0].RemoveFromAdornerLayer();
                UIElementCollection collection = group.Children;
                while (collection.Count > 0)
                {
                    UIElement child = collection[0];
                    collection.Remove(child);
                    SelectedLayer.Layer.Children.Add(child);
                    child.RenderTransform = new MatrixTransform(RestoreTranslation(child.RenderTransform.Value, group.RenderTransform.Value));
                    CreateEditingAdorner(child);
                }
                SelectedLayer.Layer.Children.Remove(group);
                stopWatch.Stop();
                Trace.WriteLine($"ungrouping time: {stopWatch.ElapsedMilliseconds}");
            }
            else
            {
                MessageBox.Show("too many object ora Selected Object is not a group");
            }
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

            VisualTreeHelper.HitTest(MainCanvas, null,
                HitTestResultCallback,
                new PointHitTestParameters(e));
        }
        private HitTestResultBehavior HitTestResultCallback(HitTestResult result)
        {
            PointHitTestResult testResult = result as PointHitTestResult;
            if ((testResult.VisualHit as FrameworkElement != null) && (testResult.VisualHit as FrameworkElement).Parent == SelectedLayer.Layer)
            {
                CreateEditingAdorner(testResult.VisualHit as UIElement);
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
        }
        private void CreateEditingAdorner(UIElement uiElement)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(uiElement);
            ResizingAdorner adorner = new(uiElement, adornerLayer, MainCanvas);
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
            foreach (ResizingAdorner adorner in SelectedObjects)
            {
                UIElement element = adorner.AdornedElement;
                Transform transform = element.RenderTransform;
                Matrix matrix = transform.Value;
                matrix.ScaleAtPrepend(isVertical ? 1 : -1, isVertical ? -1 : 1, element.RenderSize.Width / 2, element.RenderSize.Height / 2);
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
                if (DrawableType == DrawableTypes.Text)
                {
                    SelectedLayer.Layer.Children.Remove(m_drawableObjectShape);
                    Matrix matrix = m_drawableObjectShape.RenderTransform.Value;
                    TextBox textBox = new TextBox();
                    textBox.RenderTransform = new MatrixTransform(matrix);
                    textBox.RenderSize = m_drawableObjectShape.RenderSize;
                    textBox.TextWrapping = TextWrapping.Wrap;
                    textBox.AcceptsReturn = true;
                    textBox.AcceptsTab = true;
                    textBox.UndoLimit = 10;
                    textBox.Style = m_textBoxStyle;
                    textBox.Text = "Text";
                    SelectedLayer.Layer.Children.Add(textBox);
                }

            }
            else
                SelectionTest(e.GetPosition(MainCanvas));
        }
        private void MouseMoveHandler(MouseEventArgs e)
        {
            m_positionInCanvas.X = e.GetPosition(MainCanvas).X;
            m_positionInCanvas.Y = e.GetPosition(MainCanvas).Y;
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
                        Point point = e.GetPosition(MainCanvas);
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
        #endregion

        #region TestMethod
        public void TestCountOfObjects()
        {
            Task.Run(() =>
            {
                long i = 0;
                TestCount = i;
                try
                {
                    while (true)
                    {
                        SelectedLayer.Layer.Dispatcher.Invoke(() =>
                        {
                            Rectangle rect = new Rectangle();
                            rect.Width = 50;
                            rect.Height = 50;
                            rect.Fill = Brushes.White;
                            rect.StrokeThickness = 2;
                            rect.Stroke = Brushes.Black;
                            Canvas.SetLeft(rect, i);
                            SelectedLayer.Layer.Children.Add(rect);
                        });
                        i++;
                        if (i % 1000 == 0)
                        {
                            SelectedLayer.Layer.Dispatcher.Invoke(() =>
                            {
                                TestCount = i;
                                SelectedLayer.Layer.InvalidateVisual();
                                Trace.WriteLine($"CountOfObjects: {i}");
                            });
                            Thread.Sleep(100);
                        }
                    }
                }
                catch (Exception e)
                {
                    SelectedLayer.Layer.Dispatcher.Invoke(() =>
                    {
                        SelectedLayer.Layer.Children.Clear();
                        TestCount = i;
                        Trace.WriteLine($"CountOfObjects: {i}");
                        Trace.WriteLine(e.Message);
                    });
                }
            });

        }

        public void TestGroupingOfObjects()
        {
            long[] testarray = new long[] { 1000, 5000, 10000, 50000, 100000, 500000, 1000000 };
            try
            {
                for (int i = 0; i < testarray.Length; i++)
                {
                    TestCount = testarray[i];
                    for (int j = 0; j < testarray[i]; j++)
                    {
                        Rectangle rect = new Rectangle();
                        rect.Width = 50;
                        rect.Height = 50;
                        rect.Fill = Brushes.White;
                        rect.StrokeThickness = 2;
                        rect.Stroke = Brushes.Black;
                        Canvas.SetLeft(rect, i + 10);
                        SelectedLayer.Layer.Children.Add(rect);
                        CreateEditingAdorner(rect);
                    }
                    GroupObjects();
                    Thread.Sleep(1000);
                    UngroupObjects();
                    DeleteSelectedObjects();
                    Thread.Sleep(500);
                }
            }
            catch (Exception e)
            {
                SelectedLayer.Layer.Children.Clear();
                Trace.WriteLine(e.Message);
            }
        }
        #endregion
    }
}
