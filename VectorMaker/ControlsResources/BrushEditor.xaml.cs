using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VectorMaker.Commands;
using ColorDef = System.Windows.Media.Color;

namespace VectorMaker.ControlsResources
{
    /// <summary>
    /// Logika interakcji dla klasy BrushEditor.xaml
    /// </summary>
    public partial class BrushEditor : UserControl, INotifyPropertyChanged
    {
        private bool m_isSolidChecked = true;
        private bool m_isLinearChecked = false;
        private bool m_isRadialChecked = false;
        private static DependencyProperty m_editedBrush = DependencyProperty.Register("EditedBrush", typeof(Brush), typeof(BrushEditor),new PropertyMetadata(Brushes.Transparent));

        public event PropertyChangedEventHandler PropertyChanged;

        public Brush EditedBrush
        {
            get { return (Brush)GetValue(m_editedBrush); }
            set
            {
                SetValue(m_editedBrush, value);
            }
        }

        public ColorDef EditedColor
        {
            get => (EditedBrush as SolidColorBrush).Color;
            set
            {
                EditedBrush = new SolidColorBrush(value);
                OnPropertyChanged(nameof(EditedColor));
                OnPropertyChanged(nameof(EditedBrush));
            }
        }

        public bool IsSolidChecked
        {
            get => m_isSolidChecked;
            set
            {
                m_isSolidChecked = value;
                OnPropertyChanged(nameof(IsSolidChecked));
            }
        }
        public bool IsLinearChecked
        {
            get => m_isLinearChecked;
            set
            {
                m_isLinearChecked = value;
                OnPropertyChanged(nameof(IsLinearChecked));
            }
        }
        public bool IsRadialChecked
        {
            get => m_isRadialChecked;
            set
            {
                m_isRadialChecked = value;
                OnPropertyChanged(nameof(IsRadialChecked));
            }
        }



        public BrushEditor()
        {
            InitializeComponent();
        }
        #region Methods
        private void SolidClick()
        {
            if (!IsSolidChecked) //if false
            {
                IsSolidChecked = !IsSolidChecked;
                IsLinearChecked = !IsSolidChecked;
                IsRadialChecked = !IsSolidChecked;
                ConvertToSolidBrush();
            }
        }

        private void LinearClick()
        {
            if (!IsLinearChecked)//if false
            {
                IsLinearChecked = !IsLinearChecked;
                IsSolidChecked = !IsLinearChecked;
                IsRadialChecked = !IsLinearChecked;
                ConvertToLinearGradientBrush();
            }
        }

        private void RadialClick()
        {
            if (!IsRadialChecked)//if false
            {
                IsRadialChecked = !IsRadialChecked;
                IsSolidChecked = !IsRadialChecked;
                IsLinearChecked = !IsRadialChecked;
                ConvertToRadialGradientBrush();
            }
        }

        private void ConvertToRadialGradientBrush()
        {
            LinearGradientBrush temp1 = EditedBrush as LinearGradientBrush;
            SolidColorBrush temp2 = EditedBrush as SolidColorBrush;
            if (temp1 != null)
                EditedBrush = new RadialGradientBrush(temp1.GradientStops);
            else if (temp2 != null)
            {
                EditedBrush = new RadialGradientBrush(new GradientStopCollection() { new GradientStop(temp2.Color, 0), new GradientStop(temp2.Color, 1) });
            }
            else
            {
                EditedBrush = new RadialGradientBrush(new GradientStopCollection() { new GradientStop(Colors.Transparent, 0), new GradientStop(Colors.White, 1) });
            }
        }

        private void ConvertToLinearGradientBrush()
        {
            RadialGradientBrush temp1 = EditedBrush as RadialGradientBrush;
            SolidColorBrush temp2 = EditedBrush as SolidColorBrush;
            if (temp1 != null)
                EditedBrush = new LinearGradientBrush((EditedBrush as RadialGradientBrush).GradientStops);
            else if (temp2 != null)
            {
                EditedBrush = new LinearGradientBrush(new GradientStopCollection() { new GradientStop(temp2.Color, 0), new GradientStop(temp2.Color, 1) });
            }
            else
            {
                EditedBrush = new LinearGradientBrush(new GradientStopCollection() { new GradientStop(Colors.Transparent, 0), new GradientStop(Colors.White, 1) });
            }
        }

        private void ConvertToSolidBrush()
        {
            GradientBrush temp = EditedBrush as GradientBrush;
            if (temp != null)
                EditedBrush = new SolidColorBrush(temp.GradientStops[0].Color);
            else
                EditedBrush = new SolidColorBrush(Colors.White);
        }
        #endregion
        #region Events
        private void RadialGradientColor_Click(object sender, RoutedEventArgs e)
        {
            RadialClick();
        }

        private void GradientColor_Click(object sender, RoutedEventArgs e)
        {
            LinearClick();
        }

        private void SolidColor_Click(object sender, RoutedEventArgs e)
        {
            SolidClick();
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private void ColorSliders_ColorChanged(object sender, RoutedEventArgs e)
        {
            EditedColor = (sender as ColorPicker.ColorSliders).SelectedColor;
        }
    }
}
