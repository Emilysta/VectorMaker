using System.Windows.Controls;
using VectorMaker.Drawables;
using System.Xml.Linq;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Shapes;
using System.Windows.Markup;
using System.Diagnostics;
using Microsoft.Win32;
using System;
using System.Windows.Input;
using VectorMaker.Utility;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace VectorMaker.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy Page1.xaml
    /// </summary>
    public partial class DrawingCanvas : Page
    {
        private Canvas m_mainCanvas;
        private Point m_positionInCanvas;
        private List<Path> m_listOfPaths;
        private bool m_wasFirstDown = false;
        private Drawable m_drawableObject;
        private PathSettings m_pathSettings;
        private XDocument m_xamlElements = null;
        private bool m_isSaved = true;
        private string m_filePath;
        private Drawable m_selectionObject;
        private Path m_selectionObjectPath;
        private List<ResizingAdorner> m_selectedObjects;
        public ViewportController viewportController;

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
            XDocument document = SVG_XAML_Converter_Lib.SVG_To_XAML.ConvertSVGToXamlCode(fileName);
            m_filePath = fileName;
            if (document != null)
            {
                m_xamlElements = document;
                object path = XamlReader.Parse(m_xamlElements.ToString());
                m_mainCanvas.Children.Add(path as UIElement);
            }

        }

        private void SetProperties()
        {
            m_mainCanvas = MainCanvas;
            //m_mainCanvasBorder = MainCanvasBorder;
            m_positionInCanvas = new Point(0, 0);
            m_listOfPaths = new List<Path>();
            m_pathSettings = new PathSettings();
            m_xamlElements = new XDocument();
            viewportController = new ViewportController(ScaleParent, ZoomScrollViewer);
            m_selectedObjects = new List<ResizingAdorner>();
            m_selectionObjectPath = new Path();
            m_mainCanvas.Children.Add(m_selectionObjectPath);
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
            else
            {
                switch (MainWindow.Instance.DrawableType)
                {
                    case DrawableTypes.SelectionTool:
                        {
                            m_wasFirstDown = true;
                            m_selectionObject = new DrawableRectangle(PathSettings.SelectionSettings());
                            m_selectionObjectPath = m_selectionObject.SetStartPoint(m_positionInCanvas);
                            break;
                        }
                    case DrawableTypes.EditPointSelectionTool:
                        {
                            m_wasFirstDown = true;
                            m_selectionObject = new DrawableRectangle(PathSettings.SelectionSettings());
                            m_selectionObjectPath = m_selectionObject.SetStartPoint(m_positionInCanvas);
                            break;
                        }
                    case DrawableTypes.None:
                        {
                            m_selectionObject = null;
                            break;
                        }
                }
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!MainWindow.Instance.IgnoreDrawingGrometries)
                EndDrawing();
            else if (MainWindow.Instance.DrawableType != DrawableTypes.None)
                SelectionTest(e.GetPosition(m_mainCanvas));
        }


        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            m_positionInCanvas.X = e.GetPosition(m_mainCanvas).X;
            m_positionInCanvas.Y = e.GetPosition(m_mainCanvas).Y;
            if (m_wasFirstDown && m_drawableObject != null)
            {
                if (MainWindow.Instance.IgnoreDrawingGrometries == true)
                    m_selectionObject.AddPointToList(m_positionInCanvas);
                else
                    m_drawableObject.AddPointToList(m_positionInCanvas);
                m_mainCanvas.InvalidateVisual();
            }
        }


        private void EndDrawing()
        {
            m_wasFirstDown = false;
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


            // Set up a callback to receive the hit test result enumeration.
            VisualTreeHelper.HitTest(MainCanvas, null,
                HitTestResultCallback,
                new PointHitTestParameters(e));

            // Perform actions on the hit test results list.
            //if (m_selectedObjects.Count > 0)
            //{
            //    foreach (Visual visual in m_selectedObjects)
            //    {
            //        DrawObjectBoundries(visual);
            //    }
            //}
        }

        private HitTestResultBehavior HitTestResultCallback(HitTestResult result)
        {
            PointHitTestResult testResult = result as PointHitTestResult;
            if ((testResult.VisualHit as FrameworkElement).Parent != ScaleParent)
            {
                AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(testResult.VisualHit);
                ResizingAdorner adorner = new ResizingAdorner(testResult.VisualHit as UIElement, adornerLayer);
                adornerLayer.Add(adorner);
                m_selectedObjects.Add(adorner);
                //m_selectedObjects.Add(new SelectedVisual(testResult.VisualHit,m_mainCanvas));
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
        }

        public bool SaveToFile()
        {
            if (!m_isSaved)
            {
                if (m_filePath == "")
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    saveFileDialog.Filter = "Scalable Vector Graphics (*.svg) | *.svg | Microsoft XAML File (*.xaml) | *.xaml";
                    saveFileDialog.FileName = "untilted.xaml";
                    Nullable<bool> result = saveFileDialog.ShowDialog();
                    if (result == true)
                    {
                        m_xamlElements.Save(saveFileDialog.OpenFile());
                        m_filePath = saveFileDialog.FileName;
                        m_isSaved = true;
                        return true;
                    }
                    else
                        return false;
                }
                m_xamlElements.Save(m_filePath);
                return true;
            }
            return true;
        }

        private void ZoomScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            viewportController.ScrollViewerChanged(e);
        }

        private void ZoomScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            viewportController.MouseWheel(e);
        }
    }
}
