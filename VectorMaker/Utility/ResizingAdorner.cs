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
        private Thumb m_scaleThumb;
        private Thumb m_dragThumb;
        private Thumb m_rotateThumb;
        private VisualCollection m_visualCollection;
        private FrameworkElement m_adornedElement;
        private AdornerLayer m_myLayer;
        private TransformGroup m_transformGroup;
        private TranslateTransform m_translateTransform;
        private RotateTransform m_rotateTransform;
        private ScaleTransform m_scaleTransform;
        private ScaleTransform m_startScale;
        private Point m_startPoint;

        private double m_actualHeight => Double.IsNormal(Math.Abs(m_adornedElement.ActualHeight * m_scaleTransform.ScaleY)) ? Math.Abs(m_adornedElement.ActualHeight * m_scaleTransform.ScaleY) : 1;
        private double m_actualWidth => Double.IsNormal(Math.Abs(m_adornedElement.ActualWidth * m_scaleTransform.ScaleX)) ? Math.Abs(m_adornedElement.ActualHeight * m_scaleTransform.ScaleX) : 1;

        public ResizingAdorner(UIElement adornedElement, AdornerLayer myLayer) : base(adornedElement)
        {
            m_adornedElement = (FrameworkElement)adornedElement;
            m_myLayer = myLayer;
            SetClassElements();
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
            m_scaleThumb.Arrange(new Rect(finalSize.Width - 5, finalSize.Height - 5, 10, 10));
            m_dragThumb.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            m_rotateThumb.Arrange(new Rect(finalSize.Width / 2 - 10, finalSize.Height / 2 - 10, 20, 20));
            return finalSize;
        }
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            ScaleTransform scaleTransform = new ScaleTransform(1 / m_scaleTransform.ScaleX, 1 / m_scaleTransform.ScaleY);
            m_rotateThumb.RenderTransform = scaleTransform;
            //m_rotateThumb.RenderTransformOrigin = new Point(0.5, 0.5);
            m_scaleThumb.RenderTransform = scaleTransform;
            //m_scaleThumb.RenderTransformOrigin = new Point(0.5, 0.5);

            return base.GetDesiredTransform(transform);
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
            
        }
        private void CreateScaleThumb()
        {
            m_scaleThumb = new Thumb();
            m_scaleThumb.Style = (Style)Application.Current.Resources["ScaleThumb"];
            m_scaleThumb.Width = m_scaleThumb.Height = 10;
            m_scaleThumb.DragDelta += ScaleThumbDrag;
            m_scaleThumb.DragStarted += (a, b) =>
            {
                m_startScale = new ScaleTransform(m_scaleTransform.ScaleX, m_scaleTransform.ScaleY);
                m_startPoint = TranslatePoint(new Point(0, 0), m_adornedElement.Parent as UIElement);
            };
            m_scaleThumb.Cursor = Cursors.SizeNWSE;
            m_visualCollection.Add(m_scaleThumb);
        }
        private void CreateRotateThumb()
        {
            m_rotateThumb = new Thumb();
            m_rotateThumb.Style = (Style)Application.Current.Resources["RotateThumb"];
            m_rotateThumb.Width = m_scaleThumb.Height = 20;
            m_rotateThumb.MouseWheel += RotateThumbMouseWheel;
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
            if (Double.IsNormal(args.HorizontalChange) && Double.IsNormal(args.VerticalChange))
            {
                //Trace.WriteLine($"{args.HorizontalChange} {args.VerticalChange}");
                m_scaleTransform.CenterX = m_startPoint.X;
                m_scaleTransform.CenterY = m_startPoint.Y;
                m_scaleTransform.ScaleX = m_startScale.ScaleX + (args.HorizontalChange * ScaleStepValue);
                m_scaleTransform.ScaleY = m_startScale.ScaleY + (args.VerticalChange * ScaleStepValue);
                //Trace.WriteLine($"ScaleX: {m_scaleTransform.ScaleX} ScaleY: {m_scaleTransform.ScaleX}");
            }
            else
                Trace.WriteLine($"Not a normal");

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
                Point point = TranslatePoint(new Point(m_actualWidth / 2, m_actualHeight / 2), m_adornedElement.Parent as UIElement);
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
            }
            this.InvalidateVisual();
        }
    }
}

