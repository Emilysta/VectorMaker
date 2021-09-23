using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WindowsColor = System.Windows.Media.Color;
using WindowsMedia = System.Windows.Media;

namespace VectorMaker.ControlsResources
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl, INotifyPropertyChanged
    {
        private WindowsColor m_pickedColor;
        private Color m_pickedColorClass;
        private WindowsColor m_baseColor;

        public event PropertyChangedEventHandler PropertyChanged;

        public WindowsColor PickedColor
        {
            get { return m_pickedColor; }
            set { m_pickedColor = value; }
        }

        public WindowsColor BaseColor
        {
            get { return m_baseColor; }
            set
            {
                m_baseColor = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BaseColor"));
            }
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
            Picker.InvalidateVisual();
        }

        private void CalculateBaseColorOnPickedColor()
        {
            Color tempColor = new Color(m_pickedColorClass.HSLColor.H.IntProperty, 100, 50, 255, true);
            SetBaseColor(tempColor.ColorInWindowsFormat);
        }

        private void SetCursor(System.Drawing.Point position)
        {

        }

        private void ValuePicker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            WindowsColor color = GetColorFromValueGradient(e.NewValue);
            SetBaseColor(color);

        }

        private static WindowsColor GetColorFromValueGradient(double offset)
        {
            WindowsColor tempColor = new WindowsColor();
            WindowsMedia.GradientStop gradientStopBeforeOffsetThreshold;
            WindowsMedia.GradientStop gradientStopAfterOffsetThreshold = 
                ColorsReference.valueGradientStopListSegregated.FirstOrDefault(x => x.Offset >= offset);

            int index = ColorsReference.valueGradientStopListSegregated.IndexOf(gradientStopAfterOffsetThreshold);
            if (index == 0)
            {
                gradientStopBeforeOffsetThreshold = gradientStopAfterOffsetThreshold;
                tempColor = gradientStopBeforeOffsetThreshold.Color;
                return tempColor;
            }
            else
                gradientStopBeforeOffsetThreshold = ColorsReference.valueGradientStopListSegregated[index - 1];

            tempColor.ScA = (float)((offset - gradientStopBeforeOffsetThreshold.Offset) * (gradientStopAfterOffsetThreshold.Color.ScA - gradientStopBeforeOffsetThreshold.Color.ScA) / (gradientStopAfterOffsetThreshold.Offset - gradientStopBeforeOffsetThreshold.Offset) + gradientStopBeforeOffsetThreshold.Color.ScA);
            tempColor.ScR = (float)((offset - gradientStopBeforeOffsetThreshold.Offset) * (gradientStopAfterOffsetThreshold.Color.ScR - gradientStopBeforeOffsetThreshold.Color.ScR) / (gradientStopAfterOffsetThreshold.Offset - gradientStopBeforeOffsetThreshold.Offset) + gradientStopBeforeOffsetThreshold.Color.ScR);
            tempColor.ScG = (float)((offset - gradientStopBeforeOffsetThreshold.Offset) * (gradientStopAfterOffsetThreshold.Color.ScG - gradientStopBeforeOffsetThreshold.Color.ScG) / (gradientStopAfterOffsetThreshold.Offset - gradientStopBeforeOffsetThreshold.Offset) + gradientStopBeforeOffsetThreshold.Color.ScG);
            tempColor.ScB = (float)((offset - gradientStopBeforeOffsetThreshold.Offset) * (gradientStopAfterOffsetThreshold.Color.ScB - gradientStopBeforeOffsetThreshold.Color.ScB) / (gradientStopAfterOffsetThreshold.Offset - gradientStopBeforeOffsetThreshold.Offset) + gradientStopBeforeOffsetThreshold.Color.ScB);

            return tempColor;
        }
    }
}
