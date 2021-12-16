using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace VectorMaker.Drawables
{
    /// <summary>
    /// This is class realizes drawing of <see cref="Rectangle"/>.
    /// </summary>
    public class DrawableRectangle : Drawable
    {
        private PathSettings m_settings;
        /// <summary>
        /// Constructor.
        /// </summary>
        public DrawableRectangle() : base() {}

        /// <summary>
        /// Constructor.
        /// <param name="pathSettings"><paramref name="pathSettings"/>Color,stroke attributtes. See: <see cref="PathSettings"/></param>
        /// </summary>
        public DrawableRectangle(PathSettings pathSettings) : base() 
        { m_settings = pathSettings; }

        /// <summary>
        /// Overridden method of <see cref="Drawable.SetValueOfPoint(Point)"/>.<br/>
        /// <param name="point"> <paramref name="point"/> - point of mouse in canvas or other object</param>
        /// </summary>
        public override void SetValueOfPoint(Point point)
        {
            Rect rect = new Rect(m_startPoint, point);
            m_shape.Width = rect.Width;
            if(Keyboard.IsKeyDown(Key.LeftCtrl))
                m_shape.Height = rect.Width;
            else
                m_shape.Height = rect.Height;
            m_endPoint = point;
        }

        /// <summary>
        /// Overridden method of <see cref="Drawable.EndDrawing"/>.
        /// Rectangle does not need special handler of ending, so it's basically nothing.
        /// </summary>
        public override void EndDrawing() { }

        /// <summary>
        /// Overridden method of <see cref="Drawable.AddPointToCollection"/>.
        /// Rectangle does not need special handler of adding point, so it's basically nothing.
        /// </summary>
        public override void AddPointToCollection() { }

        /// <summary>
        /// Overridden method of <see cref="Drawable.CreateGeometry"/>.
        /// Creates <see cref="Rectangle"/> shape from "N:System.Windows.Shapes".
        /// </summary>
        protected override void CreateGeometry()
        {
            Rect rect = new Rect(m_startPoint, m_startPoint);
            m_shape = new Rectangle();
            m_shape.Width = rect.Width;
            m_shape.Height = rect.Height;
            SetPathSettings();
            if (m_settings != null)
            {
                m_shape.Fill = m_settings.Fill;
                m_shape.Stroke = m_settings.Stroke;
                m_shape.StrokeThickness = m_settings.StrokeThickness;
                m_shape.StrokeDashArray = null;
            }
        }
    }
}
