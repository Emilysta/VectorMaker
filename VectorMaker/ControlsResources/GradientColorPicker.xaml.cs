using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using VectorMaker.Utility;
using VectorMaker.ViewModel;

namespace VectorMaker.ControlsResources
{
    /// <summary>
    /// Interaction logic for GradientColorPicker.xaml
    /// </summary>
    public partial class GradientColorPicker : UserControl, INotifyPropertyChanged
    {
        private double m_offsetValue;
        private SolidColorBrush m_gradientStopColor = new();
        private GradientStop m_selectedGradientStop = new();
        private static DependencyProperty m_brushToEdit = DependencyProperty.Register("BrushToEdit", typeof(Brush), typeof(GradientColorPicker), new PropertyMetadata(new SolidColorBrush()));

        public Brush BrushToEdit
        {
            get { return (Brush)GetValue(m_brushToEdit); }
            set { SetValue(m_brushToEdit, value);
                OnPropertyChanged(nameof(BrushToEdit));
                OnPropertyChanged(nameof(Gradient));
                SetThumbs();
            }
        }

        public GradientBrush Gradient
        {
            get => (BrushToEdit as GradientBrush);
            set
            {
                BrushToEdit = value;
                OnPropertyChanged(nameof(Gradient));
                OnPropertyChanged(nameof(BrushToEdit));
                SetThumbs();
            }
        }

        public double OffsetValue
        {
            get => m_offsetValue;
            set
            {
                m_offsetValue = value;
                OnPropertyChanged(nameof(OffsetValue));
            }
        }

        public GradientStop SelectedGradientStop
        {
            get => m_selectedGradientStop;
            set
            {
                m_selectedGradientStop = value;
                OnPropertyChanged(nameof(SelectedGradientStop));
                GradientStopBrush.Color = m_selectedGradientStop.Color;
            }
        }


        public SolidColorBrush GradientStopBrush
        {
            get => m_gradientStopColor;
            set
            {
                m_gradientStopColor = value;
                OnPropertyChanged(nameof(GradientStopBrush));
                SelectedGradientStop.Color = m_gradientStopColor.Color;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public GradientColorPicker()
        {
            InitializeComponent();
            DataContext = this;
            Gradient = new LinearGradientBrush(
                new GradientStopCollection() {
                    new GradientStop(Colors.Transparent, 0),
                    new GradientStop(ColorsReference.magentaBaseColor, 1)
                }
                );
            GradientStopBrush = new SolidColorBrush();
            SelectedGradientStop = Gradient.GradientStops[0];
        }

        private void MultiThumbSliderObject_SelectedThumbChanged(object sender, EventArgs e)
        {
            GradientSliderAdorner gradient = MultiThumbSliderObject.SelectedThumb as GradientSliderAdorner;
            SelectedGradientStop = gradient.GradientStopObject;
        }

        private void ColorRectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColorPickerViewModel colorPicker = new(GradientStopBrush);
            colorPicker.ShowWindowAndWaitForResult();
            GradientStopBrush.Color = colorPicker.SelectedColor;
            SelectedGradientStop.Color = colorPicker.SelectedColor;
        }

        private void MultiThumbSliderObject_AddedThumb(object sender, EventArgs e)
        {
            ThumbEventArgs thumbEventArgs = e as ThumbEventArgs;
            GradientSliderAdorner gradient = thumbEventArgs.NewThumb as GradientSliderAdorner;
            Gradient.GradientStops.Add(gradient.GradientStopObject);
            OnPropertyChanged("GradientStops");
        }

        private void SetThumbs()
        {
            MultiThumbSliderObject.DeleteThumbs();
            foreach (GradientStop stop in Gradient.GradientStops)
            {
                MultiThumbSliderObject.CreateThumbWithGradient(stop);
            }
        }

        private void MultiThumbSliderObject_DeletedThumb(object sender, EventArgs e)
        {
            ThumbEventArgs thumbEventArgs = e as ThumbEventArgs;
            GradientSliderAdorner gradient = thumbEventArgs.NewThumb as GradientSliderAdorner;
            Gradient.GradientStops.Remove(gradient.GradientStopObject);
            OnPropertyChanged("GradientStops");
        }
    }
}
