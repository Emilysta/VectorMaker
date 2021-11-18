using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using VectorMaker.Commands;
using VectorMaker.Intefaces;
using VectorMaker.Utility;

namespace VectorMaker.ViewModel
{
    internal class ObjectPropertiesViewModel : ToolBaseViewModel
    {
        #region Fields
        private Visibility m_blendVisibiity;
        private ObservableCollection<ResizingAdorner> m_selectedObjects = null;
        #endregion

        #region Properties
        public Visibility BlendVisibility
        {
            get => m_blendVisibiity;
            set
            {
                m_blendVisibiity = value;
                OnPropertyChanged(nameof(BlendVisibility));
            }
        }

        public Brush FillBrush
        {
            get => SelectedObject?.Fill ?? Brushes.Transparent;
            set
            {
                if (SelectedObject != null)
                    SelectedObject.Fill = value;
            }
        }

        public Brush StrokeBrush
        {
            get => SelectedObject?.Stroke ?? Brushes.Transparent;
            set
            {
                if (SelectedObject != null)
                    SelectedObject.Stroke = value;
            }
        }

        public Shape SelectedObject { get; set; }

        protected override string m_title { get; set; } = "Fill & stroke";
        private bool IsOneObjectSelected => m_selectedObjects?.Count == 1;

        #endregion

        #region Commands

        public ICommand ApplyTranslationCommand { get; set; }

        #endregion

        #region Constructors

        public ObjectPropertiesViewModel(IMainWindowViewModel interfaceMainWindowVM) : base(interfaceMainWindowVM)
        {
            ApplyTranslationCommand = new CommandBase((obj) => ApplyTranslation());
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
            if (m_selectedObjects != null && m_selectedObjects.Count > 0)
                SelectedObject = m_selectedObjects[0].AdornedElement as Shape;

            if (IsOneObjectSelected)
                BlendVisibility = Visibility.Hidden;
            else
                BlendVisibility = Visibility.Visible;
        }

        #endregion

        #region Methods
        private void ApplyTranslation()
        {

        }

        #endregion



        //private void ObjectPropertiesTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    foreach (var item in e.AddedItems)
        //        (item as TabItem).Background = ColorsReference.selectedTabItemBackground;
        //    foreach (var item in e.RemovedItems)
        //        (item as TabItem).Background = ColorsReference.notSelectedTabItemBackground;
        //}
    }
}
