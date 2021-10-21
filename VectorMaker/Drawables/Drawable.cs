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
        Line
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
            m_path = new Path();
            CreateGeometryBase();
            m_path.Data = m_geometryToDraw;
            SetPathSettings();
            return m_path;
        }

        protected abstract void CreateGeometry();

        private void OnSettingsChange()
        {
            SetPathSettings();
        }

        private void CreateGeometryBase()
        {
            m_path.MouseLeftButtonDown += SelectObject;
            CreateGeometry();
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

        private void SelectObject(object sender, MouseButtonEventArgs e)
        {
            Trace.WriteLine("Object Clicked " + m_geometryToDraw.ToString());
            MainWindow.Instance.SelectedObejct = m_path;
        }

        public Geometry GetGeometry()
        {
            return m_geometryToDraw;
        }
    }
}
