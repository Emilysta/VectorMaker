using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using VectorMaker.Pages;

namespace VectorMaker.PropertiesPanel
{
    /// <summary>
    /// Interaction logic for ObjectTransforms.xaml
    /// </summary>
    public partial class ObjectTransforms : UserControl
    {
        private bool IsOneObjectSelected = false;
        public ObjectTransforms()
        {
            InitializeComponent();
        }

        private void ApplyTranslationButton_Click(object sender, RoutedEventArgs e)
        {
            if(IsOneObjectSelected)
            {
               
            }
        }

        private void ApplyRotationButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsOneObjectSelected)
            {

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
            Frame frame = MainWindow.Instance.DockingManager.ActiveContent as Frame;
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
