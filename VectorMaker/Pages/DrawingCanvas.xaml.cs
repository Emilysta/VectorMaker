using System.Windows.Controls;
using VectorMaker.Drawables;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Markup;
using Microsoft.Win32;
using System;
using System.Windows.Input;
using VectorMaker.Utility;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Drawing.Printing;
using System.Linq;
using FileStream = System.IO.FileStream;
using File = System.IO.File;
using FileMode = System.IO.FileMode;
using VectorMaker.ViewModel;

namespace VectorMaker.Pages
{
    /// <summary>
    /// Logika interakcji dla klasy Page1.xaml
    /// </summary>
    public partial class DrawingCanvas : UserControl
    {
        public DrawingCanvasViewModel ViewModel { get; set; }
        public ViewportController ViewportController;

        public DrawingCanvas()
        {
            InitializeComponent();
            ViewModel = new DrawingCanvasViewModel(MainCanvas);
            DataContext = ViewModel;
            ViewportController = new ViewportController(ScaleParent, ZoomScrollViewer);
        }

        public DrawingCanvas(string filePath)
        {
            InitializeComponent();
            ViewModel = new DrawingCanvasViewModel(filePath, MainCanvas);
            DataContext = ViewModel;
            ViewportController = new ViewportController(ScaleParent, ZoomScrollViewer);
        }

        private void MainCanvas_MouseLeave(object sender, MouseEventArgs e)
        {
            ViewModel.EndDrawing();
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ViewModel.MouseLeftButtonDownHandler();
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ViewModel.MouseLeftButtonUpHandler(e);
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            ViewModel.MouseMoveHandler(e);
        }

        private void ZoomScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            ViewportController.ScrollViewerChanged(e);
        }

        private void ZoomScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ViewportController.MouseWheel(e);
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
            ViewModel.KeyDownHandler(e.Key);
        }

        private void MainCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            ViewModel.KeyUpHandler(e.Key);
        }
    }
}
