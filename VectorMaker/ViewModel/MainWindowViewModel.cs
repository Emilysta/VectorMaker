using System.Windows.Media;
using VectorMaker.Drawables;
using AvalonDock.Layout;
using VectorMaker.Pages;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.Intefaces;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using VectorMaker.Utility;

namespace VectorMaker.ViewModel
{
    internal class MainWindowViewModel : NotifyPropertyChangedBase, IMainWindowViewModel
    {
        #region Commands
        public ICommand NewDocumentCommand { get; set; }
        public ICommand OpenDocumentCommand { get; set; }
        public ICommand SaveAllCommand { get; set; }

        public ICommand OpenTransformToolCommand { get; set; }
        public ICommand OpenPropertiesToolCommand { get; set; }
        public ICommand OpenColorPickerToolCommand { get; set; }
        public ICommand OpenAlignmentToolCommand { get; set; }

        public ICommand DrawRectangleCommand { get; set; }
        public ICommand DrawEllipseCommand { get; set; }
        public ICommand DrawLineCommand { get; set; }
        public ICommand DrawPolylineCommand { get; set; }
        public ICommand DrawPolygonCommand { get; set; }
        public ICommand SelectionToolCommand { get; set; }

        public ICommand UnionCommand { get; set; }

        #endregion

        #region Fields
        private ObservableCollection<DrawingCanvasViewModel> m_documents { get; set; }
        private ToolBaseViewModel[] m_tools = null;
        private DrawingCanvasViewModel m_activeCanvas;

        private ObjectTransformsViewModel m_objectTransformsVMTool = null;
        //private ObjectAlignmentViewModel m_objectAlignmentVMTool;
        //private ObjectPropertiesViewModel m_objectPropertiesVMTool;
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

        public IEnumerable<ToolBaseViewModel> Tools
        {
            get
            {
                if (m_tools == null)
                    m_tools = new ToolBaseViewModel[] { ObjectTransformsVMTool };
                return m_tools;
            }
        }

        public IEnumerable<DrawingCanvasViewModel> Documents => m_documents;
        public DrawingCanvasViewModel ActiveCanvas
        {
            get => m_activeCanvas;
            set
            {
                m_activeCanvas = value;
                OnPropertyChanged("ActiveCanvas");
                ActiveCanvasChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        #endregion

        public MainWindowViewModel()
        {
            SetCommands();
            m_documents = new ObservableCollection<DrawingCanvasViewModel>();
            m_documents.Add(new DrawingCanvasViewModel());
        }

        #region Interface methods
        public void Close(DrawingCanvasViewModel fileToClose)
        {
            if (!fileToClose.Model.IsSaved)
            {
                var result = MessageBox.Show(string.Format("Do you want to save changes " +
                    "for file '{0}'?", fileToClose.Model.FileName), "VectorMaker",
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

        public void AddFile(DrawingCanvasViewModel fileToAdd)
        {
            if (fileToAdd == null) return;
            if (m_documents.Contains(fileToAdd)) return;
            m_documents.Add(fileToAdd);
        }

        public void Save(DrawingCanvasViewModel fileToSave)
        {
            fileToSave.SaveCommand.Execute(null);
        }

        public Task<DrawingCanvasViewModel> OpenAsync(string filepath)
        {
            //DrawingCanvasViewModel drawingCanvasViewModel = Documents.FirstOrDefault(fm => fm.Model.FilePath == filepath);
            //if (drawingCanvasViewModel != null)
            //    return drawingCanvasViewModel;

            //drawingCanvasViewModel = new DrawingCanvasViewModel(filepath);
            //bool result = await drawingCanvasViewModel.OpenFileAsync(filepath);

            //if (result)
            //{
            //    Documents.Add(drawingCanvasViewModel);
            //    return drawingCanvasViewModel;
            //}

            return null;
        }

        public void CloseAllDocuments()
        {
            ActiveCanvas = null;
            m_documents.Clear();
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
            NewDocumentCommand = new CommandBase((obj) => NewDocument());
            OpenDocumentCommand = new CommandBase((obj) => OpenDocument());
            SaveAllCommand = new CommandBase((obj) => SaveAllDocuments());

            OpenTransformToolCommand = new CommandBase((obj) => TransformTool());
            OpenPropertiesToolCommand = new CommandBase((obj) => PropertiesTool());
            OpenColorPickerToolCommand = new CommandBase((obj) => ColorPickerTool());
            OpenAlignmentToolCommand = new CommandBase((obj) => AlignmentTool());

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
            //Frame frame = DockingManager.ActiveContent as Frame;
            //DrawingCanvas drawingCanvas = frame.Content as DrawingCanvas;
            //drawingCanvas.Union();
        }

        private void NewDocument()
        {
            m_documents.Add(new DrawingCanvasViewModel());
        }

        private void OpenDocument()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Scalable Vector Graphics (*.svg) | *.svg | Extensible Application Markup Language (*.xaml) | *.xaml";
            if (openFileDialog.ShowDialog() == true)
                m_documents.Add(new DrawingCanvasViewModel(openFileDialog.FileName));
        }

        private void SaveAllDocuments()
        {
            //foreach(DrawingCanvas drawingCanvas in Documents)
            //drawingCanvas.ViewModel.SaveFile();
        }

        private void SaveAsSVGDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            //m_drawingCanvas.SaveAsPDF();
        }

        private void SaveAsPNGDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame frame = DockingManager.ActiveContent as Frame;
            //DrawingCanvas drawingCanvas = frame.Content as DrawingCanvas;
            //drawingCanvas.SaveAsPNG();
        }

        private void SaveAsBMPDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            //m_drawingCanvas.SaveAsPDF();
            //DocumentPaneGroup.Active
        }

        private void SaveAsPDFDocumentButton_Click(object sender, RoutedEventArgs e)
        {
            //Frame frame = DockingManager.ActiveContent as Frame;
            //DrawingCanvas drawingCanvas = frame.Content as DrawingCanvas;
            //drawingCanvas.SaveAsPDF();
        }

        private void TransformTool()
        {
            //if (TransformTool.IsHidden)
            //    TransformTool.Show();
            //else if (TransformTool.IsVisible)
            //    TransformTool.IsActive = true;
            //else
            //    TransformTool.AddToLayout(DockingManager, AnchorableShowStrategy.Right | AnchorableShowStrategy.Most);
        }

        private void PropertiesTool()
        {
            //if (PropertiesTool.IsHidden)
            //    PropertiesTool.Show();
            //else if (PropertiesTool.IsVisible)
            //    PropertiesTool.IsActive = true;
            //else
            //    PropertiesTool.AddToLayout(DockingManager, AnchorableShowStrategy.Right | AnchorableShowStrategy.Most);
        }

        private void ColorPickerTool()
        {
            //if (ColorPicker.IsHidden)
            //    ColorPicker.Show();
            //else if (ColorPicker.IsVisible)
            //    ColorPicker.IsActive = true;
            //else
            //    ColorPicker.AddToLayout(DockingManager, AnchorableShowStrategy.Right | AnchorableShowStrategy.Most);
        }

        private void AlignmentTool()
        {
            //if (AlignmentTool.IsHidden)
            //    AlignmentTool.Show();
            //else if (AlignmentTool.IsVisible)
            //    AlignmentTool.IsActive = true;
            //else
            //    AlignmentTool.AddToLayout(DockingManager, AnchorableShowStrategy.Right | AnchorableShowStrategy.Most);
        }

        public void ChangeColor()
        {
            System.Windows.Media.Color accentColor = ColorsReference.magentaBaseColor;
            Application.Current.Resources[AvalonDock.Themes.VS2013.Themes.ResourceKeys.ControlAccentColorKey] = accentColor;
            Application.Current.Resources[AvalonDock.Themes.VS2013.Themes.ResourceKeys.ControlAccentBrushKey] = new SolidColorBrush(accentColor);
        }

        #endregion
    }
}
