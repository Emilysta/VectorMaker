using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using VectorMaker.Utility;

namespace VectorMaker.ControlsResources
{
    public class ThumbEventArgs : RoutedEventArgs
    {
        public ThumbSliderAdorner NewThumb { get; set; }

        public ThumbEventArgs(ThumbSliderAdorner thumbSlider)
        {
            NewThumb = thumbSlider;
        }
    }

    public enum ThumbTypes
    {
        SimpleThumbType,
        GradientThumbType
    }


    /// <summary>
    /// Interaction logic for MultiThumbSlider.xaml
    /// </summary>
    /// 
    public partial class MultiThumbSlider : UserControl
    {
        private ObservableCollection<ThumbSliderAdorner> m_adorners = new();
        private static readonly DependencyProperty m_thumbType = DependencyProperty.Register("ThumbType", typeof(ThumbTypes), typeof(MultiThumbSlider), new PropertyMetadata(ThumbTypes.SimpleThumbType));
        private static readonly DependencyProperty m_editorBackground = DependencyProperty.Register("EditorBackground", typeof(Brush), typeof(MultiThumbSlider), new PropertyMetadata(Brushes.Transparent));
        private AdornerLayer m_adornerLayer;

        public ObservableCollection<ThumbSliderAdorner> Adorners => m_adorners;
        public ThumbTypes ThumbType
        {
            get { return (ThumbTypes)GetValue(m_thumbType); }
            set { SetValue(m_thumbType, value); }
        }

        public Brush EditorBackground
        {
            get { return (Brush)GetValue(m_editorBackground); }
            set { SetValue(m_editorBackground, value); }
        }
        public ThumbSliderAdorner SelectedThumb { get; set; }

        public event EventHandler SelectedThumbChanged;
        public event EventHandler AddedThumb;
        public event EventHandler DeletedThumb;

        public MultiThumbSlider()
        {
            InitializeComponent();
            m_adornerLayer = AdornerLayer.GetAdornerLayer(Rectangle);
        }

        public ThumbSliderAdorner CreateThumb(double offset)
        {
            ThumbSliderAdorner adorner = CreateSpecialType(offset);
            m_adornerLayer.Add(adorner);
            m_adorners.Add(adorner);
            SetSelectedThumb(adorner);
            AddedThumb?.Invoke(this, new ThumbEventArgs(adorner));
            return adorner;
        }

        public GradientSliderAdorner CreateThumbWithGradient(GradientStop stop)
        {
            GradientSliderAdorner adorner = adorner = new GradientSliderAdorner(stop, Rectangle, stop.Offset, HandleSelectionChanged);
            m_adornerLayer.Add(adorner);
            m_adorners.Add(adorner);
            SetSelectedThumb(adorner);
            return adorner;
        }

        public void DeleteThumbs()
        {
            foreach (Adorner adorner in m_adorners)
            {
                m_adornerLayer.Remove(adorner);
            }
        }

        public void DeleteSelectedThumb()
        {
            m_adornerLayer.Remove(SelectedThumb);
            DeletedThumb?.Invoke(this, new ThumbEventArgs(SelectedThumb));
            SelectedThumb = m_adorners.Last();
        }

        public void SetSelectedThumb(ThumbSliderAdorner adorner)
        {
            SelectedThumb = adorner;
        }

        private ThumbSliderAdorner CreateSpecialType(double offset)
        {
            ThumbSliderAdorner adorner = null;
            switch (ThumbType)
            {
                case ThumbTypes.SimpleThumbType:
                    {
                        adorner = new ThumbSliderAdorner(Rectangle, offset, HandleSelectionChanged);
                        break;
                    }
                case ThumbTypes.GradientThumbType:
                    {
                        adorner = new GradientSliderAdorner(new GradientStop(Colors.White, offset), Rectangle, offset, HandleSelectionChanged);
                        break;
                    }
            }
            return adorner;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement element = sender as UIElement;
            Point point = e.GetPosition(element);
            double offsetX = point.X / element.RenderSize.Width;
            CreateThumb(offsetX);
        }

        private void HandleSelectionChanged(ThumbSliderAdorner adorner)
        {
            SelectedThumb = adorner;
            SelectedThumbChanged.Invoke(this, EventArgs.Empty);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
                window.KeyDown += UserControl_KeyDown;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Delete:
                    {
                        DeleteSelectedThumb();
                        break;
                    }
            }
        }
    }
}
