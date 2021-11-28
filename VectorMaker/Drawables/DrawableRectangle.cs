using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace VectorMaker.Drawables
{
    public class DrawableRectangle : Drawable
    {
        private PathSettings m_settings;
        public DrawableRectangle() : base() {}
        public DrawableRectangle(PathSettings pathSettings) : base() 
        { m_settings = pathSettings; }

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

        public override void EndDrawing() { }

        public override void AddPointToCollection() { }

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
