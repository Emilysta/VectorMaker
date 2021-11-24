using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.Intefaces;
using VectorMaker.Utility;


namespace VectorMaker.ViewModel
{

    public enum FileType
    {
        [EnumStringValue("Extensible Application Markup Language (*.xaml)|*.xaml")]
        XAML,
        [EnumStringValue("Scalable Vector Graphics (*.svg)|*.svg")]
        SVG,
        [EnumStringValue("Portable Network Graphics (*.png)|*.png")]
        PNG,
        [EnumStringValue("Portable Document Format (*.pdf)|*.pdf")]
        PDF,
        [EnumStringValue("Bitmap (*.bmp)|*.bmp")]
        BMP,
        [EnumStringValue("JPEG File (*.jpeg)|*.jpeg")]
        JPEG,
        [EnumStringValue("Tagged Image File Format (*.tiff)|*.tiff")]
        TIFF
    }

    abstract class DocumentViewModelBase : NotifyPropertyChangedBase, IDocumentViewModel
    {
        #region Fields
        private string m_title = null;
        private string m_contentId = null;
        private bool m_isSelected = false;
        private bool m_isActive = false;
        private string m_filePath;
        private bool m_isSaved = true;
        #endregion fields

        #region Properties

        public string Title
        {
            get => m_title;
            set
            {
                if (m_title != value)
                {
                    m_title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public string ContentId
        {
            get => m_contentId;
            protected set
            {
                if (m_contentId != value)
                {
                    m_contentId = value;
                    OnPropertyChanged(nameof(ContentId));
                }
            }
        }

        public bool IsSelected
        {
            get => m_isSelected;
            set
            {
                if (m_isSelected != value)
                {
                    m_isSelected = value;
                    OnPropertyChanged(nameof(IsSelected));
                }
            }
        }

        public bool IsActive
        {
            get => m_isActive;
            set
            {
                if (m_isActive != value)
                {
                    m_isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        public string FileName
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath))
                {
                    return IsSaved ? "untitled.xaml" : "untitled.xaml*";
                }
                return IsSaved ? System.IO.Path.GetFileName(FilePath) : (System.IO.Path.GetFileName(FilePath) + "*");
            }
        }

        public string FilePath
        {
            get => m_filePath;
            set
            {
                m_filePath = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        public bool IsSaved
        {
            get => m_isSaved;
            set
            {
                m_isSaved = value;
                OnPropertyChanged(nameof(IsSaved));
                OnPropertyChanged(nameof(FileName));
            }
        }

        protected abstract FileType[] Filters { get; set; }
        protected abstract string DefaultExtension { get; set; }

        #endregion Properties

        #region DocumentCommands
        public ICommand CloseCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }

        #endregion

        #region Constructors
        public DocumentViewModelBase()
        {
            SetCommands();
        }
        #endregion constructors

        #region Methods
        private void SetCommands()
        {
            CloseCommand = new CommandBase((obj) => CloseFile());
            SaveCommand = new CommandBase((obj) => SaveFile());
            SaveAsCommand = new CommandBase((obj) => SaveAsFile(obj as string));
        }

        protected abstract bool SaveFile();
        protected abstract void CloseFile();
        protected virtual void SaveAsFile(string type)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = CreateFilterFromArray(type);
            saveFileDialog.FileName = "untilted";
            saveFileDialog.DefaultExt = "png";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            bool result = (bool)saveFileDialog.ShowDialog();
            if (result)
            {
                RunSpecialSave(saveFileDialog.FileName);
            }
        }
        protected virtual void SaveFileAsPNG(string fullFileName) { MessageBox.Show("File format is not supported"); }
        protected virtual void SaveFileAsBMP(string fullFileName) { MessageBox.Show("File format is not supported"); }
        protected virtual void SaveFileAsPDF(string fullFileName) { MessageBox.Show("File format is not supported"); }
        protected virtual void SaveFileAsJPEG(string fullFileName) { MessageBox.Show("File format is not supported"); }
        protected virtual void SaveFileAsSVG(string fullFileName) { MessageBox.Show("File format is not supported"); }
        protected virtual void SaveFileAsTIFF(string fullFileName) { MessageBox.Show("File format is not supported"); }

        private void RunSpecialSave(string fullFileName)
        {
            string extension = Path.GetExtension(fullFileName);
            switch (extension)
            {
                case ".png":
                    {
                        SaveFileAsPNG(fullFileName);
                        break;
                    }
                case ".bmp":
                    {
                        SaveFileAsBMP(fullFileName);
                        break;
                    }
                case ".pdf":
                    {
                        SaveFileAsPDF(fullFileName);
                        break;
                    }
                case ".jpeg":
                    {
                        SaveFileAsJPEG(fullFileName);
                        break;
                    }
                case ".svg":
                    {
                        SaveFileAsSVG(fullFileName);
                        break;
                    }
                case ".tiff":
                    {
                        SaveFileAsTIFF(fullFileName);
                        break;
                    }
            }
        }

        private string CreateFilterFromArray(string firstType)
        {
            string filters = "";
            bool conversion = Enum.TryParse(firstType.ToUpper(), out FileType firstFileType);
            if (conversion)
            {
                filters += firstFileType.GetStringValueForEnum();
                filters += "|";
            }
            foreach (FileType fileType in Filters)
            {
                if (fileType != firstFileType)
                {
                    filters += fileType.GetStringValueForEnum();
                    filters += "|";
                }
            }
            filters = filters.TrimEnd('|');

            if (string.IsNullOrEmpty(filters))
            {
                return FileType.PNG.GetStringValueForEnum();
            }
            return filters;
        }
        #endregion
    }
}
