
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.Intefaces;

namespace VectorMaker.ViewModel
{
    internal class DrawingLayersToolViewModel : ToolBaseViewModel
    {
        #region Fields
        private ObservableCollection<LayerItemViewModel> m_drawingLayers;
        private DrawingCanvasViewModel m_drawingCanvas;
        private LayerItemViewModel m_selectedLayer;
        private int m_numberOfLayers;
        #endregion

        #region Properties
        public ObservableCollection<LayerItemViewModel> DrawingLayers
        {
            get => m_drawingLayers;
            set
            {
                m_drawingLayers = value;
                OnPropertyChanged(nameof(DrawingLayers));
            }
        }

        public LayerItemViewModel SelectedLayer
        {
            get => m_selectedLayer;
            set
            {
                m_selectedLayer = value;
                OnPropertyChanged(nameof(SelectedLayer));
                m_drawingCanvas.SelectedLayer = m_selectedLayer;
            }
        }

        protected override string m_title { get; set; } = "Layers";

        #endregion

        #region Commands

        public ICommand AddLayerCommand { get; set; }
        public ICommand DeleteLayerCommand { get; set; }


        #endregion

        #region Constructors

        public DrawingLayersToolViewModel(IMainWindowViewModel interfaceMainWindowVM) : base(interfaceMainWindowVM)
        {
            AddLayerCommand = new CommandBase((obj) => AddLayer());
            DeleteLayerCommand = new CommandBase((obj) => DeleteLayer(obj as LayerItemViewModel));
        }
        #endregion Constructors

        #region EventHandlers
        public override void OnActiveCanvasChanged(object sender, EventArgs e)
        {
            DrawingCanvasViewModel drawingCanvasViewModel = m_interfaceMainWindowVM.ActiveDocument as DrawingCanvasViewModel;
            if (drawingCanvasViewModel != null)
            {
                m_drawingCanvas = drawingCanvasViewModel;
                DrawingLayers = drawingCanvasViewModel.Layers;
                SelectedLayer = drawingCanvasViewModel.SelectedLayer;
                m_numberOfLayers = drawingCanvasViewModel.LayersNumber;
            }
        }
        #endregion

        #region Methods

        private void AddLayer()
        {
            m_numberOfLayers++;
            LayerItemViewModel layer = new(new System.Windows.Controls.Canvas(), m_numberOfLayers, $"Layer_{m_numberOfLayers}");
            layer.DeleteAction = DeleteLayer;
            DrawingLayers.Add(layer);
            m_drawingCanvas.MainCanvas.Children.Add(layer.Layer);
        }

        private void DeleteLayer(LayerItemViewModel layerItemViewModel)
        {
            DrawingLayers.Remove(layerItemViewModel);
            SelectedLayer = DrawingLayers.Last();
            m_drawingCanvas.MainCanvas.Children.Remove(layerItemViewModel.Layer);
        }
        #endregion
    }
}
