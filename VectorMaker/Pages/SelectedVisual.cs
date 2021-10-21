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
            FrameworkElement frameworkElement = m_visual as FrameworkElement;
            Rect rect = new Rect(frameworkElement.RenderSize);
            m_selectionBox.Width = rect.Width;
            m_selectionBox.Height = rect.Height;
            m_selectionBox.RenderTransform = frameworkElement.RenderTransform;
            //m_mainCanvas.Children.Add(m_selectionBox);
        }

        public void RemoveSelection()
        {
           m_mainCanvas.Children.Remove(m_selectionBox);
            
        }
    }
}