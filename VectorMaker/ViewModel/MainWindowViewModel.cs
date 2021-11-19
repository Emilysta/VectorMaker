using VectorMaker.Drawables;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.Intefaces;
using System.Collections.Generic;
using System;
using VectorMaker.Utility;
using MahApps.Metro.IconPacks;
using System.Windows.Media;

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
        public ToggleMenu LeftMenu { get; set; }
        #endregion

        public MainWindowViewModel()
        {
            SetCommands();
            m_documents = new ObservableCollection<DocumentViewModelBase>();
            CreateLeftMenu();
            DrawingCanvasViewModel drawingCanvas = new(this as IMainWindowViewModel);
            m_documents.Add(drawingCanvas);
        }

        #region Interface methods
        public void Close(DocumentViewModelBase fileToClose)
        {
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
        private void CreateLeftMenu()
        {
            LeftMenu = new ToggleMenu();
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.SquareOutline, () => SetDrawableType(DrawableTypes.Rectangle), 
                toolTip: "Rectangle\nCtrl - Square"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.CircleOutline, () => SetDrawableType(DrawableTypes.Ellipse), 
                toolTip: "Ellipse\nCtrl - Cricle"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorLine, () => SetDrawableType(DrawableTypes.Line), 
                toolTip: "Line"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorSquare, () => SetDrawableType(DrawableTypes.None), 
                toolTip: "Selection\nShift - MultiSelection",isChecked: true));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorPolyline, () => SetDrawableType(DrawableTypes.PolyLine), 
                toolTip: "Polyline"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorPolygon, () => SetDrawableType(DrawableTypes.Polygon), 
                toolTip: "Polygon"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorUnion, () => CombineGeometries(GeometryCombineMode.Union), 
                isToggleButton: false, toolTip: "Union"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorIntersection, () => CombineGeometries(GeometryCombineMode.Intersect), 
                isToggleButton: false, toolTip: "Intersect"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorDifferenceAb, () => CombineGeometries(GeometryCombineMode.Exclude), 
                isToggleButton: false, toolTip: "Exclude"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorDifference, () => CombineGeometries(GeometryCombineMode.Xor), 
                isToggleButton: false, toolTip: "XOR"));
        }
        private void SetCommands()
        {
            OpenApplicationSettingsCommand = new CommandBase((obj) => OpenAppSettings());

            NewDocumentCommand = new CommandBase((obj) => NewDocument());
            OpenDocumentCommand = new CommandBase((obj) => OpenDocument());
            SaveAllCommand = new CommandBase((obj) => SaveAllDocuments());
            CloseAllCommand = new CommandBase((obj) => CloseAllDocuments());

            OpenTransformToolCommand = new CommandBase((obj) => CreateTool(ObjectTransformsVMTool));
            OpenPropertiesToolCommand = new CommandBase((obj) => CreateTool(ObjectPropertiesVMTool));
            OpenColorPickerToolCommand = new CommandBase((obj) => ColorPickerTool());
            OpenAlignmentToolCommand = new CommandBase((obj) => CreateTool(ObjectAlignmentVMTool));
            OpenLayersToolCommand = new CommandBase((obj) => CreateTool(DrawingLayersVMTool));
        }
        private void CombineGeometries(GeometryCombineMode mode)
        {
            (m_activeDocument as DrawingCanvasViewModel).CombineTwoGeometries(mode);
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
        private void ColorPickerTool()
        {
            throw new NotImplementedException();  //toDo
        }
        private void CreateTool(ToolBaseViewModel tool)
        {
            if (!Tools.Contains(tool))
                Tools.Add(tool);
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
