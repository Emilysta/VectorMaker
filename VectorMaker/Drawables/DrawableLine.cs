using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VectorMaker.Drawables
{
    public class DrawableLine : Drawable
    {
        Line m_line => (Line)m_shape;
        public DrawableLine() : base() { }

        public override void SetValueOfPoint(Point point)
        {
            Point translated = m_shape.RenderTransform.Inverse.Transform(point);
            m_line.X2 = translated.X;
            m_line.Y2 = translated.Y;
            m_endPoint = translated;
        }

        protected override void CreateGeometry()
        {
            m_shape = new Line();
            m_line.X1 = 0;
            m_line.X2 = 0;
            m_line.Y1 = 0;
            m_line.Y2 = 0;
            SetPathSettingsWithoutFill();
        }

        public override void EndDrawing()
        {
            Vector vector = new Vector(m_endPoint.X, m_endPoint.Y);
            vector.Negate();

            m_line.X1 = 0;
            m_line.Y1 = 0;
            if (m_endPoint.X < 0 || m_endPoint.Y < 0)
            {
                m_line.X2 = vector.X;
                m_line.Y2 = vector.Y;
                Matrix matrix = m_matrixTransform.Value;
                matrix.OffsetX -= vector.X;
                matrix.OffsetY -= vector.Y;
                m_matrixTransform.Matrix = matrix;

            }

        }

        public override void AddPointToCollection() { }

    }
}
