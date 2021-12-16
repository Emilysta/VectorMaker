using System.Windows.Media;
using System.Windows.Shapes;
using VectorMaker.Models;

namespace VectorMaker.Drawables
{
    /// <summary>
    /// Defines enum of segements types.
    /// </summary>
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

    /// <summary>
    /// Defines enum of objects that can be drawn.
    /// </summary>
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
    /// <summary>
    /// This is a abstarct, base class for object that can be drawn.
    /// </summary>
    public abstract class Drawable
    {
        /// <summary>
        /// Defines start point of this object
        /// </summary>
        protected System.Windows.Point m_startPoint;
        /// <summary>
        /// Defines end point of this object
        /// </summary>
        protected System.Windows.Point m_endPoint;
        /// <summary>
        /// Reference to Shape object that is drawn
        /// </summary>
        protected Shape m_shape;
        /// <summary>
        /// Reference to matrix transform of m_shape
        /// </summary>
        protected MatrixTransform m_matrixTransform;
        /// <summary>
        /// Inverse transfor for m_matrix transform
        /// </summary>
        protected GeneralTransform m_inverseTranslateTransform => m_matrixTransform.Inverse;

        /// <summary>
        /// Constructor
        /// </summary>
        public Drawable() { }

        /// <summary>
        /// Method that creates m_shape and sets it's atrtributes.<br/>
        /// <param name="startPoint"> <paramref name="startPoint"/> - point of mouse in canvas or other object</param>
        /// </summary>
        public Shape StartDrawing(System.Windows.Point startPoint)
        {
            m_startPoint = startPoint;
            m_matrixTransform = new MatrixTransform(1,0,0,1,m_startPoint.X, m_startPoint.Y);
            CreateGeometryBase();
            m_shape.RenderTransform = m_matrixTransform;
            return m_shape;
        }

        /// <summary>
        /// Abstarct method that should handle drawing of specific shape<br/>
        /// </summary>
        public abstract void EndDrawing();

        /// <summary>
        /// Abstarct method that should handle adding new point to specific shape - if needed<br/>
        /// </summary>
        public abstract void AddPointToCollection();

        /// <summary>
        /// Abstarct method that should handle setting current point to specific shape - if needed<br/>
        /// <param name="point"> <paramref name="point"/> - point of mouse in canvas or other object</param>
        /// </summary>
        public abstract void SetValueOfPoint(System.Windows.Point point);

        /// <summary>
        /// Abstarct method that should create exact shape<br/>
        /// </summary>
        protected abstract void CreateGeometry();

        /// <summary>
        /// Handler for creating specific geometry, can do more.
        /// </summary>
        private void CreateGeometryBase()
        {
            CreateGeometry();
        }

        /// <summary>
        ///Method that gets shape properties from global <see cref="ShapeProperties"/> and sets attributes of drawn shape.
        /// </summary>
        protected void SetPathSettings()
        {
            m_shape.Fill = new SolidColorBrush(ShapeProperties.Instance.FillBrush.Color);
            m_shape.Stroke = new SolidColorBrush(ShapeProperties.Instance.StrokeBrush.Color);
            m_shape.StrokeThickness = ShapeProperties.Instance.StrokeThickness;
            if (ShapeProperties.Instance.StrokeDashArray != null)
                m_shape.StrokeDashArray = new DoubleCollection(ShapeProperties.Instance.StrokeDashArray);
        }

        /// <summary>
        ///Method that gets shape properties from global <see cref="ShapeProperties"/> and sets attributes of drawn shape without fill.
        /// </summary>
        protected void SetPathSettingsWithoutFill()
        {
            m_shape.Stroke = new SolidColorBrush(ShapeProperties.Instance.StrokeBrush.Color);
            m_shape.StrokeThickness = ShapeProperties.Instance.StrokeThickness;
            if(ShapeProperties.Instance.StrokeDashArray!=null)
                m_shape.StrokeDashArray = new DoubleCollection(ShapeProperties.Instance.StrokeDashArray);

        }
    }
}
