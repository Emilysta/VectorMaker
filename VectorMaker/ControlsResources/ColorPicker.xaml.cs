using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
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
        private Color m_pickedColorClass = new Color(0, 0, 0, 255);
        private WindowsColor m_baseColor;
        private Point m_colorPickerMarkerPosition;
        private Point m_firstMarkerPosition;
        private Size m_colorPickerSize;
        private DispatcherTimer m_dispatcherTimer;

        private string m_rColorText;
        private string m_gColorText;
        private string m_bColorText;
        private string m_aColorText;
        private string m_hColorText;
        private string m_sColorText;
        private string m_lColorText;

        public event PropertyChangedEventHandler PropertyChanged;

        public WindowsColor PickedColor
        {
            get { return m_pickedColor; }
            set { m_pickedColor = value;
                CalculateAllParametersOfColor();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("PickedColor"));
            }
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

        public Point ColorPickerMarkerPoisition
        {
            get { return m_colorPickerMarkerPosition; }
            set
            {
                m_colorPickerMarkerPosition = value;
                CalculateAllParametersOfColor();
            }
        }

        public string RColorText
        {
            get { return m_rColorText; }
            set
            {
                m_rColorText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("RColorText"));
            }
        }

        public string GColorText
        {
            get { return m_gColorText; }
            set
            {
                m_gColorText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("GColorText"));
            }
        }

        public string BColorText
        {
            get { return m_bColorText; }
            set
            {
                m_bColorText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("BColorText"));
            }
        }

        public string AColorText
        {
            get { return m_aColorText; }
            set
            {
                m_aColorText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("AColorText"));
            }
        }

        public string HColorText
        {
            get { return m_hColorText; }
            set
            {
                m_hColorText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("HColorText"));
            }
        }
        public string SColorText
        {
            get { return m_sColorText; }
            set
            {
                m_sColorText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("SColorText"));
            }
        }
        public string LColorText
        {
            get { return m_lColorText; }
            set
            {
                m_lColorText = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LColorText"));
            }
        }

        public ColorPicker()
        {
            InitializeComponent();
            DataContext = this;
            CalculateBaseColorOnPickedColor();
            CalculateAllParametersOfColor();
            m_colorPickerSize = Picker.RenderSize; // toDo sizeChanged method on whole app
            m_dispatcherTimer = new DispatcherTimer();
            m_dispatcherTimer.Tick += delegate
            {
               SetMarkerPosition(
                    Mouse.GetPosition(Application.Current.MainWindow)
                    );
            };
            m_dispatcherTimer.Interval = new System.TimeSpan(0, 0, 0, 0, 100);
        }

        private void SetBaseColor(WindowsColor color)
        {
            BaseColor = color;
        }

        private void CalculateBaseColorOnPickedColor()
        {
            Color tempColor = new Color(m_pickedColorClass.HSLColor.H.IntProperty, 100, 50, 255, true);
            SetBaseColor(tempColor.ColorInWindowsFormat); 
        }

        private void CalculateAllParametersOfColor()
        {
            m_pickedColorClass.ColorInWindowsFormat = PickedColor;
            if (m_pickedColorClass != null)
            {
                RColorText = m_pickedColorClass.RGBColor.R.IntProperty.ToString();
                GColorText = m_pickedColorClass.RGBColor.G.IntProperty.ToString();
                BColorText = m_pickedColorClass.RGBColor.B.IntProperty.ToString();
                HColorText = m_pickedColorClass.HSLColor.H.IntProperty.ToString();
                SColorText = m_pickedColorClass.HSLColor.S.IntProperty.ToString();
                LColorText = m_pickedColorClass.HSLColor.L.IntProperty.ToString();
                AColorText = m_pickedColorClass.A.IntProperty.ToString();
            }
        }

        private void SetMarkerPosition(System.Windows.Point position)
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

        private void ColorPickerMarker_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Application.Current.MainWindow.CaptureMouse();
            m_dispatcherTimer.Start();
            m_firstMarkerPosition = Mouse.GetPosition(Application.Current.MainWindow);
        }

        private void ColorPickerMarker_MouseUp(object sender, MouseButtonEventArgs e)
        {
            m_dispatcherTimer.Stop();
            Application.Current.MainWindow.ReleaseMouseCapture();
        }

        private void ColorEyeDropper_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ColorEyeDropper_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<WindowsColor?> e)
        {
               PickedColor = (WindowsColor)e.NewValue;
        }
    }
}
