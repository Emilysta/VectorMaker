using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace VectorMaker.Pages
{
    public class SelectedVisual
    {

        private Rectangle m_selectionBox = new Rectangle();
        private Visual m_visual;
        private Canvas m_mainCanvas;

        public SelectedVisual(Visual visual, Canvas mainCanvas)
        {
            if (visual == null)
                throw new Exception("visual was null");
            m_visual = visual;
            m_mainCanvas = mainCanvas;
            CreateSelectionBox();
        }

        private void CreateSelectionBox()
        {
            m_selectionBox.Stroke = Brushes.White;
            if (m_visual is Canvas)
            {
                FrameworkElement frameworkElement = m_visual as FrameworkElement;
                Rect rect = new Rect(frameworkElement.RenderSize);
                m_selectionBox.Width = rect.Width;
                m_selectionBox.Height = rect.Height;
                m_selectionBox.RenderTransform = frameworkElement.RenderTransform;
            }
            else
            {
                Path path = m_visual as Path;
                Geometry geometry = path.Data;
                TransformGroup transformGroup = new TransformGroup();
                if (geometry != null)
                {
                    Rect rect = geometry.Bounds;
                    m_selectionBox.Width = rect.Width + path.StrokeThickness*2;
                    m_selectionBox.Height = rect.Height + path.StrokeThickness*2;
                    transformGroup.Children.Add(new TranslateTransform(rect.X-path.StrokeThickness, rect.Y - path.StrokeThickness));
                }
                else
                {
                    m_selectionBox.Width = path.RenderSize.Width + path.StrokeThickness;
                    m_selectionBox.Height = path.RenderSize.Height + path.StrokeThickness;
                }
                transformGroup.Children.Add(path.RenderTransform);
                m_selectionBox.RenderTransform = transformGroup;
                //rectangle.RenderTransform = transformGroup;
            }
            m_mainCanvas.Children.Add(m_selectionBox);
        }

        public void RemoveSelection()
        {
           m_mainCanvas.Children.Remove(m_selectionBox);
        }
    }
}