using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VectorMaker.Drawables;

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
        private SolidColorBrush m_selectedTabItemBackground;
        private List<Path> m_listOfPaths;
        private bool m_wasFirstDown = false;
        private Drawable m_drawableObject;
        private DrawableTypes m_drawableType;
        private PathSettings m_pathSettings;

        public MainWindow()
        {

            InitializeComponent();
            m_mainCanvas = MainCanvas;
            m_mainCanvasBorder = MainCanvasBorder;
            m_positionInCanvas = new Point(0, 0);
            m_listOfPaths = new List<Path>();
            m_pathSettings = new PathSettings();

            m_selectedTabItemBackground = (SolidColorBrush)Application.Current.FindResource("TabItemBackgroundSelectedColor");
        }

        private void TabControl1_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            foreach (var item in e.AddedItems)
                (item as TabItem).Background = m_selectedTabItemBackground;
            //(item as TabItem).Background = new SolidColorBrush(Color.FromArgb(128,128,128,128));
            foreach (var item in e.RemovedItems)
                (item as TabItem).Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 8));
        }

        private void DrawRectangleButton_Click(object sender, RoutedEventArgs e)
        {
            m_drawableType = DrawableTypes.Rectangle;
        }

        private void DrawEllipseButton_Click(object sender, RoutedEventArgs e)
        {
            m_drawableType = DrawableTypes.Ellipse;
        }

        private void MainCanvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            m_positionInCanvas.X = e.GetPosition(m_mainCanvas).X;
            m_positionInCanvas.Y = e.GetPosition(m_mainCanvas).Y;
            if (m_wasFirstDown)
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
            if (!m_wasFirstDown)
            {
                m_wasFirstDown = true;
                m_drawableObject = new DrawableLine(m_pathSettings);
                var path = m_drawableObject.SetStartPoint(m_positionInCanvas);
                m_listOfPaths.Add(path);
                m_mainCanvas.Children.Add(path);
                Trace.WriteLine(m_positionInCanvas);
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
    }
}
