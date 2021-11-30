using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Shapes;
using VectorMaker.Utility;

namespace VectorMaker.ViewModel
{
    internal class ShapePopupViewModel :NotifyPropertyChangedBase
    {
        #region Fields
        private ObservableCollection<ResizingAdorner> m_selectedObjects = null;
        private Shape m_shape = null;
        #endregion

        #region Properties

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
        public ShapePopupViewModel(DrawingCanvasViewModel model) 
        {
            m_selectedObjects = model.SelectedObjects;
            model.SelectedObjects.CollectionChanged += SelectedObjectsCollectionChanged;
        }
        #endregion

        #region EventHandlers

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
