using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.Intefaces;
using VectorMaker.Utility;
using VectorMaker.Views;
using BootIcon = MahApps.Metro.IconPacks.PackIconBootstrapIconsKind;

namespace VectorMaker.ViewModel
{
    internal class CreateDocumentViewModel : NotifyPropertyChangedBase
    {
        #region Fields
        private ObservableCollection<DocumentSizeItem> m_sizeslist;
        private CreateDocumentView m_window;
        private int m_selectedSizeIndex = 0;
        private double m_width;
        private double m_height;
        private bool m_isVertical = true;
        #endregion

        #region Properties
        public int SelectedSizeIndex
        {
            get => m_selectedSizeIndex;
            set
            {
                m_selectedSizeIndex = value;
                OnPropertyChanged(nameof(SelectedSizeIndex));
                SelectedItemChanged();
            }
        }

        public double Width
        {
            get => m_width;
            set
            {
                m_width = value;
                OnPropertyChanged(nameof(Width));
            }
        }

        public double Height
        {
            get => m_height;
            set
            {
                m_height = value;
                OnPropertyChanged(nameof(Height));
            }
        }

        public bool IsVertical
        {
            get => m_isVertical;
            set
            {
                m_isVertical = value;
                OnPropertyChanged(nameof(IsVertical));
            }
        }

        public ObservableCollection<DocumentSizeItem> SizesList { get => m_sizeslist; }

        public bool IsFileToBeCreated =
            false;
        #endregion

        #region Commands
        public ICommand CloseCommand { get; set; }
        public ICommand CreateCommand { get; set; }
        #endregion

        #region Constructors
        public CreateDocumentViewModel()
        {
            CreateSizesList();
            SetCommand();
            m_window = new CreateDocumentView();
            m_window.Owner = Application.Current.MainWindow;
            m_window.DataContext = this;
            m_window.ShowDialog();
        }
        #endregion

        #region Methods
        private void CreateSizesList()
        {
            m_sizeslist = new ObservableCollection<DocumentSizeItem>();
            m_sizeslist.Add(new DocumentSizeItem(794, 1123, BootIcon.File, "A4", 96));
            m_sizeslist.Add(new DocumentSizeItem(1123, 1587, BootIcon.File, "A3", 96));
            m_sizeslist.Add(new DocumentSizeItem(559, 794, BootIcon.File, "A5", 96));
            m_sizeslist.Add(new DocumentSizeItem(250, 250, BootIcon.Square, "Square", 96));
            m_sizeslist.Add(new DocumentSizeItem(1024, 1024, BootIcon.Square, "Square", 96));
            m_sizeslist.Add(new DocumentSizeItem(2048, 2048, BootIcon.File, "Square", 96));
        }

        private void SetCommand()
        {
            CloseCommand = new CommandBase((_) => DiscardCreateDocument());
            CreateCommand = new CommandBase((_) => CreateDocument());
        }

        private void CreateDocument()
        {
            IsFileToBeCreated = true;
            m_window.Close();
        }

        private void DiscardCreateDocument()
        {
            IsFileToBeCreated = false;
            m_window.Close();
        }

        private void SelectedItemChanged()
        {
            if (SelectedSizeIndex >= 0)
            {
                DocumentSizeItem documentSizeItem = m_sizeslist[SelectedSizeIndex];
                Width = documentSizeItem.Width;
                Height = documentSizeItem.Height;
                IsVertical = documentSizeItem.IsVertical;
            }
        }
        #endregion
    }
}
