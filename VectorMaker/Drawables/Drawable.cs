using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
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
        SelectionTool,
        EditPointSelectionTool,
        Rectangle,
        Geometry,
        Ellipse,
        Line,
        PolyLine,
        Polygon,
        Path,
    }

    public abstract class Drawable
    {
        protected System.Windows.Point m_startPoint;
        protected System.Windows.Point m_endPoint;
        protected Shape m_shape;
        private PathSettings m_pathSettings;
        protected TranslateTransform m_translateTransform;
        public bool IsControlKey = false;
        protected GeneralTransform m_inverseTranslateTransform => m_translateTransform.Inverse;

        public PathSettings Settings => m_pathSettings;

        public Drawable(PathSettings pathSettings)
        {
            m_pathSettings = pathSettings;
            m_pathSettings.OnValuesChange += OnSettingsChange;
        }

        public abstract void SetValueOfPoint(System.Windows.Point point);

        public Shape SetStartPoint(System.Windows.Point startPoint)
        {
            m_startPoint = startPoint;
            m_translateTransform = new TranslateTransform(m_startPoint.X, m_startPoint.Y);
            CreateGeometryBase();
            m_shape.RenderTransform = m_translateTransform;
            return m_shape;
        }

        protected abstract void CreateGeometry();
        public abstract void EndDrawing();
        public abstract void AddPointToCollection();

        private void OnSettingsChange()
        {
            SetPathSettings();
        }

        private void CreateGeometryBase()
        {
            CreateGeometry();
        }

        protected void SetPathSettings()
        {
            m_shape.Fill = m_pathSettings.Fill;
            m_shape.Stroke = m_pathSettings.Stroke;
            m_shape.StrokeThickness = m_pathSettings.StrokeThickness;
            m_shape.Visibility = m_pathSettings.Visibility;
            m_shape.VerticalAlignment = m_pathSettings.VerticalAlignment;
            m_shape.HorizontalAlignment = m_pathSettings.HorizontalAlignment;
        }

        protected void SetPathSettingsWithoutFill()
        {
            m_shape.Stroke = m_pathSettings.Stroke;
            m_shape.StrokeThickness = m_pathSettings.StrokeThickness;
            m_shape.Visibility = m_pathSettings.Visibility;
            m_shape.VerticalAlignment = m_pathSettings.VerticalAlignment;
            m_shape.HorizontalAlignment = m_pathSettings.HorizontalAlignment;
        }
    }
}
