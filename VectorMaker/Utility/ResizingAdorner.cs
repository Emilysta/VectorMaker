using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace VectorMaker.Utility
{
    public class ResizingAdorner : Adorner
    {
        private readonly Thumb m_rightBottomThumb;
        private readonly Thumb m_dragThumb;
        private readonly VisualCollection m_visualCollection;
        private FrameworkElement m_adornedElement;
        private AdornerLayer m_myLayer;

        public ResizingAdorner(UIElement adornedElement, AdornerLayer myLayer) : base(adornedElement)
        {
            m_visualCollection = new VisualCollection(this);
            m_rightBottomThumb = CreateThumb(RightBottomThumbDrag, Cursors.SizeNWSE);
            m_dragThumb = new Thumb();
            m_dragThumb.Width = m_dragThumb.Height = 10;
            m_dragThumb.BorderBrush = Brushes.CadetBlue;
            m_dragThumb.DragDelta += RightBottomThumbDrag;
            m_visualCollection.Add(m_dragThumb);
            adornedElement = (FrameworkElement)AdornedElement;
            m_myLayer = myLayer;
        }
        private Thumb CreateThumb(DragDeltaEventHandler dragDeltaEventHandler, Cursor cursor)
        {
            Thumb thumb = new Thumb();
            thumb.Width = thumb.Height = 10;
            thumb.BorderThickness = new Thickness(1);
            thumb.BorderBrush = Brushes.Black;
            thumb.DragDelta += dragDeltaEventHandler;
            thumb.Cursor = cursor;

            m_visualCollection.Add(thumb);

            return thumb;
        }
        protected override Visual GetVisualChild(int index)
        {
            return m_visualCollection[index];
        }
        protected override int VisualChildrenCount
        {
            get { return m_visualCollection.Count; }
        }
        protected override Size ArrangeOverride(Size finalSize)
        {
            double controlWidth = m_adornedElement.ActualWidth;
            double controlHeight = m_adornedElement.ActualHeight;
            m_rightBottomThumb.Arrange(new Rect(controlWidth - 10 / 2, controlHeight - 10 / 2, 10, 10));
            m_dragThumb.Arrange(new Rect(controlWidth / 2 - 10/ 2, controlHeight / 2 - 10 / 2, 10, 10));

            return finalSize;
        }
        void RightBottomThumbDrag(object sender, DragDeltaEventArgs args)
        {

            m_adornedElement.Width = Math.Max(0, m_adornedElement.ActualWidth + args.HorizontalChange);
            m_adornedElement.Height = Math.Max(0, args.VerticalChange + m_adornedElement.ActualHeight);
        }
        void CenteThumbDrag(object sender, DragDeltaEventArgs args)
        {
            m_adornedElement.Width = Math.Max(0, m_adornedElement.ActualWidth + args.HorizontalChange);
            m_adornedElement.Height = Math.Max(0, args.VerticalChange + m_adornedElement.ActualHeight);
        }

        public void RemoveFromAdornerLayer()
        {
            m_myLayer.Remove(this);
        }
    }
}

