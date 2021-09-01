using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VectorMaker.ControlsResources
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public ColorPicker()
        {
            InitializeComponent();
        }

        private static DependencyProperty m_baseColor = DependencyProperty
        .Register("BaseColor", typeof(Color), typeof(Slider),
        new PropertyMetadata(ColorsReference.magentaBaseColor));

        public Color BaseColor
        {
            get { return (Color)GetValue(m_baseColor); }
            set { SetValue(m_baseColor, value); }
        }

        private void SetBaseColor(Color color)
        {
            BaseColor = color;
        }

        private void CalculateBaseColorOnPickedColor()
        {

        }

        private void SetCursor(Point position)
        { 
        
        }

        public Color GetPickedColor()
        {
            Color color;

            //toDo to_implement
            return color;
        }

    }
}
