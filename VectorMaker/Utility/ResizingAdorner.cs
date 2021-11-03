using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using Microsoft.Xaml.Behaviors.Layout;

namespace VectorMaker.Utility
{
    public class ResizingAdorner : Adorner
    {
        private VisualCollection m_visualCollection;
        private Thumb m_scaleThumb;
        private Thumb m_dragThumb;
        private Thumb m_rotateThumb;

        private FrameworkElement m_adornedElement;
        private AdornerLayer m_myLayer;
        private Canvas m_canvas;

        private Transform m_transform;
        private Point m_startPoint;
        private Style m_scaleThumbStyleDuringDrag;
        private Style m_scaleThumbStyle;

        public ResizingAdorner(UIElement adornedElement, AdornerLayer myLayer, Canvas canvas) : base(adornedElement)
        {
            m_adornedElement = (FrameworkElement)adornedElement;
            m_myLayer = myLayer;
            SetClassElements();
            m_canvas = canvas;
            m_transform = m_adornedElement.RenderTransform;
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
            m_scaleThumb?.Arrange(new Rect(finalSize.Width - 5, finalSize.Height - 5, 10, 10));
            m_dragThumb?.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
            m_rotateThumb?.Arrange(new Rect(finalSize.Width / 2 - 10, finalSize.Height / 2 - 10, 20, 20));
            return finalSize;
        }
        public override GeneralTransform GetDesiredTransform(GeneralTransform transform)
        {
            if (m_transform != null)
            {
                //Matrix matrix = m_transform.Value;
                //ScaleTransform scaleTransform = new ScaleTransform(1 / matrix.M11, 1 / matrix.M22);
                //if (m_rotateThumb != null)
                //{
                //    m_rotateThumb.RenderTransform = scaleTransform;
                //    m_rotateThumb.RenderTransformOrigin = new Point(0.5, 0.5);
                //}
                //if (m_scaleThumb != null)
                //{
                //    m_scaleThumb.RenderTransform = scaleTransform;
                //    m_scaleThumb.RenderTransformOrigin = new Point(0.5, 0.5);
                //}
                //if(m_dragThumb != null)
                //{
                //    m_dragThumb.RenderTransform = scaleTransform;
                //    m_dragThumb.RenderTransformOrigin = new Point(0.5, 0.5);
                //}
            }
            return base.GetDesiredTransform(transform);
        }
        private void SetClassElements()
        {
            m_visualCollection = new VisualCollection(this);
            SetTransformGroup();
            CreateDragThumb();
            CreateScaleThumb();
            CreateRotateThumb();
            SetStartCompletedDragEvent();
        }
        private void SetTransformGroup()
        {
            //m_transformGroup = new TransformGroup();
            //if (m_adornedElement.RenderTransform != null)
            //{
            //    m_transformGroup.Children.Add(m_adornedElement.RenderTransform);
            //}
            //m_translateTransform = new TranslateTransform();
            //m_transformGroup.Children.Add(m_translateTransform);
            //m_adornedElement.RenderTransform = m_transformGroup;

        }
        private void CreateScaleThumb()
        {
            m_scaleThumb = new Thumb();
            m_scaleThumbStyle = (Style)Application.Current.Resources["ScaleThumb"];
            m_scaleThumbStyleDuringDrag = (Style)Application.Current.Resources["ScaleThumbDuringDrag"];
            m_scaleThumb.Width = m_scaleThumb.Height = 10;
            m_scaleThumb.Style = m_scaleThumbStyle;
            m_scaleThumb.DragDelta += ScaleThumbDrag;
            m_scaleThumb.Cursor = Cursors.SizeNWSE;
            m_visualCollection.Add(m_scaleThumb);
        }
        private void CreateRotateThumb()
        {
            m_rotateThumb = new Thumb();
            m_rotateThumb.Style = (Style)Application.Current.Resources["RotateThumb"];
            m_rotateThumb.Width = m_scaleThumb.Height = 20;
            m_rotateThumb.ApplyTemplate();
            m_rotateThumb.MouseWheel += RotateThumbMouseWheel;
            m_rotateThumb.Cursor = Cursors.SizeNWSE;
            m_visualCollection.Add(m_rotateThumb);
        }
        private void CreateDragThumb()
        {
            m_dragThumb = new Thumb();
            m_dragThumb.Style = (Style)Application.Current.Resources["DragThumb"];
            m_dragThumb.Width = m_adornedElement.Width;
            m_dragThumb.Height = m_adornedElement.Height;
            m_dragThumb.ApplyTemplate();
            m_dragThumb.DragDelta += TranslateThumbDrag;
            m_dragThumb.Cursor = Cursors.SizeAll;
            m_visualCollection.Add(m_dragThumb);
        }

        private void RecreateDrugThumb()
        {
            m_visualCollection.Remove(m_dragThumb);
            m_dragThumb.DragDelta -= TranslateThumbDrag;
            CreateDragThumb();
        }

        private void SetStartCompletedDragEvent()
        {
            m_scaleThumb.DragStarted += (a, b) =>
            {
                m_scaleThumb.Style = m_scaleThumbStyleDuringDrag;
                m_dragThumb.Visibility = Visibility.Hidden;
                m_rotateThumb.Visibility = Visibility.Hidden;
                m_startPoint = m_adornedElement.TranslatePoint(new Point(0, 0), m_canvas);
            };
            m_scaleThumb.DragCompleted += (a, b) =>
            {
                m_scaleThumb.Style = m_scaleThumbStyle;
                m_dragThumb.Visibility = Visibility.Visible;
                m_rotateThumb.Visibility = Visibility.Visible;
                RecreateDrugThumb();
                this.InvalidateVisual();
            };

            m_dragThumb.DragStarted += (a, b) =>
            {
                m_scaleThumb.Visibility = Visibility.Hidden;
                m_rotateThumb.Visibility = Visibility.Hidden;
            };
            m_dragThumb.DragCompleted += (a, b) =>
            {
                m_scaleThumb.Visibility = Visibility.Visible;
                m_rotateThumb.Visibility = Visibility.Visible;
            };

            m_rotateThumb.DragStarted += (a, b) =>
            {
                Point point = new Point(m_adornedElement.Width / 2, m_adornedElement.Height / 2);
                m_startPoint = m_adornedElement.TranslatePoint(point, m_canvas);
                //m_rotateTransform = new RotateTransform();
                //m_rotateTransform.CenterX = m_startPoint.X;
                //m_rotateTransform.CenterY = m_startPoint.Y;
                //m_transformGroup.Children.Add(m_rotateTransform);
                m_scaleThumb.Visibility = Visibility.Hidden;
                m_dragThumb.Visibility = Visibility.Hidden;
            };
            m_rotateThumb.DragCompleted += (a, b) =>
            {
                m_scaleThumb.Visibility = Visibility.Visible;
                m_dragThumb.Visibility = Visibility.Visible;
            };
        }
        private void ScaleThumbDrag(object sender, DragDeltaEventArgs args)
        {
            Matrix matrix = m_transform.Value;
            Point CurrentPoint = Mouse.GetPosition(m_canvas);
            Point test = new Point(CurrentPoint.X - m_startPoint.X,CurrentPoint.Y - m_startPoint.Y);
            double scaleX = test.X / (AdornedElement.RenderSize.Width *matrix.M11);
            double scaleY = test.Y / (AdornedElement.RenderSize.Height * matrix.M22);
            if (double.IsNormal(scaleX) && double.IsNormal(scaleY))
            {
                matrix.ScaleAtPrepend(scaleX, scaleY, 0, 0);
                m_adornedElement.RenderTransform = new MatrixTransform(matrix);
                m_transform = m_adornedElement.RenderTransform;
            }
        }
        private void TranslateThumbDrag(object sender, DragDeltaEventArgs args)
        {
            Matrix matrix = m_transform.Value;
            matrix.Translate(args.HorizontalChange, args.VerticalChange);
            m_adornedElement.RenderTransform = new MatrixTransform(matrix);
            m_transform = m_adornedElement.RenderTransform;
        }
        private void RotateThumbMouseWheel(object sender, MouseWheelEventArgs args)
        {
            if (m_rotateThumb.IsDragging)
            {
                Matrix matrix = m_transform.Value;
                matrix.RotateAtPrepend(args.Delta > 0 ? 1 : -1,m_adornedElement.Width/2, m_adornedElement.Height / 2);
                m_adornedElement.RenderTransform = new MatrixTransform(matrix);
                m_transform = m_adornedElement.RenderTransform;
                AdornedElement.InvalidateVisual();
            }
        }
    }
}

