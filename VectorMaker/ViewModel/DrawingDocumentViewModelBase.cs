using System;
using System.Windows;
using System.Windows.Markup;
using FileStream = System.IO.FileStream;
using File = System.IO.File;
using FileMode = System.IO.FileMode;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using VectorMaker.Utility;
using System.Xml;
using System.Windows.Xps.Packaging;
using System.Windows.Xps;
using System.IO.Packaging;
using System.IO;
using System.Printing;
using VectorMaker.Intefaces;
using System.Xml.Linq;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using VectorMaker.Models;

namespace VectorMaker.ViewModel
{
    internal class DrawingDocumentViewModelBase : DocumentViewModelBase
    {
        #region Fields
        /*Drawing file data*/
        private Canvas m_mainCanvas;
        private ObservableCollection<LayerItemViewModel> m_layers; 
        private int m_layersCount = 1;
        private DrawingDocumentData m_data = new DrawingDocumentData();

        /*Others*/
        protected IMainWindowViewModel m_interfaceMainWindowVM;
        protected bool m_isFileToBeLoaded = false;

        /*BaseClass*/
        private FileType[] m_filters = new FileType[] { FileType.SVG, FileType.PNG, FileType.PDF, FileType.BMP, FileType.JPEG, FileType.TIFF };
        private string m_defaultExtension = "xaml";
        #endregion

        #region Properties
        /*Drawing Data*/
        public Canvas MainCanvas
        {
            get => m_mainCanvas;
            set
            {
                m_mainCanvas = value;
                if (m_isFileToBeLoaded)
                    LoadFile();
                else
                {
                    m_mainCanvas.Children.Add(SelectedLayer.Layer);
                }
            }
        }
        public ObservableCollection<LayerItemViewModel> Layers
        {
            get => m_layers;
            set
            {
                if (m_layers != null)
                    m_layers.CollectionChanged -= LayersCollectionChanged;
                m_layers = value;
                m_layers.CollectionChanged += LayersCollectionChanged;
                OnPropertyChanged(nameof(Layers));
            }
        }
        public int LayersNumber
        {
            get => m_layersCount;
            set
            {
                m_layersCount = value;
                OnPropertyChanged(nameof(LayersNumber));
            }
        }
        public DrawingDocumentData Data { get => m_data; }
        public virtual LayerItemViewModel SelectedLayer { get; set; }
        public virtual bool IsMetadataToSave { get; set; } = true;

        /*Base class and reference*/
        public static Configuration AppConfiguration => Configuration.Instance;
        protected override FileType[] Filters { get => m_filters; set => m_filters = value; }
        protected override string DefaultExtension { get => m_defaultExtension; set => m_defaultExtension = value; }
        #endregion

        public DrawingDocumentViewModelBase(IMainWindowViewModel mainWindowViewModel, double width = 250, double height = 250)
        {
            m_interfaceMainWindowVM = mainWindowViewModel;
            m_data.Width = width;
            m_data.Height = height;
        }

        #region Events
        private void LayersCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                m_layersCount++;
        }
        #endregion

        #region Methods
        protected override bool SaveFile()
        {
            if (!IsSaved)
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    bool result = OpenSaveDialog("Microsoft XAML File (*.xaml) | *.xaml", "untilted.xaml", out string filePath);
                    if (result == true)
                    {
                        FilePath = filePath;
                        bool saved2 = SaveStreamToXAML(FilePath);
                        IsSaved = saved2;
                    }
                    return IsSaved;
                }
                bool saved = SaveStreamToXAML(FilePath);
                if (saved)
                    IsSaved = true;
            }
            return IsSaved;
        }
        protected override void CloseFile()
        {
            if (!IsSaved)
            {
                var result = MessageBox.Show(string.Format("Do you want to save changes " +
                    "for file '{0}'?", FileName), "VectorMaker",
                    MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Cancel)
                    return;

                if (result == MessageBoxResult.Yes)
                {
                    if (SaveFile())
                        m_interfaceMainWindowVM.Close(this);
                    return;
                }
            }
            m_interfaceMainWindowVM.Close(this);

        }
        protected override void PrintFile()
        {
            PrintDialog printDialog = new PrintDialog();
            bool result = (bool)printDialog.ShowDialog();
            if (result)
            {
                printDialog.PrintVisual(MainCanvas, "VectorMaker print image");
            }
        }
        protected override void SaveFileAsPDF(string fullFilePath)
        {
            PrintTicket printTicket = new PrintTicket();
            printTicket.OutputColor = OutputColor.Color;
            printTicket.PageMediaSize = new PageMediaSize(m_mainCanvas.ActualWidth, m_mainCanvas.ActualHeight);
            printTicket.PageMediaType = PageMediaType.Photographic;
            printTicket.PageBorderless = PageBorderless.Borderless;
            printTicket.PageOrientation = PageOrientation.Portrait;
            printTicket.CopyCount = 1;
            printTicket.OutputQuality = OutputQuality.Photographic;
            try
            {
                MemoryStream memoryStream = new MemoryStream();
                Package package = Package.Open(memoryStream, FileMode.Create);
                XpsDocument xpsDocument = new XpsDocument(package);
                XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                VisualsToXpsDocument visToXps = (VisualsToXpsDocument)xpsWriter.CreateVisualsCollator(printTicket, printTicket);
                visToXps.BeginBatchWrite();
                //visToXps.Write(m_mainCanvas,printTicket);
                visToXps.Write(m_mainCanvas);
                visToXps.EndBatchWrite();
                xpsDocument.Close();
                package.Close();
                var xpsToPDF = PdfSharp.Xps.XpsModel.XpsDocument.Open(memoryStream);
                PdfSharp.Xps.XpsConverter.Convert(xpsToPDF, fullFilePath, 0);
                memoryStream.Dispose();
            }
            catch (PrintDialogException e)
            {
                MessageBox.Show(e.Message);
            }
        }
        protected override void SaveFileAsPNG(string fullFilePath)
        {
            PngBitmapEncoder encoder = new();
            SaveWithSpecialBitmap(encoder, fullFilePath);
        }
        protected override void SaveFileAsBMP(string fullFilePath)
        {
            BmpBitmapEncoder encoder = new();
            SaveWithSpecialBitmap(encoder, fullFilePath);
        }
        protected override void SaveFileAsJPEG(string fullFilePath)
        {
            JpegBitmapEncoder encoder = new();
            SaveWithSpecialBitmap(encoder, fullFilePath);
        }
        protected override void SaveFileAsTIFF(string fullFilePath)
        {
            TiffBitmapEncoder encoder = new();
            SaveWithSpecialBitmap(encoder, fullFilePath);
        }
        private void LoadFile()
        {
            try
            {
                if (Path.GetExtension(FilePath) == ".svg")
                {
                    XDocument document = SVG_XAML_Converter_Lib.SVG_To_XAML.ConvertSVGToXamlCode(FilePath);

                    if (document != null)
                    {
                        object path = XamlReader.Parse(document.ToString());
                        MainCanvas.Children.Add(path as UIElement); //toDo Check if MainCanvas
                        IsSaved = false; //toDo load layers
                        //LoadLayers();
                    }
                }
                else
                {
                    using (FileStream fileStream = File.Open(FilePath, FileMode.Open))
                    {
                        object loadedFile = XamlReader.Load(fileStream);
                        Canvas canvas = loadedFile as Canvas;
                        MainCanvas.Children.Add(loadedFile as UIElement);
                        LoadLayers(canvas);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

        }
        private void LoadLayers(Canvas loadedMainCanvas)
        {
            int i = 1;
            foreach (Canvas canvas in loadedMainCanvas.Children.OfType<Canvas>())
            {
                if ((string)canvas.Tag == "Layer")
                {
                    LayerItemViewModel layer = new(canvas, i, canvas.Name);
                    layer.DeleteAction = (layer) =>
                    {
                        Layers.Remove(layer);
                        SelectedLayer = Layers.Last();
                        loadedMainCanvas.Children.Remove(layer.Layer);
                    };
                    Layers.Add(layer);
                    i++;
                }
            }
            LayersNumber = Layers.Count;
            if (Layers.Count != 0)
                SelectedLayer = Layers[0];
        }
        private bool OpenSaveDialog(string filter, string fileName, out string filePath)
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            saveFileDialog.Filter = filter;
            saveFileDialog.FileName = fileName;
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                filePath = saveFileDialog.FileName;
                return true;
            }
            filePath = "";
            return false;
        }
        private void SaveWithSpecialBitmap(BitmapEncoder bitmapEncoder, string filePath)
        {
            RenderTargetBitmap renderBitmap = new((int)m_mainCanvas.Width, (int)m_mainCanvas.Height, 96d, 96d, PixelFormats.Pbgra32);
            Size size = new Size(m_mainCanvas.Width, m_mainCanvas.Height);
            VisualBrush sourceBrush = new VisualBrush(m_mainCanvas);
            sourceBrush.Stretch = Stretch.None;
            DrawingVisual drawingVisual = new();
            DrawingContext drawingContext = drawingVisual.RenderOpen();
            using (drawingContext)
            {
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(size));
            }
            renderBitmap.Render(m_mainCanvas);
            using (FileStream fileStream = new(filePath, FileMode.Create))
            {
                bitmapEncoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                bitmapEncoder.Save(fileStream);
            }
        }
        private bool SaveStreamToXAML(string filePath)
        {
            try
            {
                using (FileStream fileStream = File.OpenWrite(filePath))
                {
                    fileStream.SetLength(0);
                    XmlTextWriter xmlTextWriter = new(fileStream, System.Text.Encoding.UTF8);
                    xmlTextWriter.Formatting = Formatting.Indented;
                    if(IsMetadataToSave)
                        xmlTextWriter.WriteComment(Configuration.Instance.GetMetadataFromConfig());
                    XamlWriter.Save(m_mainCanvas, xmlTextWriter);
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }
        }
        #endregion
    }
}
