using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VectorMaker.ControlsResources
{
    /// <summary>
    /// Logika interakcji dla klasy BrushEditor.xaml
    /// </summary>
    public partial class BrushEditor : UserControl
    {

        private static readonly DependencyProperty m_brush = DependencyProperty.Register("Brush", typeof(Brush), typeof(BrushEditor), new PropertyMetadata(null));

        public Brush Brush
        {
            get { return (Brush)GetValue(m_brush); }
            set
            {
                SetValue(m_brush, value);
            }
        }

        public BrushEditor()
        {
            InitializeComponent();
        }
    }
}
