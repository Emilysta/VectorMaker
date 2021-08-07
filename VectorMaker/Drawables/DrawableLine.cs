using System.Windows;
using System.Windows.Media;

namespace VectorMaker.Drawables
{
    public class DrawableLine : Drawable
    {
        private LineGeometry m_lineGeometry;

        protected override Geometry m_geometryToDraw => m_lineGeometry;
        
        public DrawableLine(PathSettings pathSettings) : base(pathSettings) {}

        public override void AddPointToList(Point point)
        {
            m_lineGeometry.EndPoint = point;
            m_endPoint = point;
        }

        protected override void CreateGeometry()
        {
            m_lineGeometry = new LineGeometry();
            m_lineGeometry.StartPoint = m_startPoint;
            m_lineGeometry.EndPoint = m_startPoint;
        }
    }
}
