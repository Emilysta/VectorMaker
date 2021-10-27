using System.Windows;
using System.Windows.Shapes;

namespace VectorMaker.Drawables
{
    public class DrawableEllipse : Drawable
    {
        public DrawableEllipse(PathSettings pathSettings) : base(pathSettings) { }

        public override void SetValueOfPoint(Point point)
        {
            Rect rect = new Rect(m_startPoint, point);
            m_shape.Width = rect.Width;
            m_shape.Height = rect.Height;
            m_endPoint = point;
        }

        public override void EndDrawing() { }

        public override void AddPointToCollection() { }

        protected override void CreateGeometry()
        {
            Rect rect = new Rect(m_startPoint, m_startPoint);
            m_shape = new Ellipse();
            m_shape.Width = rect.Width;
            m_shape.Height = rect.Height;
            SetPathSettings();
        }
    }
}
