using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace VectorMaker.Drawables
{
    /// <summary>
    /// This is class realizes drawing of <see cref="Line"/>.
    /// </summary>
    public class DrawableLine : Drawable
    {
        /// <summary>
        /// Reference to <see cref="Line"/> object.
        /// </summary>
        Line m_line => (Line)m_shape;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DrawableLine() : base() { }

        /// <summary>
        /// Overridden method of <see cref="Drawable.SetValueOfPoint(Point)"/>.<br/>
        /// <param name="point"> <paramref name="point"/> - point of mouse in canvas or other object</param>
        /// </summary>
        public override void SetValueOfPoint(Point point)
        {
            Point translated = m_shape.RenderTransform.Inverse.Transform(point);
            m_line.X2 = translated.X;
            m_line.Y2 = translated.Y;
            m_endPoint = translated;
        }

        /// <summary>
        /// Overridden method of <see cref="Drawable.CreateGeometry"/>.<br/>
        /// Creates <see cref="Line"/> shape from "N:System.Windows.Shapes".
        /// </summary>
        protected override void CreateGeometry()
        {
            m_shape = new Line();
            m_line.X1 = 0;
            m_line.X2 = 0;
            m_line.Y1 = 0;
            m_line.Y2 = 0;
            SetPathSettingsWithoutFill();
        }

        /// <summary>
        /// Overridden method of <see cref="Drawable.EndDrawing"/>.<br/>
        /// Line need special handle of ending drawing, <br/>so it contains making translation on <see cref="Drawable.m_endPoint"/>.
        /// </summary>
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

        /// <summary>
        /// Overridden method of <see cref="Drawable.AddPointToCollection"/>.
        /// Line does not need special handler of adding point, so it's basically nothing.
        /// </summary>
        public override void AddPointToCollection() { }

    }
}
