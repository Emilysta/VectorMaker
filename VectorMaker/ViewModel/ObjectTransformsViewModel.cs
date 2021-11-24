using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using VectorMaker.Commands;
using VectorMaker.Intefaces;
using VectorMaker.Utility;

namespace VectorMaker.ViewModel
{
    internal class ObjectTransformsViewModel : ToolBaseViewModel
    {
        #region Fields
        private Visibility m_blendVisibiity;
        private ObservableCollection<ResizingAdorner> m_selectedObjects = null;
        private Adorner m_adorner = null;
        #endregion

        #region Properties
        public Visibility BlendVisibility
        {
            get => m_blendVisibiity;
            set
            {
                m_blendVisibiity = value;
                OnPropertyChanged("BlendVisibility");
            }
        }
        protected override string m_title { get; set; } = "Transforms";
        private bool IsOneObjectSelected => m_selectedObjects?.Count == 1;
        private Transform m_transform
        {
            get => m_adorner?.AdornedElement.RenderTransform;
            set
            {
                if (m_adorner != null)
                {
                    m_adorner.AdornedElement.RenderTransform = value;
                    m_adorner.AdornedElement.InvalidateVisual();
                }
            }
        }
        private Size m_adornedElementSize
        {
            get => (Size)m_adorner?.AdornedElement.RenderSize; 
        }

        #endregion

        #region Commands

        public ICommand ApplyTranslationCommand { get; set; }
        public ICommand ApplyRotationCommand { get; set; }
        public ICommand ApplyScaleCommand { get; set; }
        public ICommand ApplySkewCommand { get; set; }

        #endregion

        #region Constructors

        public ObjectTransformsViewModel(IMainWindowViewModel interfaceMainWindowVM) : base(interfaceMainWindowVM)
        {
            ApplyTranslationCommand = new CommandBase((obj) => ApplyTranslation(obj));
            ApplyRotationCommand = new CommandBase((obj) => ApplyRotation(obj));
            ApplyScaleCommand = new CommandBase((obj) => ApplyScale(obj));
            ApplySkewCommand = new CommandBase((obj) => ApplySkew(obj));
        }

        #endregion Constructors

        #region EventHandlers
        public override void OnActiveCanvasChanged(object sender, EventArgs e)
        {
            if (m_interfaceMainWindowVM.ActiveDocument is DrawingCanvasViewModel drawingCanvasViewModel)
            {
                if (m_selectedObjects != null)
                    m_selectedObjects.CollectionChanged -= SelectedObjectsCollectionChanged;

                m_selectedObjects = drawingCanvasViewModel.SelectedObjects;
                m_selectedObjects.CollectionChanged += SelectedObjectsCollectionChanged;
            }
        }

        private void SelectedObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (IsOneObjectSelected)
                BlendVisibility = Visibility.Hidden;
            else
                BlendVisibility = Visibility.Visible;

            if (m_selectedObjects.Count > 0)
                m_adorner = m_selectedObjects[0];
            else
                m_adorner = null;
            m_transform = m_adorner?.AdornedElement.RenderTransform;
        }

        #endregion

        #region Methods
        private void ApplyTranslation(object values)
        {
            var valArray = (Tuple<double, double>)values;
            if (IsOneObjectSelected && m_adorner != null)
            {
                Matrix matrix = m_transform.Value;
                matrix.Translate(valArray.Item1, valArray.Item2);
                m_transform = new MatrixTransform(matrix);
                m_interfaceMainWindowVM.ActiveDocument.IsSaved = false;
            }
        }

        private void ApplyRotation(object values)
        {
            var valArray = (Tuple<double, double, double>)values;//RotateCenterX,RotateCenterY,RotateAngle
            if (IsOneObjectSelected && m_adorner != null)
            {
                Matrix matrix = m_transform.Value;
                matrix.RotateAtPrepend(valArray.Item3,
                    valArray.Item1* m_adornedElementSize.Width, 
                    valArray.Item2* m_adornedElementSize.Width);
                m_transform = new MatrixTransform(matrix);
                m_interfaceMainWindowVM.ActiveDocument.IsSaved = false;
            }
        }

        private void ApplyScale(object values)
        {
            var valArray = (Tuple<double, double, double, double>)values;//ScaleCenterX,ScaleCenterY,ScaleX,ScaleY
            if (IsOneObjectSelected && m_adorner != null)
            {
                Matrix matrix = m_transform.Value;
                matrix.ScaleAtPrepend(valArray.Item3, valArray.Item4, 
                    valArray.Item1*m_adornedElementSize.Width, 
                    valArray.Item2* m_adornedElementSize.Height);
                m_transform = new MatrixTransform(matrix);
                m_interfaceMainWindowVM.ActiveDocument.IsSaved = false;
            }
        }

        private void ApplySkew(object values)
        {
            var valArray = (Tuple<double, double>)values;//SkewX,SkewY
            if (IsOneObjectSelected && m_adorner != null)
            {
                Matrix matrix = m_transform.Value;
                matrix.SkewPrepend(valArray.Item1, valArray.Item2);
                m_transform = new MatrixTransform(matrix);
                m_interfaceMainWindowVM.ActiveDocument.IsSaved = false;
            }
        }
        #endregion
    }
}
