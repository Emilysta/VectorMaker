using VectorMaker.Drawables;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.Intefaces;
using System.Collections.Generic;
using System;
using VectorMaker.Utility;

namespace VectorMaker.ViewModel
{
    internal class MainWindowViewModel : NotifyPropertyChangedBase, IMainWindowViewModel
    {
        #region Commands
        public ICommand OpenApplicationSettingsCommand { get; set; }

        public ICommand NewDocumentCommand { get; set; }
        public ICommand OpenDocumentCommand { get; set; }
        public ICommand SaveAllCommand { get; set; }
        public ICommand CloseAllCommand { get; set; }

        public ICommand OpenTransformToolCommand { get; set; }
        public ICommand OpenPropertiesToolCommand { get; set; }
        public ICommand OpenColorPickerToolCommand { get; set; }
        public ICommand OpenAlignmentToolCommand { get; set; }
        public ICommand OpenLayersToolCommand { get; set; }

        public ICommand DrawRectangleCommand { get; set; }
        public ICommand DrawEllipseCommand { get; set; }
        public ICommand DrawLineCommand { get; set; }
        public ICommand DrawPolylineCommand { get; set; }
        public ICommand DrawPolygonCommand { get; set; }
        public ICommand SelectionToolCommand { get; set; }
        public ICommand UnionCommand { get; set; }
        #endregion

        #region Fields
        private ObservableCollection<DocumentViewModelBase> m_documents { get; set; }
        private ObservableCollection<ToolBaseViewModel> m_tools = null;
        private DocumentViewModelBase m_activeDocument;

        private ObjectTransformsViewModel m_objectTransformsVMTool = null;
        private ObjectAlignmentViewModel m_objectAlignmentVMTool = null;
        private ObjectPropertiesViewModel m_objectPropertiesVMTool = null;
        private DrawingLayersToolViewModel m_drawingLayersVMTool = null;
        #endregion

        #region Properties
        public ObjectTransformsViewModel ObjectTransformsVMTool
        {
            get
            {
                if (m_objectTransformsVMTool == null)
                    m_objectTransformsVMTool = new ObjectTransformsViewModel(this as IMainWindowViewModel);
                return m_objectTransformsVMTool;
            }
        }

        public ObjectAlignmentViewModel ObjectAlignmentVMTool
        {
            get
            {
                if (m_objectAlignmentVMTool == null)
                    m_objectAlignmentVMTool = new ObjectAlignmentViewModel(this as IMainWindowViewModel);
                return m_objectAlignmentVMTool;
            }
        }

        public ObjectPropertiesViewModel ObjectPropertiesVMTool
        {
            get
            {
                if (m_objectPropertiesVMTool == null)
                    m_objectPropertiesVMTool = new ObjectPropertiesViewModel(this as IMainWindowViewModel);
                return m_objectPropertiesVMTool;
            }
        }

        public DrawingLayersToolViewModel DrawingLayersVMTool
        {
            get
            {
                if (m_drawingLayersVMTool == null)
                {
                    m_drawingLayersVMTool = new DrawingLayersToolViewModel(this as IMainWindowViewModel);
                }
                return m_drawingLayersVMTool;
            }
        }

        public ObservableCollection<ToolBaseViewModel> Tools
        {
            get
            {
                if (m_tools == null)
                    m_tools = new ObservableCollection<ToolBaseViewModel>() { ObjectTransformsVMTool, ObjectAlignmentVMTool, ObjectPropertiesVMTool, DrawingLayersVMTool };
                return m_tools;
            }
        }

        public IEnumerable<DocumentViewModelBase> Documents => m_documents;
        public DocumentViewModelBase ActiveDocument
        {
            get => m_activeDocument;
            set
            {
                m_activeDocument = value;
                OnPropertyChanged(nameof(ActiveDocument));
                ActiveCanvasChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion
        public MainWindowViewModel()
        {
            SetCommands();
            m_documents = new ObservableCollection<DocumentViewModelBase>();
            DrawingCanvasViewModel drawingCanvas = new(this as IMainWindowViewModel);
            m_documents.Add(drawingCanvas);
        }

        #region Interface methods
        public void Close(DocumentViewModelBase fileToClose)
        {
            if (!fileToClose.IsSaved)
            {
                var result = MessageBox.Show(string.Format("Do you want to save changes " +
                    "for file '{0}'?", fileToClose.FileName), "VectorMaker",
                    MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Cancel)
                    return;

                if (result == MessageBoxResult.Yes)
                {
                    Save(fileToClose);
                }
            }
            m_documents.Remove(fileToClose);
        }

        public void AddFile(DocumentViewModelBase fileToAdd)
        {
            if (fileToAdd == null) return;
            if (m_documents.Contains(fileToAdd)) return;
            m_documents.Add(fileToAdd);
        }

        public void Save(DocumentViewModelBase fileToSave)
        {
            fileToSave.SaveCommand.Execute(null);
        }

        public void CloseAllDocuments()
        {
            ActiveDocument = null;
            m_documents.Clear();
        }

        public void CloseTool(ToolBaseViewModel tool)
        {
            if (Tools.Contains(tool))
                Tools.Remove(tool);
        }

        #endregion

        public event EventHandler ActiveCanvasChanged;

        #region Methods
        public void SetDrawableType(DrawableTypes type)
        {
            foreach (DrawingCanvasViewModel canvas in m_documents)
            {
                canvas.DrawableType = type;
            }
        }

        private void SetCommands()
        {
            OpenApplicationSettingsCommand = new CommandBase((obj) => OpenAppSettings());

            NewDocumentCommand = new CommandBase((obj) => NewDocument());
            OpenDocumentCommand = new CommandBase((obj) => OpenDocument());
            SaveAllCommand = new CommandBase((obj) => SaveAllDocuments());
            CloseAllCommand = new CommandBase((obj) => CloseAllDocuments());

            OpenTransformToolCommand = new CommandBase((obj) => TransformTool());
            OpenPropertiesToolCommand = new CommandBase((obj) => PropertiesTool());
            OpenColorPickerToolCommand = new CommandBase((obj) => ColorPickerTool());
            OpenAlignmentToolCommand = new CommandBase((obj) => AlignmentTool());
            OpenLayersToolCommand = new CommandBase((obj) => LayersTool());

            DrawRectangleCommand = new CommandBase((obj) => DrawRectangle());
            DrawEllipseCommand = new CommandBase((obj) => DrawEllipse());
            DrawLineCommand = new CommandBase((obj) => DrawLine());
            DrawPolylineCommand = new CommandBase((obj) => DrawPolyline());
            DrawPolygonCommand = new CommandBase((obj) => DrawPolygon());
            SelectionToolCommand = new CommandBase((obj) => SelectionToolEnable());

            UnionCommand = new CommandBase((obj) => Union());

        }

        private void DrawRectangle()
        {
            SetDrawableType(DrawableTypes.Rectangle);
        }

        private void DrawEllipse()
        {
            SetDrawableType(DrawableTypes.Ellipse);
        }

        private void DrawLine()
        {
            SetDrawableType(DrawableTypes.Line);
        }

        private void DrawPolyline()
        {
            SetDrawableType(DrawableTypes.PolyLine);
        }

        private void DrawPolygon()
        {
            SetDrawableType(DrawableTypes.Polygon);
        }

        private void SelectionToolEnable()
        {
            SetDrawableType(DrawableTypes.None);
        }

        private void Union()
        {
            throw new NotImplementedException(); //toDo
        }

        private void OpenAppSettings()
        {
            AppSettingsViewModel appSettingsViewModel = new();
        }

        private void NewDocument()
        {
            DrawingCanvasViewModel drawingCanvas = new(this as IMainWindowViewModel);
            m_documents.Add(drawingCanvas);
        }

        private void OpenDocument()
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Filter = "Scalable Vector Graphics (*.svg) | *.svg | Extensible Application Markup Language (*.xaml) | *.xaml";
            if (openFileDialog.ShowDialog() == true)
            {
                DrawingCanvasViewModel drawingCanvas = new(openFileDialog.FileName, this as IMainWindowViewModel);
                m_documents.Add(drawingCanvas);
            }
        }

        private void SaveAllDocuments()
        {
            throw new NotImplementedException();
        }

        private void TransformTool()
        {
            if (!Tools.Contains(ObjectTransformsVMTool))
                Tools.Add(ObjectTransformsVMTool);
        }

        private void PropertiesTool()
        {
            if (!Tools.Contains(ObjectPropertiesVMTool))
                Tools.Add(ObjectPropertiesVMTool);
        }

        private void ColorPickerTool()
        {
            throw new NotImplementedException();  //toDo
        }

        private void AlignmentTool()
        {
            if (!Tools.Contains(ObjectAlignmentVMTool))
                Tools.Add(ObjectAlignmentVMTool);
        }

        private void LayersTool()
        {
            if (!Tools.Contains(DrawingLayersVMTool))
                Tools.Add(DrawingLayersVMTool);
        }

        //private void ChangeColor()
        //{
        //    System.Windows.Media.Color accentColor = ColorsReference.magentaBaseColor;
        //    Application.Current.Resources[AvalonDock.Themes.VS2013.Themes.ResourceKeys.ControlAccentColorKey] = accentColor;
        //    Application.Current.Resources[AvalonDock.Themes.VS2013.Themes.ResourceKeys.ControlAccentBrushKey] = new SolidColorBrush(accentColor);
        //}

        #endregion
    }
}
