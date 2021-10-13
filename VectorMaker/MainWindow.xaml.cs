using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VectorMaker.Drawables;
using System.Xml.Linq;
using SVG_XAML_Converter_Lib;
using System.Windows.Markup;
using System.Linq;

namespace VectorMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Border m_mainCanvasBorder;
        private Canvas m_mainCanvas;
        private Point m_positionInCanvas;
        private List<Path> m_listOfPaths;
        private bool m_wasFirstDown = false;
        private Drawable m_drawableObject;
        private DrawableTypes m_drawableType;
        private PathSettings m_pathSettings;
        private bool m_ignoreDrawingGeometries = true;
        private Observable<Path> m_selectedObject = null;
        private XDocument m_xamlElements = null;

        public static MainWindow Instance { get; private set; }
        public Drawable DrawableObject
        {
            get
            {
                return m_drawableObject;
            }
            set
            {
                m_drawableObject = value;
            }
        }
        public DrawableTypes DrawableTypes
        {
            get
            {
                return m_drawableType;
            }
            set => m_drawableType = value;
        }

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

        static MainWindow()
        {
            Instance = new MainWindow();
        }

        public MainWindow()
        {
            this.DataContext = this;
            InitializeComponent();
            m_mainCanvas = MainCanvas;
            m_mainCanvasBorder = MainCanvasBorder;
            m_positionInCanvas = new Point(0, 0);
            m_listOfPaths = new List<Path>();
            m_pathSettings = new PathSettings();
            m_selectedObject = new Observable<Path>(SelectionOfObject);
            m_xamlElements = SVG_To_XAML.ConvertSVGToXamlCode("C://Users//emili//OneDrive//Pulpit//rysunek.svg");
            //Trace.WriteLine(m_xamlElements.ToString());
            object path = XamlReader.Parse(m_xamlElements.ToString());
            //Geometry.Parse()
            m_mainCanvas.Children.Add(path as UIElement);
        }

        private void TabControl1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            foreach (var item in e.AddedItems)
                (item as TabItem).Background = ColorsReference.selectedTabItemBackground;
            foreach (var item in e.RemovedItems)
                (item as TabItem).Background = ColorsReference.notSelectedTabItemBackground;
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

        private void MainCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            m_positionInCanvas.X = e.GetPosition(m_mainCanvas).X;
            m_positionInCanvas.Y = e.GetPosition(m_mainCanvas).Y;
            if (m_wasFirstDown && m_drawableObject != null)
            {

                m_drawableObject.AddPointToList(m_positionInCanvas);
                m_mainCanvas.InvalidateVisual();
            }
        }

        private void MainCanvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            EndDrawing();
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!m_ignoreDrawingGeometries)
            {
                if (!m_wasFirstDown)
                {
                    m_wasFirstDown = true;
                    switch (m_drawableType)
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
                    }
                    if (m_drawableObject != null)
                    {
                        var path = m_drawableObject.SetStartPoint(m_positionInCanvas);
                        m_listOfPaths.Add(path);
                        m_mainCanvas.Children.Add(path);
                        Trace.WriteLine(m_positionInCanvas);
                    }
                }
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            EndDrawing();
        }

        private void EndDrawing()
        {
            m_wasFirstDown = false;
        }

        private void SelectItemButton_Click(object sender, RoutedEventArgs e)
        {
            DrawableTypes = DrawableTypes.None;
            DrawableObject = null;
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
    }
}
 
