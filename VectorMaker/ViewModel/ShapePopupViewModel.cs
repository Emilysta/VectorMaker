using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Shapes;
using VectorMaker.Intefaces;
using VectorMaker.Utility;

namespace VectorMaker.ViewModel
{
    internal class ShapePopupViewModel : ToolBaseViewModel
    {
        #region Fields
        private ObservableCollection<ResizingAdorner> m_selectedObjects = null;
        private Shape m_shape = null;
        #endregion

        #region Properties
        protected override string m_title { get; set; } = "Shape Popup";

        public bool IsEllipse { get; set; }

        public bool IsRect { get; set; }

        public bool IsLine { get; set; }

        public bool IsPoly { get; set; }

        public bool IsPath { get; set; }

        public string ShapeLabel { get; set; } = "NoShapeSelected";

        public Shape SelectedShape
        {
            get => m_shape;
            set
            {
                m_shape = value;
                OnPropertyChanged(nameof(SelectedShape));
            }
        }

        #endregion

        #region Constructors
        public ShapePopupViewModel(IMainWindowViewModel interfaceMainWindowVM) : base(interfaceMainWindowVM)
        {
        }
        #endregion

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
            if (m_selectedObjects.Count <= 0) return;
            SelectedShape = m_selectedObjects[0]?.AdornedElement as Shape;
            IsRect = false;
            IsEllipse = false;
            IsPath = false;
            IsLine = false;
            IsPoly = false;
            if (m_shape is Ellipse)
            {
                IsEllipse = true;
                ShapeLabel = "Ellipse";
            }
            if (m_shape is Rectangle)
            {
                IsRect = true;
                ShapeLabel = "Rectangle";
            }
            if (m_shape is Path)
            {
                IsPath = true;
                ShapeLabel = "Path";
            }
            if (m_shape is Polyline)
            {
                IsPoly = true;
                ShapeLabel = "Polyline";
            }
            if (m_shape is Polygon)
            {
                IsPoly = true;
                ShapeLabel = "Polygon";
            }
            if (m_shape is Line)
            {
                IsLine = true;
                ShapeLabel = "Line";
            }

            OnAllPropertiesChanged();
        }

        #endregion
    }
}
