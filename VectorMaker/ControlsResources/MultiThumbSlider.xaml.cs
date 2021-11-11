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

    /// <summary>
    /// Interaction logic for MultiThumbSlider.xaml
    /// </summary>
    /// 
    public partial class MultiThumbSlider : UserControl
    {
        private List<ThumbSliderAdorner> m_adorners = new();

        public List<ThumbSliderAdorner> Adorners => m_adorners.OrderBy(x => x.Offset).ToList();

        public ThumbSliderAdorner SelectedThumb { get; set; }

        public event EventHandler SelectedThumbChanged;
        public event EventHandler AddedThumb;

        public MultiThumbSlider()
        {
            InitializeComponent();
        }

        public ThumbSliderAdorner CreateThumb(double offset)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
            ThumbSliderAdorner adorner = new ThumbSliderAdorner(this, offset, HandleSelectionChanged);
            adornerLayer.Add(adorner);
            m_adorners.Add(adorner);
            AddedThumb?.Invoke(this, new ThumbEventArgs(adorner));
            return adorner;
        }

        private void Rectangle_MouseDown(object sender, MouseButtonEventArgs e)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(this);
            ThumbSliderAdorner adorner = new ThumbSliderAdorner(this, 0, HandleSelectionChanged);
            adornerLayer.Add(adorner);
            m_adorners.Add(adorner);
            AddedThumb?.Invoke(this, new ThumbEventArgs(adorner));
        }

        private void HandleSelectionChanged(ThumbSliderAdorner adorner)
        {
            SelectedThumb = adorner;
            SelectedThumbChanged.Invoke(this, EventArgs.Empty);
        }
    }
}
