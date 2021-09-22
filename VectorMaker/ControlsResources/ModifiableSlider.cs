using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VectorMaker.ControlsResources
{
    public class ModifiableSlider : Slider
    {
        static ModifiableSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ModifiableSlider), new FrameworkPropertyMetadata(typeof(ModifiableSlider)));
        }

        protected override void OnInitialized(EventArgs e) //override On Initialized Event Rised after initialization of Parent
        {
            base.OnInitialized(e);
        }

        private static DependencyProperty m_backgroundRadius = DependencyProperty
            .Register("BackgroundRadius", typeof(CornerRadius), typeof(ModifiableSlider),
            new PropertyMetadata(new CornerRadius(0, 0, 0, 0)));

        private static DependencyProperty m_backgroundStrokeThickness = DependencyProperty
            .Register("BackgroundStrokeThickness", typeof(double), typeof(ModifiableSlider),
            new PropertyMetadata(0D));

        private static DependencyProperty m_backgroundStrokeColor = DependencyProperty
            .Register("BackgroundStrokeColor", typeof(Brush), typeof(ModifiableSlider),
            new PropertyMetadata(Brushes.Transparent));

        private static DependencyProperty m_thumbStrokeThickness = DependencyProperty
            .Register("ThumbStrokeThickness", typeof(double), typeof(ModifiableSlider),
            new PropertyMetadata(0D));

        private static DependencyProperty m_thumbHeight = DependencyProperty
            .Register("ThumbHeight", typeof(double), typeof(ModifiableSlider),
            new PropertyMetadata(0D));

        private static DependencyProperty m_thumbWidth = DependencyProperty
            .Register("ThumbWidth", typeof(double), typeof(ModifiableSlider),
            new PropertyMetadata(0D));

        private static DependencyProperty m_thumbStrokeColor = DependencyProperty
            .Register("ThumbStrokeColor", typeof(Brush), typeof(ModifiableSlider),
            new PropertyMetadata(Brushes.Transparent));

        private static DependencyProperty m_thumbColor = DependencyProperty
            .Register("ThumbColor", typeof(Brush), typeof(ModifiableSlider),
            new PropertyMetadata(Brushes.Transparent));

        private static DependencyProperty m_selectionColor = DependencyProperty
            .Register("SelectionColor", typeof(Brush), typeof(ModifiableSlider),
            new PropertyMetadata(Brushes.Transparent));

        private static DependencyProperty m_isTextEnabled = DependencyProperty
            .Register("IsTextEnabled", typeof(bool), typeof(ModifiableSlider),
            new PropertyMetadata(true));

        private static DependencyProperty m_textVisibility = DependencyProperty
            .Register("TextVisibility", typeof(Visibility), typeof(ModifiableSlider),
            new PropertyMetadata(Visibility.Visible));


        public CornerRadius BackgroundRadius //implement Wrapper
        {
            get { return (CornerRadius)GetValue(m_backgroundRadius); }
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

        public double ThumbStrokeThickness //implement Wrapper
        {
            get { return (double)GetValue(m_thumbStrokeThickness); }
            set { SetValue(m_thumbStrokeThickness, value); }
        }

        public double ThumbWidth //implement Wrapper
        {
            get { return (double)GetValue(m_thumbWidth); }
            set { SetValue(m_thumbWidth, value); }
        }

        public double ThumbHeight //implement Wrapper
        {
            get { return (double)GetValue(m_thumbHeight); }
            set { SetValue(m_thumbHeight, value); }
        }

        public Brush ThumbStrokeColor //implement Wrapper
        {
            get { return (Brush)GetValue(m_thumbStrokeColor); }
            set { SetValue(m_thumbStrokeColor, value); }
        }

        public Brush ThumbColor //implement Wrapper
        {
            get { return (Brush)GetValue(m_thumbColor); }
            set { SetValue(m_thumbColor, value); }
        }

        public Brush SelectionColor //implement Wrapper
        {
            get { return (Brush)GetValue(m_selectionColor); }
            set { SetValue(m_selectionColor, value); }
        }

        public bool IsTextEnabled //implement Wrapper
        {
            get { return (bool)GetValue(m_isTextEnabled); }
            set { SetValue(m_isTextEnabled, value); }
        }

        public Visibility TextVisibility//implement Wrapper
        {
            get { return (Visibility)GetValue(m_textVisibility); }
            set { SetValue(m_textVisibility, value); }
        }
        //private void Thumb_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    if (!m_wasFirstSet)
        //    {
        //        m_wasFirstSet = true;
        //        m_startPoint = e.GetPosition(this);
        //        m_startDifference = MaxValue - SliderValue;
        //        Trace.WriteLine("WasFirstDown");
        //    }
        //}

        //private void Thumb_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        //{
        //    m_wasFirstSet = false;
        //}

        //private void Thumb_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        //{
        //    if (m_wasFirstSet)
        //    {
        //        Point position = e.GetPosition(this);
        //        Rectangle rectangle = sender as Rectangle;
        //        double positionInX = position.X;
        //        double positionInY = position.Y;
        //        double differenceInX = positionInX - m_startPoint.X;
        //        TranslateTransform translateTransform = new TranslateTransform(differenceInX, 0);
        //        rectangle.RenderTransform = translateTransform;
        //    }
        //}
    }
}
