using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private GradientStopCollection m_gradientStops = new GradientStopCollection();

        private double m_offsetValue;
        private Brush m_gradientStopColor;
        private GradientBrush m_gradientBrushFill;
        private Dictionary<ThumbSliderAdorner, GradientStop> m_thumbDictionary;


        public GradientStopCollection GradientStops
        {
            get => m_gradientStops;
            set
            {
                m_gradientStops = value;
                OnPropertyChanged(nameof(GradientStops));
                SetDictionaryAndThumbs();
            }
        }


        public GradientBrush GradientBrushFill
        {
            get => m_gradientBrushFill;
            set
            {
                m_gradientBrushFill = value;
                OnPropertyChanged(nameof(GradientBrushFill));
                GradientStops = m_gradientBrushFill.GradientStops;
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

        public Brush GradientStopColor
        {
            get => m_gradientStopColor;
            set
            {
                m_gradientStopColor = value;
                OnPropertyChanged(nameof(GradientStopColor));
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
        }

        private void MultiThumbSliderObject_SelectedThumbChanged(object sender, EventArgs e)
        {
            OffsetValue = MultiThumbSliderObject.SelectedThumb.Offset;
        }

        private void ColorRectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ColorPickerViewModel colorPicker = new ColorPickerViewModel(GradientStopColor);
            colorPicker.ShowWindowAndWaitForResult();
        }

        private void MultiThumbSliderObject_AddedThumb(object sender, EventArgs e)
        {
            ThumbEventArgs thumbEventArgs = e as ThumbEventArgs;
            GradientStop gradientStop = new GradientStop(Colors.White, thumbEventArgs.NewThumb.Offset);
            GradientStops.Add(gradientStop);
            m_thumbDictionary.Add(thumbEventArgs.NewThumb, gradientStop);
        }

        private void SetDictionaryAndThumbs()
        {
            //m_thumbDictionary = new Dictionary<ThumbSliderAdorner, GradientStop>();
            //foreach (GradientStop stop in GradientStops)
            //{
            //    m_thumbDictionary.Add(MultiThumbSliderObject.CreateThumb(stop.Offset), stop);
            //}
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            GradientStops = new GradientStopCollection() { new GradientStop(Colors.Transparent, 0), new GradientStop(ColorsReference.magentaBaseColor, 1) };
        }
    }
}
