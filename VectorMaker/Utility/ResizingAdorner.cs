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
        private const double ScaleStepValue = 0.007d;
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
        private Transform m_transform;
        private Point m_startPoint;
        private Style m_scaleThumbStyleDuringDrag;
        private Style m_scaleThumbStyle;
        private Canvas m_canvas;
        private MouseDragElementBehavior mouseDrag = new();

        public ResizingAdorner(UIElement adornedElement, AdornerLayer myLayer, Canvas canvas) : base(adornedElement)
        {
            m_adornedElement = (FrameworkElement)adornedElement;
            m_myLayer = myLayer;
            SetClassElements();
            m_canvas = canvas;
            m_transform = m_adornedElement.RenderTransform;
            mouseDrag.Attach(AdornedElement);
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
            //if (m_scaleTransform != null)
            //{
            //    ScaleTransform scaleTransform = new ScaleTransform(1 / m_scaleTransform.ScaleX, 1 / m_scaleTransform.ScaleY);
            //    m_rotateThumb.RenderTransform = scaleTransform;
            //    m_rotateThumb.RenderTransformOrigin = new Point(0.5, 0.5);
            //    m_scaleThumb.RenderTransform = scaleTransform;
            //    m_scaleThumb.RenderTransformOrigin = new Point(0.5, 0.5);
            //}
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
            m_dragThumb.ApplyTemplate();
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
        private void SetStartCompletedDragEvent()
        {
            m_dragThumb.MouseDown += (a, b) =>
            {
                //mouseDrag.Attach(AdornedElement);
                m_dragThumb.CancelDrag();
            };

            m_dragThumb.MouseUp += (a, b) =>
            {
                //mouseDrag.Detach();
            };
            m_scaleThumb.DragStarted += (a, b) =>
            {
                m_scaleThumb.Style = m_scaleThumbStyleDuringDrag;
                m_dragThumb.Visibility = Visibility.Hidden;
                m_rotateThumb.Visibility = Visibility.Hidden;
                //m_scaleTransform = new ScaleTransform();
                m_startPoint = m_adornedElement.TranslatePoint(new Point(0, 0), m_canvas);
                //m_startScale = new ScaleTransform(m_scaleTransform.ScaleX, m_scaleTransform.ScaleY);
                //m_scaleTransform.CenterX = m_startPoint.X;
                //m_scaleTransform.CenterY = m_startPoint.Y;
                //m_transformGroup.Children.Add(m_scaleTransform);
            };
            m_scaleThumb.DragCompleted += (a, b) =>
            {
                
                m_scaleThumb.Style = m_scaleThumbStyle;
                m_dragThumb.Visibility = Visibility.Visible;
                m_rotateThumb.Visibility = Visibility.Visible;
                //Size size = new Size(AdornedElement.RenderSize.Width * m_scaleTransform.ScaleX, AdornedElement.RenderSize.Height * m_scaleTransform.ScaleY);
                //m_adornedElement.Width = size.Width;
                //m_adornedElement.Height = size.Height;
                
                //m_scaleTransform.ScaleX = 1;
                //m_scaleTransform.ScaleY = 1;
                this.InvalidateVisual();
            };

            //m_dragThumb.DragStarted += (a, b) =>
            //{
            //    m_scaleThumb.Visibility = Visibility.Hidden;
            //    m_rotateThumb.Visibility = Visibility.Hidden;
            //    //Interaction.GetBehaviors(AdornedElement).Add(mouseDrag);
            //    mouseDrag.Attach(AdornedElement);
            //};
            //m_dragThumb.DragCompleted += (a, b) =>
            //{
            //    m_scaleThumb.Visibility = Visibility.Visible;
            //    m_rotateThumb.Visibility = Visibility.Visible;
            //    //Interaction.GetBehaviors(AdornedElement).Remove(mouseDrag);
            //    mouseDrag.Detach();
            //};

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
                Trace.WriteLine($"{test} {scaleX} {scaleY}");
                matrix.ScaleAt(scaleX, scaleY, m_startPoint.X, m_startPoint.Y);
                m_adornedElement.RenderTransform = new MatrixTransform(matrix);
                m_transform = m_adornedElement.RenderTransform;
            }
            AdornedElement.InvalidateVisual();
        }
        private void TranslateThumbDrag(object sender, DragDeltaEventArgs args)
        {
            //m_translateTransform.X += args.HorizontalChange;
            //m_translateTransform.Y += args.VerticalChange;
            //Matrix matrix = m_transform.Value;
            //matrix.Translate(args.HorizontalChange, args.VerticalChange);
            //m_adornedElement.RenderTransform = new MatrixTransform(matrix);
            //m_transform = m_adornedElement.RenderTransform;
           // AdornedElement.InvalidateVisual();
            //Interaction.GetBehaviors(AdornedElement).Add(new MouseDragElementBehavior());

            Trace.WriteLine(m_transform.Value.ToString());
            //this.InvalidateVisual();
        }
        private void RotateThumbMouseWheel(object sender, MouseWheelEventArgs args)
        {
            if (m_rotateThumb.IsDragging)
            {
                //m_rotateTransform.CenterX = m_startPoint.X;
                //m_rotateTransform.CenterY = m_startPoint.Y;
                //m_rotateTransform.Angle += args.Delta > 0 ? 1 : -1; //todo SetByUser
                //if (m_rotateTransform.Angle > 360)
                //{
                //    m_rotateTransform.Angle -= 360;
                //}
                //else if (m_rotateTransform.Angle < -360)
                //{
                //    m_rotateTransform.Angle += 360;
                //}
                Matrix matrix = m_transform.Value;
                matrix.RotateAt(args.Delta > 0 ? 1 : -1,m_startPoint.X,m_startPoint.Y);
                m_adornedElement.RenderTransform = new MatrixTransform(matrix);
                m_transform = m_adornedElement.RenderTransform;
                AdornedElement.InvalidateVisual();
            }
            this.InvalidateVisual();
        }
    }
}

