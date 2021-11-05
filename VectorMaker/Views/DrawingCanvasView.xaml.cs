using System.Windows.Controls;
using VectorMaker.Utility;
using VectorMaker.ViewModel;

namespace VectorMaker.Views
{
    /// <summary>
    /// Inicjalizacja View dla DrawingCanvas
    /// </summary>
    public partial class DrawingCanvasView : UserControl
    {
        public DrawingCanvasView()
        {
            InitializeComponent();
            var conf = Configuration.Instance;
            DataContextChanged += DrawingCanvasView_DataContextChanged;
        }

        private void DrawingCanvasView_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            (DataContext as DrawingCanvasViewModel).MainCanvas = CanvasObject;
        }
    }
}
