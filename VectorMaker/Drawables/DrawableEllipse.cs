using System.Windows;
using System.Windows.Media;

namespace VectorMaker.Drawables
{
    public class DrawableEllipse : Drawable
    {
        private EllipseGeometry m_ellipseGeometry;
        private Point m_centerPoint;
        private double m_radiusX;
        private double m_radiusY;

        public Point CenterPoint => m_centerPoint;
        public double RadiusX => m_radiusX;
        public double RadiusY => m_radiusY;
        protected override Geometry m_geometryToDraw => m_ellipseGeometry;

        public DrawableEllipse(PathSettings pathSettings) : base(pathSettings) { }

        public override void AddPointToList(Point point)
        {
            Rect rect = new Rect(m_startPoint, point);
            m_ellipseGeometry.RadiusX = rect.Width / 2.0f;
            m_ellipseGeometry.RadiusY = rect.Height / 2.0f;
            m_endPoint = point;
            m_radiusX = m_ellipseGeometry.RadiusX;
            m_radiusY = m_ellipseGeometry.RadiusY;
        }

        protected override void CreateGeometry()
        {
            m_ellipseGeometry = new EllipseGeometry(new Rect(m_startPoint, m_startPoint));
            m_radiusX = m_ellipseGeometry.RadiusX;
            m_radiusY = m_ellipseGeometry.RadiusY;
        }
    }
}
