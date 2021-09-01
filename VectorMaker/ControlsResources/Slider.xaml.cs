using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VectorMaker.ControlsResources
{

    public partial class Slider : UserControl
    {
        private bool m_wasFirstSet = false;
        private Point m_startPoint;
        private double m_startDifference;

        public Slider()
        {
            InitializeComponent();
        }

        private static DependencyProperty m_height = DependencyProperty
            .Register("HeightOfControl", typeof(int), typeof(Slider), new PropertyMetadata(20));

        private static DependencyProperty m_width = DependencyProperty
            .Register("WidthOfControl", typeof(int), typeof(Slider), new PropertyMetadata(100));

        private static DependencyProperty m_sliderValue = DependencyProperty
            .Register("SliderValue", typeof(double), typeof(Slider), new PropertyMetadata(50D));

        private static DependencyProperty m_maxValue = DependencyProperty
            .Register("MaxValue", typeof(double), typeof(Slider),
            new PropertyMetadata(100D));

        private static DependencyProperty m_minValue = DependencyProperty
            .Register("MinValue", typeof(double), typeof(Slider),
            new PropertyMetadata(0D));

        private static DependencyProperty m_backgroundColor = DependencyProperty
            .Register("BackgroundColor", typeof(Brush), typeof(Slider),
            new PropertyMetadata(Brushes.Gray));

        private static DependencyProperty m_backgroundRadius = DependencyProperty
            .Register("BackgroundRadius", typeof(double), typeof(Slider),
            new PropertyMetadata(5D));

        private static DependencyProperty m_backgroundStrokeThickness = DependencyProperty
            .Register("BackgroundStrokeThickness", typeof(double), typeof(Slider),
            new PropertyMetadata(0D));

        private static DependencyProperty m_backgroundStrokeColor = DependencyProperty
            .Register("BackgroundStrokeColor", typeof(Brush), typeof(Slider),
            new PropertyMetadata(Brushes.Transparent));

        private static DependencyProperty m_sliderText = DependencyProperty
            .Register("SliderText", typeof(string), typeof(Slider), new PropertyMetadata(""));

        private static DependencyProperty m_offsetValue = DependencyProperty
            .Register("OffsetValue", typeof(double), typeof(Slider), new PropertyMetadata(0.5D));

        private static DependencyProperty m_sliderValueInString = DependencyProperty
    .Register("SliderValueInString", typeof(string), typeof(Slider), new PropertyMetadata("50.00"));

        protected override void OnInitialized(EventArgs e) //override On Initialized Event Rised after initialization of Parent
        {
            base.OnInitialized(e);
        }


        public int HeightOfControl //implement Wrapper
        {
            get { return (int)GetValue(m_height); }
            set { SetValue(m_height, value); }
        }

        public int WidthOfControl //implement Wrapper
        {
            get { return (int)GetValue(m_width); }
            set { SetValue(m_width, value); }
        }

        public double SliderValue //implement Wrapper
        {
            get { return (double)GetValue(m_sliderValue); }
            set { OffsetValue = value/MaxValue; 
                SliderValueInString = value.ToString("0.00"); 
                SetValue(m_sliderValue, value); 
            }
        }

        public double MaxValue //implement Wrapper
        {
            get { return (double)GetValue(m_maxValue); }
            set { SetValue(m_maxValue, value); }
        }

        public double MinValue //implement Wrapper
        {
            get { return (double)GetValue(m_minValue); }
            set { SetValue(m_minValue, value); }
        }

        public Brush BackgroundColor //implement Wrapper
        {
            get { return (Brush)GetValue(m_backgroundColor); }
            set { SetValue(m_backgroundColor, value); }
        }

        public double BackgroundRadius //implement Wrapper
        {
            get { return (double)GetValue(m_backgroundRadius); }
            set { SetValue(m_backgroundRadius, value); }
        }

        public double BackgroundStrokeThickness //implement Wrapper
        {
            get { return (double)GetValue(m_backgroundStrokeThickness); }
            set { SetValue(m_backgroundStrokeThickness, value); }
        }

        public Brush BackgroundStrokeColor //implement Wrapper
        {
            get { return (Brush)GetValue(m_backgroundStrokeColor); }
            set { SetValue(m_backgroundStrokeColor, value); }
        }

        public string SliderText //implement Wrapper
        {
            get { return (string)GetValue(m_sliderText); }
            set { SetValue(m_sliderText, value); }
        }

        public double OffsetValue
        {
            get { return (double)GetValue(m_offsetValue); }
            set { SetValue(m_offsetValue, value); }
        }

        public string SliderValueInString
        {
            get { return (string)GetValue(m_sliderValueInString); }
            set { SetValue(m_sliderValueInString, value); }
        }

        private void Thumb_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!m_wasFirstSet)
            {
                m_wasFirstSet = true;
                m_startPoint = e.GetPosition(this);
                m_startDifference = MaxValue - SliderValue;
                Trace.WriteLine("WasFirstDown");
            }
        }

        private void Thumb_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            m_wasFirstSet = false;
        }

        private void Thumb_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (m_wasFirstSet)
            {
                Point position = e.GetPosition(this);
                Rectangle rectangle = sender as Rectangle;
                double positionInX = position.X;
                double positionInY = position.Y;
                double differenceInX = positionInX - m_startPoint.X;
                TranslateTransform translateTransform = new TranslateTransform(differenceInX, 0);
                rectangle.RenderTransform = translateTransform;
            }
        }
    }
}
