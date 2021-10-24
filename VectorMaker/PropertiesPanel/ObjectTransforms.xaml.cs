using System;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
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
        public ObjectTransforms()
        {
            InitializeComponent();
        }

        private void ApplyTranslationButton_Click(object sender, RoutedEventArgs e)
        {
            if(IsOneObjectSelected)
            {
                Transform transform = (frame.Content as DrawingCanvas).SelectedObjects[0].AdornedElement.RenderTransform;
                TranslateTransform translateTransform = new TranslateTransform();
                translateTransform.X = (double)TranslationXTextBox.Value;
                translateTransform.Y = (double)TranslationYTextBox.Value;
                TransformGroup transformGroup = transform as TransformGroup;
                if (transformGroup == null)
                {
                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(transform);
                    transformGroup.Children.Add(translateTransform);
                    (frame.Content as DrawingCanvas).SelectedObjects[0].AdornedElement.RenderTransform = transformGroup;
                }
                else
                {
                    transformGroup.Children.Add(translateTransform);
                }
                
                (frame.Content as DrawingCanvas).SelectedObjects[0].AdornedElement.InvalidateVisual();
            }
        }

        private void ApplyRotationButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsOneObjectSelected)
            {
                Transform transform = (frame.Content as DrawingCanvas).SelectedObjects[0].AdornedElement.RenderTransform;
                RotateTransform rotateTransform = new RotateTransform();
                //rotateTransform.CenterX = (double)RotateCenterXBox.Value;
                //rotateTransform.CenterY = (double)RotateCenterYBox.Value;
                Point toTranslate = new Point((frame.Content as DrawingCanvas).SelectedObjects[0].AdornedElement.RenderSize.Width / 2,
                    (frame.Content as DrawingCanvas).SelectedObjects[0].AdornedElement.RenderSize.Height / 2);
                Point center = (frame.Content as DrawingCanvas).SelectedObjects[0].AdornedElement.TranslatePoint(toTranslate, (frame.Content as DrawingCanvas).MainCanvas);
                rotateTransform.Angle = (double)RotateAngleBox.Value;
                rotateTransform.CenterX = center.X;
                rotateTransform.CenterY = center.Y;

                TransformGroup transformGroup = transform as TransformGroup;
                if (transformGroup == null)
                {
                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(transform);
                    transformGroup.Children.Add(rotateTransform);
                    (frame.Content as DrawingCanvas).SelectedObjects[0].AdornedElement.RenderTransform = transformGroup;
                }
                else
                {
                    transformGroup.Children.Add(rotateTransform);
                }

                (frame.Content as DrawingCanvas).SelectedObjects[0].AdornedElement.InvalidateVisual();
            }
        }

        private void ApplyScaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsOneObjectSelected)
            {

            }
        }

        private void ApplySkewButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsOneObjectSelected)
            {

            }
        }

        private bool CheckSelection()
        {
            frame = MainWindow.Instance.DockingManager.ActiveContent as Frame;
            // MainWindow.Instance.DockingManager.Con
            if ((frame.Content as DrawingCanvas).SelectedObjects.Count > 1)
            {
                BlockingBlend.Visibility = Visibility.Visible;
                IsOneObjectSelected = false;
                return false;
            }
            IsOneObjectSelected = true;
            BlockingBlend.Visibility = Visibility.Hidden;
            return true;
        }

        public void SelectedObjectsChanged(object sender,NotifyCollectionChangedEventArgs e )
        {
            CheckSelection();
        }
    }
}
