using System.Windows;
using System.Windows.Controls;
using WindowsColor = System.Windows.Media.Color;

namespace VectorMaker.ControlsResources
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        private WindowsColor m_pickedColor;
        private Color m_pickedColorClass;
        private WindowsColor m_baseColor;

        public WindowsColor PickedColor
        {
            get { return m_pickedColor; }
            set { m_pickedColor = value; }
        }

        public WindowsColor BaseColor
        {
            get { return m_baseColor; }
            set { m_baseColor = value; }
        }

        public ColorPicker()
        {
            InitializeComponent();
            DataContext = this;
            m_pickedColorClass = new Color(0, 0, 0, 255);
            CalculateBaseColorOnPickedColor();
        }

        private void SetBaseColor(WindowsColor color)
        {
            BaseColor = color;
        }

        private void CalculateBaseColorOnPickedColor()
        {
            Color tempColor = new Color(m_pickedColorClass.HSLColor.H.IntProperty, 100, 50, 255, true);
            BaseColor = tempColor.ColorInWindowsFormat;
            Picker.InvalidateVisual();
        }

        private void SetCursor(System.Drawing.Point position)
        {

        }

    }
}
