using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace VectorMaker.Utility
{
    /// <summary>
    /// This class defines adorner for <see cref="ControlsResources.MultiThumbSlider"/>.
    /// </summary>
    public class ThumbSliderAdorner : Adorner
    {
        public event Action<double> OnValueChanged;

        private Thumb m_thumb;
        private const int m_thumbWidth = 6;
        private const int m_thumbHeight = 10;
        private double m_pixelOffset = 0;
        private VisualCollection m_visualCollection;
        private Action<ThumbSliderAdorner> m_action;
        private FrameworkElement m_element;
        public double Offset => (double)(m_pixelOffset/ m_element.Width);

        public ThumbSliderAdorner(UIElement adornedElement,double offset,Action<ThumbSliderAdorner> action) : base(adornedElement)
        {
            m_element = adornedElement as FrameworkElement;
            m_visualCollection = new VisualCollection(this);
            m_pixelOffset = offset*m_element.Width;
            m_action = action;
            SetThumb();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            m_thumb?.Arrange(new Rect(m_pixelOffset-m_thumbWidth/2, finalSize.Height, m_thumbWidth, m_thumbHeight));
            return finalSize;
        }
        private void SetThumb()
        {
            m_thumb = new Thumb();
            m_thumb.DragStarted += (_,_) => m_action?.Invoke(this);
            m_thumb.Style = (Style)Application.Current.Resources["MultiThumbSliderStyle"];
            m_thumb.Width = m_thumbWidth;
            m_thumb.Height = m_thumbHeight;
            m_thumb.ApplyTemplate();
            m_thumb.DragDelta += ThumbDragDelta;
            m_thumb.Cursor = Cursors.Hand;
            m_visualCollection.Add(m_thumb);
        }

        private void ThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            double sum = m_pixelOffset + e.HorizontalChange;
            if (sum >= 0 && sum <= m_element.Width)
                m_pixelOffset += e.HorizontalChange;
            else if (sum < 0)
                m_pixelOffset = 0;
            else
                m_pixelOffset = m_element.Width;
            OnValueChanged?.Invoke(Offset);
            this.InvalidateVisual();
        }

        protected override Visual GetVisualChild(int index)
        {
            return m_visualCollection[index];
        }

        protected override int VisualChildrenCount
        {
            get { return m_visualCollection.Count; }
        }
    }
}
