using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using VectorMaker.Pages;

namespace VectorMaker.PropertiesPanel
{
    /// <summary>
    /// Interaction logic for ObjectTransforms.xaml
    /// </summary>
    public partial class ObjectTransforms : UserControl
    {
        private Frame frame;
        private bool IsOneObjectSelected = false;
        private Adorner m_adorner;
        public ObjectTransforms()
        {
            InitializeComponent();
        }

        private void ApplyTranslationButton_Click(object sender, RoutedEventArgs e)
        {
            //if (IsOneObjectSelected && m_adorner != null)
            //{
            //    Transform transform = m_adorner.AdornedElement.RenderTransform;
            //    TranslateTransform translateTransform = new TranslateTransform();
            //    translateTransform.X = (double)TranslationXTextBox.Value;
            //    translateTransform.Y = (double)TranslationYTextBox.Value;
            //    TransformGroup transformGroup = transform as TransformGroup;
            //    if (transformGroup == null)
            //    {
            //        transformGroup = new TransformGroup();
            //        transformGroup.Children.Add(transform);
            //        transformGroup.Children.Add(translateTransform);
            //        m_adorner.AdornedElement.RenderTransform = transformGroup;
            //    }
            //    else
            //    {
            //        transformGroup.Children.Add(translateTransform);
            //    }

            //    m_adorner.AdornedElement.InvalidateVisual();
            //}
        }

        private void ApplyRotationButton_Click(object sender, RoutedEventArgs e)
        {
            //if (IsOneObjectSelected && m_adorner != null)
            //{
            //    Transform transform = m_adorner.AdornedElement.RenderTransform;
            //    RotateTransform rotateTransform = new RotateTransform();

            //    Point toTranslate = new Point((double)RotateCenterXBox.Value * m_adorner.ActualWidth
            //        , (double)RotateCenterYBox.Value * m_adorner.ActualHeight);
            //    Point center = m_adorner.TranslatePoint(toTranslate, (frame.Content as DrawingCanvas).MainCanvas);
            //    rotateTransform.Angle = (double)RotateAngleBox.Value;
            //    rotateTransform.CenterX = center.X;
            //    rotateTransform.CenterY = center.Y;

            //    TransformGroup transformGroup = transform as TransformGroup;
            //    if (transformGroup == null)
            //    {
            //        transformGroup = new TransformGroup();
            //        transformGroup.Children.Add(transform);
            //        transformGroup.Children.Add(rotateTransform);
            //        m_adorner.AdornedElement.RenderTransform = transformGroup;
            //    }
            //    else
            //    {
            //        transformGroup.Children.Add(rotateTransform);
            //    }
            //    m_adorner.AdornedElement.InvalidateVisual();
            //}
        }

        private void ApplyScaleButton_Click(object sender, RoutedEventArgs e)
        {
            //if (IsOneObjectSelected && m_adorner != null)
            //{
            //    Transform transform = m_adorner.AdornedElement.RenderTransform;
            //    ScaleTransform scaleTransform = new ScaleTransform();

            //    Point toTranslate = new Point((double)ScaleCenterXBox.Value * m_adorner.ActualWidth,
            //        (double)ScaleCenterYBox.Value * m_adorner.ActualHeight);

            //    Point center = m_adorner.TranslatePoint(toTranslate, (frame.Content as DrawingCanvas).MainCanvas);
            //    scaleTransform.ScaleX = (double)ScaleXBox.Value;
            //    scaleTransform.ScaleY = (double)ScaleYBox.Value;
            //    scaleTransform.CenterX = center.X;
            //    scaleTransform.CenterY = center.Y;

            //    TransformGroup transformGroup = transform as TransformGroup;
            //    if (transformGroup == null)
            //    {
            //        transformGroup = new TransformGroup();
            //        transformGroup.Children.Add(transform);
            //        transformGroup.Children.Add(scaleTransform);
            //        m_adorner.AdornedElement.RenderTransform = transformGroup;
            //    }
            //    else
            //    {
            //        transformGroup.Children.Add(scaleTransform);
            //    }
            //    m_adorner.AdornedElement.InvalidateVisual();
            //}
        }

        private void ApplySkewButton_Click(object sender, RoutedEventArgs e)
        {
        //    if (IsOneObjectSelected && m_adorner != null)
        //    {
        //        Transform transform = m_adorner.AdornedElement.RenderTransform;
        //        SkewTransform skewTransform = new SkewTransform();

        //        Point toTranslate = new Point((double)ScaleCenterXBox.Value * m_adorner.ActualWidth,
        //            (double)ScaleCenterYBox.Value * m_adorner.ActualHeight);

        //        Point center = m_adorner.TranslatePoint(toTranslate, (frame.Content as DrawingCanvas).MainCanvas);
        //        skewTransform.AngleX = (double)SkewAngleXBox.Value;
        //        skewTransform.AngleY = (double)SkewAngleYBox.Value;
        //        skewTransform.CenterX = center.X;
        //        skewTransform.CenterY = center.Y;

        //        TransformGroup transformGroup = transform as TransformGroup;
        //        if (transformGroup == null)
        //        {
        //            transformGroup = new TransformGroup();
        //            transformGroup.Children.Add(transform);
        //            transformGroup.Children.Add(skewTransform);
        //            m_adorner.AdornedElement.RenderTransform = transformGroup;
        //        }
        //        else
        //        {
        //            transformGroup.Children.Add(skewTransform);
        //        }
        //        m_adorner.AdornedElement.InvalidateVisual();
        //    }
        }

        private void CheckSelection()
        {
        //    frame = MainWindow.Instance.DockingManager.ActiveContent as Frame;
        //    // MainWindow.Instance.DockingManager.Con
        //    if ((frame.Content as DrawingCanvas).SelectedObjects.Count > 1)
        //    {
        //        BlockingBlend.Visibility = Visibility.Visible;
        //        IsOneObjectSelected = false;
        //    }
        //    else if ((frame.Content as DrawingCanvas).SelectedObjects.Count == 1)
        //    {
        //        m_adorner = (frame.Content as DrawingCanvas).SelectedObjects[0];
        //        IsOneObjectSelected = true;
        //        BlockingBlend.Visibility = Visibility.Hidden;
        //    }
        //    else
        //    {
        //        BlockingBlend.Visibility = Visibility.Visible;
        //        IsOneObjectSelected = false;
        //    }
        }

        public void SelectedObjectsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
           // CheckSelection();
        }
    }
}
