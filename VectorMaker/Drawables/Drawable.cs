using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VectorMaker.Drawables
{
    public enum PathTypes
    {
        None,
        ArcSegment,
        BezierSegment,
        LineSegment,
        PolyBezierSegment,
        PolyLineSegment,
        PolyQuadraticBezierSegment,
        QuadraticBezierSegment
    }

    public enum DrawableTypes
    { 
        None,
        Rectangle,
        Geometry,
        Ellipse,
        Path
    }

    public abstract class Drawable
    {
        protected abstract Geometry m_geometryToDraw { get; }
        protected System.Windows.Point m_startPoint;
        protected System.Windows.Point m_endPoint;
        protected Path m_path;
        private PathSettings m_pathSettings;

        public PathSettings Settings => m_pathSettings;

        public Drawable(PathSettings pathSettings)
        {
            m_pathSettings = pathSettings;
            m_pathSettings.OnValuesChange += OnSettingsChange;
        }

        public abstract void AddPointToList(System.Windows.Point point);

        public Path SetStartPoint(System.Windows.Point startPoint)
        {
            m_startPoint = startPoint;
            CreateGeometry();
            m_path = new Path();
            m_path.Data = m_geometryToDraw;
            SetPathSettings();
            return m_path;
        }

        protected abstract void CreateGeometry();

        private void OnSettingsChange()
        {
            SetPathSettings();
        }

        private void SetPathSettings()
        {
            m_path.Fill = m_pathSettings.Fill;
            m_path.Stroke = m_pathSettings.Stroke;
            m_path.StrokeThickness = m_pathSettings.StrokeThickness;
            m_path.Visibility = m_pathSettings.Visibility;
            m_path.VerticalAlignment = m_pathSettings.VerticalAlignment;
            m_path.HorizontalAlignment = m_pathSettings.HorizontalAlignment;
        }
    }
}
