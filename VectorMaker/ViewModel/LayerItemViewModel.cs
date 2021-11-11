using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.IconPacks;
using VectorMaker.Commands;
using VectorMaker.ControlsResources;
using VectorMaker.Utility;
using Vis = System.Windows.Visibility;

namespace VectorMaker.ViewModel
{
    class LayerItemViewModel : NotifyPropertyChangedBase
    {
        private Canvas m_layer;
        private int m_layerNumber;
        private string m_layerName;
        private PackIconFontAwesomeKind m_iconKind => 
            m_isVisible ? PackIconFontAwesomeKind.EyeRegular : PackIconFontAwesomeKind.EyeSlashRegular;
        private bool m_isVisible = true;
        private bool m_isUpVisible = true;
        private bool m_isDownVisible = true;

        public Action<LayerItemViewModel> DeleteAction;
        public Action<LayerItemViewModel> MoveUpAction;
        public Action<LayerItemViewModel> MoveDownAction;

        public Canvas Layer
        {
            get => m_layer;
            set
            {
                m_layer = value;
                OnPropertyChanged(nameof(Layer));
            }
        }
        public int LayerNumber
        {
            get => m_layerNumber;
            set
            {
                m_layerNumber = value;
                OnPropertyChanged(nameof(LayerNumber));
            }
        }
        public string LayerName
        {
            get => m_layerName;
            set
            {
                m_layerName = value;
                OnPropertyChanged(nameof(LayerName));
                Layer.Name = LayerName;
            }
        }
        public bool IsVisible
        {
            get => m_isVisible;
            set {
                m_isVisible= value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }
        public bool IsUpVisible
        {
            get => m_isUpVisible;
            set
            {
                m_isUpVisible = value;
                OnPropertyChanged(nameof(IsUpVisible));
            }
        }
        public bool IsDownVisible
        {
            get => m_isDownVisible;
            set
            {
                m_isDownVisible = value;
                OnPropertyChanged(nameof(IsDownVisible));
            }
        }

        public ICommand DeleteCommand { get; set; }
        public ICommand ChangeVisibilityCommand { get; set; }
        public ICommand MoveUpCommand { get; set; }
        public ICommand MoveDownCommand { get; set; }

        public LayerItemViewModel(Canvas canvas, int layerNumber, string layername)
        {
            Layer = canvas;
            LayerNumber = layerNumber;
            LayerName = layername;
            IsVisible = true;
            Layer.Name = LayerName;
            Layer.Visibility = Vis.Visible;
            SetCommands();
        }
        private void SetCommands()
        {
            DeleteCommand = new CommandBase((obj) => DeleteThisObject());
            ChangeVisibilityCommand = new CommandBase((obj) => ChangeVisibility(obj as ToggleButtonWithIcon));
            MoveUpCommand = new CommandBase((obj) => MoveUpLayer());
            MoveDownCommand = new CommandBase((obj) => MoveDownLayer());
        }
        private void DeleteThisObject()
        {
            var result = MessageBox.Show("Are you sure you want to delete this layer?",$"Delete Layer+{LayerNumber}",MessageBoxButton.YesNoCancel);
            if(result == MessageBoxResult.Yes)
            {
                DeleteAction?.Invoke(this);
            }
        }
        private void ChangeVisibility(ToggleButtonWithIcon toggleButton)
        {
            IsVisible = !IsVisible;
            Layer.Visibility = IsVisible ? Vis.Visible : Vis.Hidden;
            toggleButton.IsChecked = !toggleButton.IsChecked;
            toggleButton.IconKind = m_iconKind;
        }
        private void MoveUpLayer()
        {
            MoveUpAction?.Invoke(this);
        }
        private void MoveDownLayer()
        {
            MoveDownAction?.Invoke(this);
        }
    }
}
