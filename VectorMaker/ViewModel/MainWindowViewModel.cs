using System.Windows.Media;
using VectorMaker.Drawables;
using AvalonDock.Layout;
using VectorMaker.Pages;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Input;
using VectorMaker.Commands;

namespace VectorMaker.ViewModel
{
    public class MainWindowViewModel
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

        public ObservableCollection<DrawingCanvasViewModel> Documents { get; set; }
        public DrawingCanvasViewModel ActiveCanvas { get; set; }

        public void SetDrawableType(DrawableTypes type)
        {
            foreach (DrawingCanvasViewModel canvas in Documents)
            {
                canvas.DrawableType = type;
            }
        }

        public MainWindowViewModel()
        {
            SetCommands();
            Documents = new ObservableCollection<DrawingCanvasViewModel>();
            Documents.Add(new DrawingCanvasViewModel());
        }

        private void SetCommands()
        {
            NewDocumentCommand = new CommandBase((obj)=> NewDocument());
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
            Documents.Add(new DrawingCanvasViewModel());
        }

        private void OpenDocument()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Scalable Vector Graphics (*.svg) | *.svg | Extensible Application Markup Language (*.xaml) | *.xaml";
            if (openFileDialog.ShowDialog() == true)
                Documents.Add(new DrawingCanvasViewModel(openFileDialog.FileName));
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

    }
}
