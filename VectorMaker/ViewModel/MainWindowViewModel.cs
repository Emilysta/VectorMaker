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
using VectorMaker.Models;
using System.Windows;
using System.Collections.Specialized;

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
        public ICommand OpenAlignmentToolCommand { get; set; }
        public ICommand OpenLayersToolCommand { get; set; }

        public ICommand FillBrushPickCommand { get; set; }
        public ICommand StrokeBrushPickCommand { get; set; }

        public ICommand TestObjectsCountCommand { get; set; }
        public ICommand TestObjectsGroupingCommand { get; set; }
        public ICommand TestTransformsGroupCommand { get; set; }
        public ICommand TestSavingObjectsCommand { get; set; }
        #endregion

        #region Fields
        private ObservableCollection<DocumentViewModelBase> m_documents { get; set; }
        private ObservableCollection<ToolBaseViewModel> m_tools = null;
        private DocumentViewModelBase m_activeDocument;
        private ObservableCollection<ResizingAdorner> m_selectedObjects = null;
        private FrameworkElement m_selectedElement = null;

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
        public ToggleMenu TopMenu { get; set; }

        public ShapeProperties ShapePropertiesModel => ShapeProperties.Instance;

        public FrameworkElement SelectedElement
        {
            get => m_selectedElement;
            set
            {
                m_selectedElement = value;
                OnPropertyChanged(nameof(SelectedElement));
                OnPropertyChanged(nameof(SelectedObjectLabel));
                OnPropertyChanged(nameof(SelectedObjectTransformX));
                OnPropertyChanged(nameof(SelectedObjectTransformY));
            }
        }

        public string SelectedObjectLabel
        {
            get => SelectedElement?.ToString() ?? "No object selected";
        }

        public double SelectedObjectTransformX
        {
            get => SelectedElement?.RenderTransform.Value.OffsetX ?? 0;
            set
            {
                Matrix matrix = SelectedElement.RenderTransform.Value;
                matrix.OffsetX = value;
                SelectedElement.RenderTransform = new MatrixTransform(matrix);
                OnPropertyChanged(nameof(SelectedObjectTransformX));
            }
        }
        public double SelectedObjectTransformY
        {
            get => SelectedElement?.RenderTransform.Value.OffsetY ?? 0;
            set
            {
                Matrix matrix = SelectedElement.RenderTransform.Value;
                matrix.OffsetY = value;
                SelectedElement.RenderTransform = new MatrixTransform(matrix);
                OnPropertyChanged(nameof(SelectedObjectTransformY));
            }
        }

        #endregion

        public MainWindowViewModel()
        {
            SetCommands();
            m_documents = new ObservableCollection<DocumentViewModelBase>();
            CreateLeftMenu();
            CreateTopMenu();
            ActiveCanvasChanged += OnActiveCanvasChanged;
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

        public void OnActiveCanvasChanged(object sender, EventArgs e)
        {
            if (ActiveDocument is DrawingCanvasViewModel drawingCanvasViewModel)
            {
                if (m_selectedObjects != null)
                    m_selectedObjects.CollectionChanged -= SelectedObjectsCollectionChanged;

                m_selectedObjects = drawingCanvasViewModel.SelectedObjects;
                m_selectedObjects.CollectionChanged += SelectedObjectsCollectionChanged;
            }
        }

        private void SelectedObjectsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (SelectedElement != null)
            {
                UIPropertyChanged.Instance.PropertyChanged -= SelectedObjectRenderTransformChanged;
            }
            if (m_selectedObjects.Count <= 0)
            {
                OnPropertyChanged(nameof(SelectedElement));
                SelectedElement = null;
                return;
            }
            SelectedElement = m_selectedObjects[0]?.AdornedElement as FrameworkElement;
            if (SelectedElement != null)
            {
                UIPropertyChanged.Instance.PropertyChanged += SelectedObjectRenderTransformChanged;
            }
        }

        private void SelectedObjectRenderTransformChanged()
        {
            OnPropertyChanged(nameof(SelectedObjectTransformX));
            OnPropertyChanged(nameof(SelectedObjectTransformY));
        }

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
                toolTip: "Selection\nShift - MultiSelection", isChecked: true));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorPolyline, () => SetDrawableType(DrawableTypes.PolyLine),
                toolTip: "Polyline"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorPolygon, () => SetDrawableType(DrawableTypes.Polygon),
                toolTip: "Polygon"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.TextRecognition, () => SetDrawableType(DrawableTypes.Text),
                 toolTip: "Text"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorUnion, () => CombineGeometries(GeometryCombineMode.Union),
                isToggleButton: false, toolTip: "Union"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorIntersection, () => CombineGeometries(GeometryCombineMode.Intersect),
                isToggleButton: false, toolTip: "Intersect"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorDifferenceAb, () => CombineGeometries(GeometryCombineMode.Exclude),
                isToggleButton: false, toolTip: "Exclude"));
            LeftMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.VectorDifference, () => CombineGeometries(GeometryCombineMode.Xor),
                isToggleButton: false, toolTip: "XOR"));

        }

        private void CreateTopMenu()
        {
            TopMenu = new ToggleMenu();

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.File, () => NewDocument(), isToggleButton: false, toolTip: "New File"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.FileFind, () =>OpenDocument(), isToggleButton: false, toolTip: "Open File"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.ContentSave, () => Save(ActiveDocument), isToggleButton: false, toolTip: "Save Current File"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.ContentSaveAll, () => SaveAllDocuments(), isToggleButton: false, toolTip: "Save All Files"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.CloseBox, () => Close(ActiveDocument), isToggleButton: false, toolTip: "Close Current File"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.CloseBoxMultiple, () => CloseAllDocuments(), isToggleButton: false, toolTip: "Close All Files"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.FileExport, () => { ActiveDocument.SaveAsCommand.Execute("png"); }, isToggleButton: false, toolTip: "Export"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.Printer, () => { ActiveDocument.PrintCommand.Execute(null); }, isToggleButton: false, toolTip: "Print"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.Group, () => GroupObjects(), isToggleButton: false, toolTip: "Group"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.Ungroup, () => UngroupObjects(), isToggleButton: false, toolTip: "Ungroup"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.FlipHorizontal, () => { FlipHorizontalObjects(); }, isToggleButton: false, toolTip: "Flip horizontal"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.FlipVertical, () => { FlipVerticalObjects(); }, isToggleButton: false, toolTip: "Flip vertical"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.RotateLeftVariant, () => { RotateCounterclockwiseObjects(); }, isToggleButton: false, toolTip: "Rotate left"));

            TopMenu.AddNewButton(new ToggleButtonForMenu(PackIconMaterialKind.RotateRightVariant, () => { RotateClockwiseObjects(); }, isToggleButton: false, toolTip: "Rotate right"));
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
            OpenAlignmentToolCommand = new CommandBase((obj) => CreateTool(ObjectAlignmentVMTool));
            OpenLayersToolCommand = new CommandBase((obj) => CreateTool(DrawingLayersVMTool));

            FillBrushPickCommand = new CommandBase((obj) => FillBrushColorPick());
            StrokeBrushPickCommand = new CommandBase((obj) => StrokeBrushColorPick());
            TestObjectsCountCommand = new CommandBase((_) => TestCountOfObjects());
            TestObjectsGroupingCommand = new CommandBase((_) => TestGroupingOfObjects());
            TestTransformsGroupCommand = new CommandBase((_) => TestTransformsOfGroup());
            TestSavingObjectsCommand = new CommandBase((_) => TestSavingObjects());
        }

        private void FillBrushColorPick()
        {
            ColorPickerViewModel colorPicker = new ColorPickerViewModel(ShapePropertiesModel.FillBrush);
            colorPicker.ShowWindowAndWaitForResult();
        }

        private void StrokeBrushColorPick()
        {
            ColorPickerViewModel colorPicker = new ColorPickerViewModel(ShapePropertiesModel.StrokeBrush);
            colorPicker.ShowWindowAndWaitForResult();
        }

        private void CombineGeometries(GeometryCombineMode mode)
        {
            (m_activeDocument as DrawingCanvasViewModel).CombineTwoGeometries(mode);
        }
        private void GroupObjects()
        {
            (m_activeDocument as DrawingCanvasViewModel).GroupObjects(out double measure);
        }

        private void UngroupObjects()
        {
            (m_activeDocument as DrawingCanvasViewModel).UngroupObjects(out double measure);
        }
        private void FlipVerticalObjects()
        {
            (m_activeDocument as DrawingCanvasViewModel).FlipVerticalCommand.Execute(null);
        }
        private void FlipHorizontalObjects()
        {
            (m_activeDocument as DrawingCanvasViewModel).FlipHorizontalCommand.Execute(null);   
        }
        private void RotateClockwiseObjects()
        {
            (m_activeDocument as DrawingCanvasViewModel).RotateClockwiseCommand.Execute(null);
        }
        private void RotateCounterclockwiseObjects()
        {
            (m_activeDocument as DrawingCanvasViewModel).RotateCounterclockwiseCommand.Execute(null);
        }

        private void OpenAppSettings()
        {
            AppSettingsViewModel appSettingsViewModel = new();
        }
        private void NewDocument()
        {
            CreateDocumentViewModel createDocumentViewModel = new CreateDocumentViewModel();
            if (createDocumentViewModel.IsFileToBeCreated)
            {
                DrawingCanvasViewModel drawingCanvas; 
                if (createDocumentViewModel.IsVertical)
                    drawingCanvas = new(this as IMainWindowViewModel, createDocumentViewModel.Width, createDocumentViewModel.Height);
                else
                    drawingCanvas = new(this as IMainWindowViewModel, createDocumentViewModel.Height, createDocumentViewModel.Width);
                m_documents.Add(drawingCanvas);
            }
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
            foreach(var document in m_documents)
                document.SaveCommand.Execute(null);
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

        #region TestMethods
        private void TestCountOfObjects()
        {
            (m_activeDocument as DrawingCanvasViewModel).TestCountOfObjects();
        }

        private void TestGroupingOfObjects()
        {
            (m_activeDocument as DrawingCanvasViewModel).TestGroupingOfObjects();
        }

        private void TestTransformsOfGroup()
        {
            (m_activeDocument as DrawingCanvasViewModel).TestTransformsOfGroup();
        }

        private void TestSavingObjects()
        {
            (m_activeDocument as DrawingCanvasViewModel).TestSavingObjects();
        }
        #endregion
    }
}
