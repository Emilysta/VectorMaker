using System.Windows.Controls;
using VectorMaker.Drawables;
using System.Xml.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Diagnostics;

namespace VectorMaker.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy Page1.xaml
    /// </summary>
    public partial class DrawingCanvas : Page
    {
        private Border m_mainCanvasBorder;
        private Canvas m_mainCanvas;
        private Point m_positionInCanvas;
        private List<Path> m_listOfPaths;
        private bool m_wasFirstDown = false;
        private Drawable m_drawableObject;
        private PathSettings m_pathSettings;
        private XDocument m_xamlElements = null;

        public DrawingCanvas()
        {
            InitializeComponent();
            SetProperties();
        }


        public DrawingCanvas(string fileName)
        {
            InitializeComponent();
            SetProperties();
            XDocument document = SVG_XAML_Converter_Lib.SVG_To_XAML.ConvertSVGToXamlCode(fileName);
            if (document != null)
            {
                m_xamlElements = document;
                object path = XamlReader.Parse(m_xamlElements.ToString());
                Trace.WriteLine(m_xamlElements.ToString());
                //Geometry.Parse()
                m_mainCanvas.Children.Add(path as UIElement);
            }
        }

        private void SetProperties()
        {
            m_mainCanvas = MainCanvas;
            m_mainCanvasBorder = MainCanvasBorder;
            m_positionInCanvas = new Point(0, 0);
            m_listOfPaths = new List<Path>();
            m_pathSettings = new PathSettings();
            m_xamlElements = new XDocument();
        }

        private void MainCanvas_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            EndDrawing();
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!MainWindow.Instance.IgnoreDrawingGrometries)
            {
                if (!m_wasFirstDown)
                {
                    m_wasFirstDown = true;
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

        private void EndDrawing()
        {
            m_wasFirstDown = false;
        }
    }
}
