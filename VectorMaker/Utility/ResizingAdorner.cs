using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace VectorMaker.Utility
{
    public class ResizingAdorner : Adorner
    {
        private const int ScaleStep = 5;
        private const double ScaleStepValue = 0.01d;
        private Thumb m_rightBottomThumb;
        private Thumb m_dragThumb;
        private Thumb m_rotateThumb;
        private VisualCollection m_visualCollection;
        private FrameworkElement m_adornedElement;
        private AdornerLayer m_myLayer;
        private TransformGroup m_transformGroup;
        private TranslateTransform m_translateTransform;
        private RotateTransform m_rotateTransform;
        private ScaleTransform m_scaleTransform;
        private Style m_dragStye;

        private double m_actualHeight => Math.Abs(m_adornedElement.ActualHeight * m_scaleTransform.ScaleY) == double.NaN ? 1 : Math.Abs(m_adornedElement.ActualHeight * m_scaleTransform.ScaleY);
        private double m_actualWidth => Math.Abs(m_adornedElement.ActualWidth * m_scaleTransform.ScaleX) == double.NaN ? 1 : Math.Abs(m_adornedElement.ActualHeight * m_scaleTransform.ScaleY);

        public ResizingAdorner(UIElement adornedElement, AdornerLayer myLayer) : base(adornedElement)
        {
            m_adornedElement = (FrameworkElement)adornedElement;
            m_myLayer = myLayer;
            SetClassElements();
            m_dragStye = (Style)Application.Current.Resources["DragThumb"];
        }
        public void RemoveFromAdornerLayer()
        {
            m_myLayer.Remove(this);
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
            m_rightBottomThumb.Arrange(new Rect(m_actualWidth - 5, m_actualWidth - 5, 10, 10));
            m_dragThumb.Arrange(new Rect(0, 0, m_actualWidth, m_actualHeight));
            m_dragThumb.Style = m_dragStye;
            m_rotateThumb.Arrange(new Rect(m_actualWidth / 2 - 10, m_actualHeight / 2 - 10, 20, 20));
            return finalSize;
        }
        private void SetClassElements()
        {
            m_visualCollection = new VisualCollection(this);
            SetTransformGroup();
            CreateDragThumb();
            CreateScaleThumb();
            CreateRotateThumb();
        }
        private void SetTransformGroup()
        {
            m_transformGroup = new TransformGroup();
            if (m_adornedElement.RenderTransform != null)
                m_transformGroup.Children.Add(m_adornedElement.RenderTransform);
            m_translateTransform = new TranslateTransform();
            m_rotateTransform = new RotateTransform();
            m_scaleTransform = new ScaleTransform();
            m_transformGroup.Children.Add(m_translateTransform);
            m_transformGroup.Children.Add(m_rotateTransform);
            m_transformGroup.Children.Add(m_scaleTransform);
            m_adornedElement.RenderTransform = m_transformGroup;
            RenderTransform = m_transformGroup;
        }
        private void CreateScaleThumb()
        {
            m_rightBottomThumb = new Thumb();
            m_rightBottomThumb.Style = (Style)Application.Current.Resources["ScaleThumb"];
            m_rightBottomThumb.Width = m_rightBottomThumb.Height = 10;
            m_rightBottomThumb.DragDelta += ScaleThumbDrag;
            m_rightBottomThumb.Cursor = Cursors.SizeNWSE;
            m_visualCollection.Add(m_rightBottomThumb);
        }
        private void CreateRotateThumb()
        {
            m_rotateThumb = new Thumb();
            m_rotateThumb.Style = (Style)Application.Current.Resources["RotateThumb"];
            m_rotateThumb.Width = m_rightBottomThumb.Height = 20;
            m_rotateThumb.MouseWheel +=  RotateThumbMouseWheel;
            m_rotateThumb.Cursor = Cursors.SizeNWSE;
            m_visualCollection.Add(m_rotateThumb);
        }
        private void CreateDragThumb()
        {
            m_dragThumb = new Thumb();
            m_dragThumb.Style = (Style)Application.Current.Resources["DragThumb"];
            m_dragThumb.Width = m_actualWidth;
            m_dragThumb.Height = m_actualHeight;
            m_dragThumb.ApplyTemplate();
            m_dragThumb.DragDelta += TranslateThumbDrag;
            m_dragThumb.Cursor = Cursors.SizeAll;
            m_visualCollection.Add(m_dragThumb);
        }

        private void ScaleThumbDrag(object sender, DragDeltaEventArgs args)
        {
            double countOfScaleStepsX = (double)args.HorizontalChange / ScaleStep;
            double countOfScaleStepsY = (double)args.VerticalChange / ScaleStep;

            Point point = TranslatePoint(new Point(0, 0), m_adornedElement.Parent as UIElement);
            m_scaleTransform.CenterX = point.X;
            m_scaleTransform.CenterY = point.Y;
            m_scaleTransform.ScaleX += countOfScaleStepsX * ScaleStepValue;
            m_scaleTransform.ScaleY += countOfScaleStepsY * ScaleStepValue;
            //ArrangeOverride(new Size(0, 0));
            Trace.WriteLine($"X: {m_actualWidth} Y: {m_actualHeight}");
            this.InvalidateVisual();
        }
        private void TranslateThumbDrag(object sender, DragDeltaEventArgs args)
        {
            m_translateTransform.X += args.HorizontalChange;
            m_translateTransform.Y += args.VerticalChange;
            this.InvalidateVisual();
        }
        private void RotateThumbMouseWheel(object sender, MouseWheelEventArgs args)
        {
            if (m_rotateThumb.IsDragging)
            {
                double angleInRadians = m_rotateTransform.Angle * Math.PI / 180;
                double sizeX = Math.Cos(angleInRadians) * m_adornedElement.RenderSize.Width + Math.Sin(angleInRadians) * m_adornedElement.RenderSize.Height;
                double sizeY = Math.Cos(angleInRadians) * m_adornedElement.RenderSize.Height + Math.Sin(angleInRadians) * m_adornedElement.RenderSize.Width;
                Point point = TranslatePoint(new Point(sizeX / 2, sizeY / 2), m_adornedElement.Parent as UIElement);
                m_rotateTransform.CenterX = point.X;
                m_rotateTransform.CenterY = point.Y;
                m_rotateTransform.Angle += args.Delta > 0 ? 1 : -1; //todo SetByUser
                if (m_rotateTransform.Angle > 360)
                {
                    m_rotateTransform.Angle -= 360;
                }
                else if (m_rotateTransform.Angle < -360)
                {
                    m_rotateTransform.Angle += 360;
                }
                this.InvalidateVisual();
            }
        }
    }
}

