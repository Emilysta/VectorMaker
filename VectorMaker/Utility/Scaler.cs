using System.Windows.Media;

namespace VectorMaker.Utility
{
    public class Scaler
    {
        private const float DefaultScale = 1;
        private const float ScaleStep = 0.1f;
        private const float MinimumScale = 0.1f;
        private const float MaximumScale = 4.0f;

        private float m_scale = DefaultScale;
        public float Scale
        {
            get => m_scale;
            set { m_scale = value;
                objectScaleTransform.ScaleX = m_scale;
                objectScaleTransform.ScaleY = m_scale;
            }
        }

        public ScaleTransform objectScaleTransform;
        
        public Scaler(ScaleTransform scaleTransform)
        {
            objectScaleTransform = scaleTransform;
        }

        public void ZoomIn()
        {
            if (Scale + ScaleStep > MaximumScale)
                Scale = MaximumScale;
            else
                Scale += ScaleStep;
        }

        public void ZoomOut()
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
    }
}
