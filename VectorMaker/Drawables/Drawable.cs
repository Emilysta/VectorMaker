using System.Windows.Media;
using System.Windows.Shapes;
using VectorMaker.Models;

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
        Line,
        PolyLine,
        Polygon,
        Path,
        Text
    }

    public abstract class Drawable
    {
        protected System.Windows.Point m_startPoint;
        protected System.Windows.Point m_endPoint;
        protected Shape m_shape;
        protected TranslateTransform m_translateTransform;
        protected GeneralTransform m_inverseTranslateTransform => m_translateTransform.Inverse;

        public Drawable() { }

        public Shape StartDrawing(System.Windows.Point startPoint)
        {
            m_startPoint = startPoint;
            m_translateTransform = new TranslateTransform(m_startPoint.X, m_startPoint.Y);
            CreateGeometryBase();
            m_shape.RenderTransform = m_translateTransform;
            return m_shape;
        }
 
        public abstract void EndDrawing();
        public abstract void AddPointToCollection();
        public abstract void SetValueOfPoint(System.Windows.Point point);
        protected abstract void CreateGeometry();
        private void CreateGeometryBase()
        {
            CreateGeometry();
        }
        protected void SetPathSettings()
        {
            m_shape.Fill = new SolidColorBrush(ShapeProperties.Instance.FillBrush.Color);
            m_shape.Stroke = new SolidColorBrush(ShapeProperties.Instance.StrokeBrush.Color);
            m_shape.StrokeThickness = ShapeProperties.Instance.StrokeThickness;
            if (ShapeProperties.Instance.StrokeDashArray != null)
                m_shape.StrokeDashArray = new DoubleCollection(ShapeProperties.Instance.StrokeDashArray);
        }
        protected void SetPathSettingsWithoutFill()
        {
            m_shape.Stroke = new SolidColorBrush(ShapeProperties.Instance.StrokeBrush.Color);
            m_shape.StrokeThickness = ShapeProperties.Instance.StrokeThickness;
            if(ShapeProperties.Instance.StrokeDashArray!=null)
                m_shape.StrokeDashArray = new DoubleCollection(ShapeProperties.Instance.StrokeDashArray);

        }
    }
}
