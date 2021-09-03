using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;


namespace VectorMaker.ControlsResources
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        private Color m_pickedColor;

        public Color PickedColor
        {
            get { return m_pickedColor; }
            set { m_pickedColor = value; }
        }



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
            float hueColor = m_pickedColor.GetHue();
            float saturationColot = m_pickedColor.GetSaturation();
            float value = m_pickedColor.GetBrightness();
        }

        private void SetCursor(System.Drawing.Point position)
        { 
        
        }
    }
}
