using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace VectorMaker.Drawables
{
    /// <summary>
    /// This is class realizes drawing of <see cref="Ellipse"/>.
    /// </summary>
    public class DrawableEllipse : Drawable
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DrawableEllipse() : base() { }

        /// <summary>
        /// Overridden method of <see cref="Drawable.SetValueOfPoint(Point)"/>.<br/>
        /// <param name="point"> <paramref name="point"/> - point of mouse in canvas or other object</param>
        /// </summary>
        public override void SetValueOfPoint(Point point)
        {
            Rect rect = new Rect(m_startPoint, point);
            m_shape.Width = rect.Width;
            
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                m_shape.Height = rect.Width;
            else
                m_shape.Height = rect.Height;
            m_endPoint = point;
        }

        /// <summary>
        /// Overridden method of <see cref="Drawable.EndDrawing"/>.
        /// Ellipse does not need special handler of ending, so it's basically nothing.
        /// </summary>
        public override void EndDrawing() { }

        /// <summary>
        /// Overridden method of <see cref="Drawable.AddPointToCollection"/>.
        /// Ellipse does not need special handler of adding point, so it's basically nothing.
        /// </summary>
        public override void AddPointToCollection() { }

        /// <summary>
        /// Overridden method of <see cref="Drawable.CreateGeometry"/>.
        /// Creates <see cref="Ellipse"/> shape from "N:System.Windows.Shapes".
        /// </summary>
        protected override void CreateGeometry()
        {
            Rect rect = new Rect(m_startPoint, m_startPoint);
            m_shape = new Ellipse();
            m_shape.Width = rect.Width;
            m_shape.Height = rect.Height;
            SetPathSettings();
        }
    }
}
