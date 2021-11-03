using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VectorMaker.Commands;

namespace VectorMaker.Utility
{
    public class ViewportController : Control
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

        public ICommand ScrollChangedCommand { get; set; }
        public ICommand PreviewMouseWheelCommand { get; set; }

        private static DependencyProperty m_scrollViewerProperty =
DependencyProperty.Register("ScrollViewerProperty", typeof(ScrollViewer), typeof(ViewportController), new PropertyMetadata(null));

        private static DependencyProperty m_objectToControlProperty =
DependencyProperty.Register("ObjectToControlProperty", typeof(UIElement), typeof(ViewportController), new PropertyMetadata(null));

        public ScrollViewer ScrollViewerProperty
        {
            get { return (ScrollViewer)GetValue(m_scrollViewerProperty); }
            set { SetValue(m_scrollViewerProperty, value); }
        }

        public UIElement ObjectToControlProperty
        {
            get { return (UIElement)GetValue(m_objectToControlProperty); }
            set { SetValue(m_objectToControlProperty, value); }
        }

        static ViewportController()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ViewportController), new FrameworkPropertyMetadata(typeof(ViewportController)));
        }

        private void SetCommands()
        {
            ScrollChangedCommand = new CommandBase((obj) => ScrollViewerChangedHandler(obj as ScrollChangedEventArgs));
            PreviewMouseWheelCommand = new CommandBase((obj) => PreviewMouseWheelHandler(obj as MouseWheelEventArgs));
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


        public void PreviewMouseWheelHandler(MouseWheelEventArgs e)
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

        public void ScrollViewerChangedHandler(ScrollChangedEventArgs e)
        {
            
        }

        protected override void OnInitialized(EventArgs e) //override On Initialized Event Rised after initialization of Parent
        {
            base.OnInitialized(e);
            ObjectsTransformGroup.Children.Add(ObjectsScaleTransform);
            ObjectsTransformGroup.Children.Add(ObjectsRotateTransform);
            ObjectsTransformGroup.Children.Add(ObjectsTranslateTransform);

            ObjectToControlProperty.RenderTransform = ObjectsTransformGroup;

            ObjectsRotateTransform.CenterX = ScrollViewerProperty.ActualWidth / 2;
            ObjectsRotateTransform.CenterY = ScrollViewerProperty.ActualHeight / 2;
        }
    }
}
