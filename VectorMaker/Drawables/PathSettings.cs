using System;
using System.Windows;
using System.Windows.Media;

namespace VectorMaker.Drawables
{
    public class PathSettings
    {
        public event Action OnValuesChange;

        private Brush m_fill = Brushes.Black;
        private Brush m_stroke = Brushes.Red;
        private float m_strokeThickness = 1f;
        private Visibility m_visibility = Visibility.Visible;
        private VerticalAlignment m_verticalAlignment;
        private HorizontalAlignment m_horizontalAlignment;

        public Brush Fill
        {
            get { return m_fill; }
            set
            {
                m_fill = value;
                OnValuesChange?.Invoke();
            }
        }
        public Brush Stroke
        {
            get { return m_stroke; }
            set
            {
                m_stroke = value;
                OnValuesChange?.Invoke();
            }
        }

        public float StrokeThickness
        {
            get { return m_strokeThickness; }
            set
            {
                m_strokeThickness = value;
                OnValuesChange?.Invoke();
            }
        }
        public Visibility Visibility
        {
            get { return m_visibility; }
            set
            {
                m_visibility = value;
                OnValuesChange?.Invoke();
            }
        }
        public VerticalAlignment VerticalAlignment
        {
            get { return m_verticalAlignment; }
            set
            {
                m_verticalAlignment = value;
                OnValuesChange?.Invoke();
            }
        }
        public HorizontalAlignment HorizontalAlignment
        {
            get { return m_horizontalAlignment; }
            set
            {
                m_horizontalAlignment = value;
                OnValuesChange?.Invoke();
            }
        }
    }
}
