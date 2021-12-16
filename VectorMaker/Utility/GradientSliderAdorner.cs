using System;
using System.Windows;
using System.Windows.Media;

namespace VectorMaker.Utility
{
    /// <summary>
    /// Defines special addorner for <see cref="ControlsResources.GradientColorPicker"/>.
    /// </summary>
    public class GradientSliderAdorner : ThumbSliderAdorner
    {
        private GradientStop m_gradientStop;

        /// <summary>
        /// Holds Gradient Stop in Thumb.
        /// </summary>
        public GradientStop GradientStopObject {
            get => m_gradientStop;
            set
            {
                m_gradientStop = value;
            }
        }
        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="gradientStop"></param>
        /// <param name="adornedElement"></param>
        /// <param name="offset"></param>
        /// <param name="action"></param>
        public GradientSliderAdorner(GradientStop gradientStop,UIElement adornedElement, double offset, Action<ThumbSliderAdorner> action): base(adornedElement,offset,action)
        {
            GradientStopObject = gradientStop;
            OnValueChanged += GradientSliderAdornerOnValueChanged;
        }

        /// <summary>
        /// Method that changes Offset value in GradientStopObject
        /// </summary>
        /// <param name="obj"></param>
        private void GradientSliderAdornerOnValueChanged(double obj)
        {
            GradientStopObject.Offset = obj;
        }
    }
}
