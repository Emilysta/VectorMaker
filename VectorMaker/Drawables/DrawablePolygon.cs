using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Media = System.Windows.Media;

namespace VectorMaker.Drawables
{
    /// <summary>
    /// This is class realizes drawing of <see cref="Polygon"/>.
    /// </summary>
    public class DrawablePolygon : Drawable
    {
        /// <summary>
        /// <see cref="PointCollection"/> for m_polygon. 
        /// </summary>
        private Media.PointCollection m_pointsCollection = new Media.PointCollection();

        /// <summary>
        /// Temporary point.
        /// </summary>
        private Point m_tempPoint;

        /// <summary>
        /// Reference to <see cref="Polygon"/> object.
        /// </summary>
        private Polygon m_polygon => (Polygon)m_shape;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DrawablePolygon() : base() { }

        /// <summary>
        /// Overridden method of <see cref="Drawable.SetValueOfPoint(Point)"/>.<br/>
        /// <param name="point"> <paramref name="point"/> - point of mouse in canvas or other object</param>
        /// </summary>
        public override void SetValueOfPoint(Point point)
        {
            Point translated = m_inverseTranslateTransform.Transform(point);
            m_pointsCollection.RemoveAt(m_pointsCollection.Count - 1);
            m_tempPoint = translated;
            m_pointsCollection.Add(m_tempPoint);
        }

        /// <summary>
        /// Overridden method of <see cref="Drawable.EndDrawing"/>.<br/>
        /// Polygon need special handle of ending drawing, <br/>so it contains removing current point from collection.
        /// </summary>
        public override void EndDrawing()
        {
            m_pointsCollection.RemoveAt(m_pointsCollection.Count - 1);
        }

        /// <summary>
        /// Overridden method of <see cref="Drawable.CreateGeometry"/>.<br/>
        /// Creates <see cref="Polygon"/> shape from "N:System.Windows.Shapes"
        /// </summary>
        protected override void CreateGeometry()
        {
            m_shape = new Polygon();
            m_polygon.Points = m_pointsCollection;
            m_pointsCollection.Add(new Point(0, 0));
            m_tempPoint = new Point(0, 0);
            m_pointsCollection.Add(m_tempPoint);
            SetPathSettings();
        }

        /// <summary>
        /// Overridden method of <see cref="Drawable.AddPointToCollection"/>.
        /// Polygon need special handle of adding point to collection, <br/>so it contains addding m_tempPoint to collection.
        /// </summary>
        public override void AddPointToCollection()
        {
            m_pointsCollection.Add(m_tempPoint);
        }
    }
}
