using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace VectorMaker.ControlsResources
{

    public partial class Slider : UserControl
    {
        public Slider()
        {
            InitializeComponent();
        }

        private static DependencyProperty m_height = DependencyProperty
            .Register("HeightOfControl", typeof(int), typeof(Slider), new PropertyMetadata(20));

        private static DependencyProperty m_width = DependencyProperty
            .Register("WidthOfControl", typeof(int), typeof(Slider), new PropertyMetadata(100));

        private static DependencyProperty m_sliderValue = DependencyProperty
            .Register("SliderValue", typeof(double), typeof(Slider), 
            new PropertyMetadata(50D));

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
            set { SetValue(m_sliderValue, value); }
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
            get { return (double)(SliderValue/MaxValue); }
        }

        public string SliderValueInString
        {
            get { return (string)(SliderValue.ToString("0.00")); }
        }
    }
}
