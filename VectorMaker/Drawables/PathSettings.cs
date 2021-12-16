using System;
using System.Windows;
using System.Windows.Media;

namespace VectorMaker.Drawables
{
    /// <summary>
    /// This is class set of visual attributes of object.
    /// </summary>
    public class PathSettings
    {
        /// <summary>
        /// Action. Invoked when one of properties changes.
        /// </summary>
        public event Action OnValuesChange;

        private Brush m_fill = Brushes.Black;
        private Brush m_stroke = Brushes.Red;
        private float m_strokeThickness = 1f;
        private Visibility m_visibility = Visibility.Visible;
        private VerticalAlignment m_verticalAlignment;
        private HorizontalAlignment m_horizontalAlignment;

        /// <summary>
        /// Color of object's fill - <see cref="Brush"/>.
        /// </summary>
        public Brush Fill
        {
            get { return m_fill; }
            set
            {
                m_fill = value;
                OnValuesChange?.Invoke();
            }
        }

        /// <summary>
        /// Color of object's stroke - <see cref="Brush"/>.
        /// </summary>
        public Brush Stroke
        {
            get { return m_stroke; }
            set
            {
                m_stroke = value;
                OnValuesChange?.Invoke();
            }
        }


        /// <summary>
        /// Thickness of object's stroke
        /// </summary>
        public float StrokeThickness
        {
            get { return m_strokeThickness; }
            set
            {
                m_strokeThickness = value;
                OnValuesChange?.Invoke();
            }
        }

        /// <summary>
        /// Object visibility - <see cref="Visibility"/>.
        /// </summary>
        public Visibility Visibility
        {
            get { return m_visibility; }
            set
            {
                m_visibility = value;
                OnValuesChange?.Invoke();
            }
        }

        /// <summary>
        /// Object vertical alignement - <see cref="VerticalAlignment"/>.
        /// </summary>
        public VerticalAlignment VerticalAlignment
        {
            get { return m_verticalAlignment; }
            set
            {
                m_verticalAlignment = value;
                OnValuesChange?.Invoke();
            }
        }

        /// <summary>
        /// Object horizontal alignement - <see cref="HorizontalAlignment"/>.
        /// </summary>
        public HorizontalAlignment HorizontalAlignment
        {
            get { return m_horizontalAlignment; }
            set
            {
                m_horizontalAlignment = value;
                OnValuesChange?.Invoke();
            }
        }
        /// <summary>
        /// Static method with default object settings.
        /// </summary>
        /// <returns>new <see cref="PathSettings"/></returns>
        public static PathSettings Default()
        {
            return new PathSettings();
        }

        /// <summary>
        /// Static method with settings that are needed to visualize selection box
        /// </summary>
        /// <returns> <see cref="PathSettings"/></returns>
        public static PathSettings SelectionSettings()
        {
            PathSettings pathSettings = new PathSettings();
            pathSettings.Fill = Brushes.Transparent;
            pathSettings.Stroke = Brushes.LightGray;
            pathSettings.StrokeThickness = 1f;
            return pathSettings;
        }
    }
}
