using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using VectorMaker.Commands;
using VectorMaker.Intefaces;
using VectorMaker.Utility;
using VectorMaker.Views;

namespace VectorMaker.ViewModel
{
    internal class ObjectTransformsViewModel : ToolBaseViewModel, INotifyPropertyChanged
    {
        private Visibility m_blendVisibiity;
        private DrawingCanvasViewModel m_activeContent;
        private ObservableCollection<ResizingAdorner> m_selectedObjects => m_activeContent?.SelectedObjects;
        private Adorner m_adorner => m_selectedObjects[0];
        private Transform m_transform => m_adorner.AdornedElement.RenderTransform;
        private bool IsOneObjectSelected => m_selectedObjects.Count == 1;
        private readonly IMainWindowViewModel m_interfaceMainWindowVM;

        public ICommand ApplyTranslationCommand { get; set; }
        public ICommand ApplyRotationCommand { get; set; }
        public ICommand ApplyScaleCommand { get; set; }
        public ICommand ApplySkewCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public Visibility BlendVisibility
        {
            get => m_blendVisibiity;
            set
            {
                m_blendVisibiity = value;
                OnPropertyChanged("BlendVisibility");
            }
        }

        public ObjectTransformsViewModel(IMainWindowViewModel interfaceMainWindowVM) : base()
        {
            m_interfaceMainWindowVM = interfaceMainWindowVM;
            m_interfaceMainWindowVM.ActiveCanvasChanged += OnActiveCanvasChanged;
            ApplyTranslationCommand = new CommandBase((obj) => ApplyTranslation(obj));
            ApplyRotationCommand = new CommandBase((obj) => ApplyRotation(obj));
            ApplyScaleCommand = new CommandBase((obj) => ApplyScale(obj));
            ApplySkewCommand = new CommandBase((obj) => ApplySkew(obj)); 
        }

        private void OnActiveCanvasChanged(object sender, System.EventArgs e)
        {
            //throw new System.NotImplementedException();
        }

        protected ObjectTransformsViewModel(): base()
        {

        }

        private void SelectedObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (m_selectedObjects.Count > 1)
            {
                BlendVisibility = Visibility.Visible;
            }
            else if (IsOneObjectSelected)
            {
                BlendVisibility = Visibility.Hidden;
            }
            else
            {
                BlendVisibility = Visibility.Visible;
            }
        }

        private void ApplyTranslation(object values)
        {
            var valArray = (object[])values;
            if (IsOneObjectSelected && m_adorner != null)
            {
                TranslateTransform translateTransform = new TranslateTransform();
                translateTransform.X = (double)valArray[0];
                translateTransform.Y = (double)valArray[1];
                TransformGroup transformGroup = m_transform as TransformGroup;
                if (transformGroup == null)
                {
                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(m_transform);
                    transformGroup.Children.Add(translateTransform);
                    m_adorner.AdornedElement.RenderTransform = transformGroup;
                }
                else
                {
                    transformGroup.Children.Add(translateTransform);
                }

                m_adorner.AdornedElement.InvalidateVisual();
            }
        }

        private void ApplyRotation(object values)
        {
            var valArray = (object[])values;//RotateCenterX,RotateCenterY,RotateAngle
            if (IsOneObjectSelected && m_adorner != null)
            {
                RotateTransform rotateTransform = new RotateTransform();

                Point toTranslate = new Point((double)valArray[0] * m_adorner.ActualWidth, (double)valArray[1] * m_adorner.ActualHeight);
                Point center = m_adorner.TranslatePoint(toTranslate, m_activeContent.MainCanvas);
                rotateTransform.Angle = (double)valArray[2];
                rotateTransform.CenterX = center.X;
                rotateTransform.CenterY = center.Y;

                TransformGroup transformGroup = m_transform as TransformGroup;
                if (transformGroup == null)
                {
                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(m_transform);
                    transformGroup.Children.Add(rotateTransform);
                    m_adorner.AdornedElement.RenderTransform = transformGroup;
                }
                else
                {
                    transformGroup.Children.Add(rotateTransform);
                }
                m_adorner.AdornedElement.InvalidateVisual();
            }
        }

        private void ApplyScale(object values)
        {
            var valArray = (object[])values;//ScaleCenterX,ScaleCenterY,ScaleX,ScaleY
            if (IsOneObjectSelected && m_adorner != null)
            {
                ScaleTransform scaleTransform = new ScaleTransform();

                Point toTranslate = new Point((double)valArray[0] * m_adorner.ActualWidth,
                    (double)valArray[1] * m_adorner.ActualHeight);

                Point center = m_adorner.TranslatePoint(toTranslate, m_activeContent.MainCanvas);
                scaleTransform.ScaleX = (double)valArray[2];
                scaleTransform.ScaleY = (double)valArray[3];
                scaleTransform.CenterX = center.X;
                scaleTransform.CenterY = center.Y;

                TransformGroup transformGroup = m_transform as TransformGroup;
                if (transformGroup == null)
                {
                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(m_transform);
                    transformGroup.Children.Add(scaleTransform);
                    m_adorner.AdornedElement.RenderTransform = transformGroup;
                }
                else
                {
                    transformGroup.Children.Add(scaleTransform);
                }
                m_adorner.AdornedElement.InvalidateVisual();
            }
        }

        private void ApplySkew(object values)
        {
            var valArray = (object[])values;//SkewCenterX,SkewCenterY,SkewX,SkewY
            if (IsOneObjectSelected && m_adorner != null)
            {
                SkewTransform skewTransform = new SkewTransform();

                Point toTranslate = new Point((double)valArray[0] * m_adorner.ActualWidth,
                    (double)valArray[1] * m_adorner.ActualHeight);

                Point center = m_adorner.TranslatePoint(toTranslate, m_activeContent.MainCanvas);
                skewTransform.AngleX = (double)valArray[2];
                skewTransform.AngleY = (double)valArray[3];
                skewTransform.CenterX = center.X;
                skewTransform.CenterY = center.Y;

                TransformGroup transformGroup = m_transform as TransformGroup;
                if (transformGroup == null)
                {
                    transformGroup = new TransformGroup();
                    transformGroup.Children.Add(m_transform);
                    transformGroup.Children.Add(skewTransform);
                    m_adorner.AdornedElement.RenderTransform = transformGroup;
                }
                else
                {
                    transformGroup.Children.Add(skewTransform);
                }
                m_adorner.AdornedElement.InvalidateVisual();
            }
        }
    }
}
