using System;
using System.Windows;
using System.Windows.Media;

namespace VectorMaker.Utility
{
    public class GradientSliderAdorner : ThumbSliderAdorner
    {
        private GradientStop m_gradientStop;

        public GradientStop GradientStopObject {
            get => m_gradientStop;
            set
            {
                m_gradientStop = value;
            }
        }

        public GradientSliderAdorner(GradientStop gradientStop,UIElement adornedElement, double offset, Action<ThumbSliderAdorner> action): base(adornedElement,offset,action)
        {
            GradientStopObject = gradientStop;
            OnValueChanged += GradientSliderAdornerOnValueChanged;
        }

        private void GradientSliderAdornerOnValueChanged(double obj)
        {
            GradientStopObject.Offset = obj;
        }
    }
}
