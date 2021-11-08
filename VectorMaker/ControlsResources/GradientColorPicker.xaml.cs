using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace VectorMaker.ControlsResources
{
    /// <summary>
    /// Interaction logic for GradientColorPicker.xaml
    /// </summary>
    public partial class GradientColorPicker : UserControl, INotifyPropertyChanged
    {

        private GradientStopCollection m_gradientStops = new GradientStopCollection() { new GradientStop(Colors.Transparent, 0), new GradientStop(ColorsReference.magentaBaseColor, 1) };

        private double m_offsetValue;

        public GradientStopCollection GradientStops
        {
            get => m_gradientStops;
            set
            {
                m_gradientStops = value;
                OnPropertyChanged(nameof(GradientStops));
            }
        }

        public double OffsetValue
        {
            get => m_offsetValue;
            set {
                m_offsetValue = value;
                OnPropertyChanged(nameof(OffsetValue));

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
    }
}
