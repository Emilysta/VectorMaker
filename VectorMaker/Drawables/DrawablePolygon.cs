using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Media = System.Windows.Media;

namespace VectorMaker.Drawables
{
    public class DrawablePolygon : Drawable
    {
        private Media.PointCollection m_pointsCollection = new Media.PointCollection();
        private Point m_tempPoint;
        private Polygon m_polygon => (Polygon)m_shape;

        public DrawablePolygon() : base() { }

        public override void SetValueOfPoint(Point point)
        {
            Point translated = m_inverseTranslateTransform.Transform(point);
            m_pointsCollection.RemoveAt(m_pointsCollection.Count - 1);
            m_tempPoint = translated;
            m_pointsCollection.Add(m_tempPoint);
        }

        public override void EndDrawing()
        {
            m_pointsCollection.RemoveAt(m_pointsCollection.Count - 1);
        }

        protected override void CreateGeometry()
        {
            m_shape = new Polygon();
            m_polygon.Points = m_pointsCollection;
            m_pointsCollection.Add(new Point(0, 0));
            m_tempPoint = new Point(0, 0);
            m_pointsCollection.Add(m_tempPoint);
            SetPathSettings();
        }

        public override void AddPointToCollection()
        {
            m_pointsCollection.Add(m_tempPoint);
        }
    }
}
