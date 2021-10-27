using System.Windows;
using System.Windows.Media;

namespace VectorMaker.Drawables
{
    public class DrawableRectangle : Drawable
    {
        private RectangleGeometry m_rectangleGeometry;
        private Size m_size;

        protected override Geometry m_geometryToDraw => m_rectangleGeometry;

        public DrawableRectangle(PathSettings pathSettings) : base(pathSettings) {}

        public override void AddPointToList(Point point)
        {
            Rect rect = new Rect(m_startPoint, point);
            m_rectangleGeometry.Rect = new Rect(0,0,rect.Width,rect.Height);
            m_endPoint = point;
            m_size = m_rectangleGeometry.Rect.Size;
        }

        protected override void CreateGeometry()
        {
            m_rectangleGeometry = new RectangleGeometry();
            Rect rect = new Rect(m_startPoint, m_startPoint);
            m_rectangleGeometry.Rect = new Rect(0, 0, rect.Width, rect.Height);
            m_size = m_rectangleGeometry.Rect.Size;
        }

        public Point? GetEndPoint()
        {
            if (m_endPoint.X == 0 && m_endPoint.Y == 0)
                return null;
            return m_endPoint;
        }
    }
}
