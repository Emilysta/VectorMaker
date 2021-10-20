using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VectorMaker.Utility
{
    public class ViewportController
    {
        private const float DefaultScale = 1;
        private const float ScaleStep = 0.1f;
        private const float MinimumScale = 0.5f;
        private const float MaximumScale = 4.0f;
        private const float RotateStep = 5f;

        private float m_scale = DefaultScale;
        public float Scale
        {
            get => m_scale;
            set
            {
                m_scale = value;
                ObjectsScaleTransform.ScaleX = m_scale;
                ObjectsScaleTransform.ScaleY = m_scale;
            }
        }

        public ScaleTransform ObjectsScaleTransform = new ScaleTransform();
        public TranslateTransform ObjectsTranslateTransform = new TranslateTransform();
        public RotateTransform ObjectsRotateTransform = new RotateTransform();
        public TransformGroup ObjectsTransformGroup = new TransformGroup();
        public TransformGroup ObjectsTransformGroup2 = new TransformGroup();
        public FrameworkElement ObjectToControl;
        public ScrollViewer viewer;


        public ViewportController(FrameworkElement objectToControl, ScrollViewer scrollViewer)
        {
            ObjectToControl = objectToControl;
            ObjectsTransformGroup.Children.Add(ObjectsScaleTransform);
            ObjectsTransformGroup2.Children.Add(ObjectsRotateTransform);
            ObjectsTransformGroup.Children.Add(ObjectsTranslateTransform);

            objectToControl.LayoutTransform = ObjectsTransformGroup;

            objectToControl.RenderTransform = ObjectsTransformGroup2;

            ObjectsRotateTransform.CenterX = (ObjectToControl.Parent as FrameworkElement).ActualWidth / 2;
            ObjectsRotateTransform.CenterY = (ObjectToControl.Parent as FrameworkElement).ActualHeight / 2;
            viewer = scrollViewer;
        }

        private void ZoomIn()
        {
            if (Scale + ScaleStep > MaximumScale)
                Scale = MaximumScale;
            else
                Scale += ScaleStep;
        }

        private void ZoomOut()
        {
            if (Scale - ScaleStep < MinimumScale)
                Scale = MinimumScale;
            else
                Scale -= ScaleStep;
        }

        public void ResetZoom()
        {
            if (Scale != DefaultScale)
                Scale = DefaultScale;
        }

        private void Rotate(bool IsRotatePositive)
        {
            if (IsRotatePositive)
                ObjectsRotateTransform.Angle += RotateStep;
            else
                ObjectsRotateTransform.Angle -= RotateStep;


        }

        private void ResetRotate()
        {
            ObjectsRotateTransform.Angle = 0;
        }


        public void MouseWheel(MouseWheelEventArgs e)
        {
            
            switch (Keyboard.Modifiers)
            {
                case ModifierKeys.Control:
                    {
                        if (e.Delta > 0)
                            ZoomIn();
                        else if (e.Delta < 0)
                            ZoomOut();
                        break;
                    }
                case ModifierKeys.Alt:
                    {
                        if (e.Delta > 0)
                            Rotate(true);
                        else if (e.Delta < 0)
                            Rotate(false);
                        break;
                    }
            }
        }

        public void ScrollViewerChanged(ScrollChangedEventArgs e)
        {
            
        }

    }
}
