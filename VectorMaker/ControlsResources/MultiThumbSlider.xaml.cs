using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private List<ThumbSliderAdorner> m_adorners = new();
        private static readonly DependencyProperty m_thumbType = DependencyProperty.Register("ThumbType", typeof(ThumbTypes), typeof(MultiThumbSlider), new PropertyMetadata(ThumbTypes.SimpleThumbType));
        private AdornerLayer m_adornerLayer;

        public List<ThumbSliderAdorner> Adorners => m_adorners.OrderBy(x => x.Offset).ToList();
        public ThumbTypes ThumbType
        {
            get { return (ThumbTypes)GetValue(m_thumbType); }
            set { SetValue(m_thumbType, value); }
        }
        public ThumbSliderAdorner SelectedThumb { get; set; }

        public event EventHandler SelectedThumbChanged;
        public event EventHandler AddedThumb;
        public event EventHandler DeletedThumb;

        public MultiThumbSlider()
        {
            InitializeComponent();
        }

        public ThumbSliderAdorner CreateThumb(double offset)
        {
            SetAdornerLayer();
            ThumbSliderAdorner adorner = CreateSpecialType(offset);
            m_adornerLayer.Add(adorner);
            m_adorners.Add(adorner);
            AddedThumb?.Invoke(this, new ThumbEventArgs(adorner));
            SetSelectedThumb(adorner);
            return adorner;
        }

        public GradientSliderAdorner CreateThumbWithGradient(GradientStop stop)
        {
            SetAdornerLayer();
            GradientSliderAdorner adorner = adorner = new GradientSliderAdorner(stop, this, stop.Offset, HandleSelectionChanged);
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
                        adorner = new ThumbSliderAdorner(this, offset, HandleSelectionChanged);
                        break;
                    }
                case ThumbTypes.GradientThumbType:
                    {
                        adorner = new GradientSliderAdorner(new GradientStop(Colors.White, offset), this, offset, HandleSelectionChanged);
                        break;
                    }
            }
            return adorner;
        }

        private void SetAdornerLayer()
        {
            if (m_adornerLayer == null)
                m_adornerLayer = Adorner.AdornerLayer;
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
            SetAdornerLayer();
            var window = Window.GetWindow(this);
            if(window!=null)
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
