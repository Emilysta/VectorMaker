using System;
using System.Windows;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.Models;
using VectorMaker.Utility;
using VectorMaker.Views;

namespace VectorMaker.ViewModel
{
    internal class MetaFileSettingsViewModel : NotifyPropertyChangedBase
    {
        #region Fields
        private MetaFileSettingsView m_window;
        private DrawingDocumentData m_data;
        private bool m_saveMetadata = true;
        #endregion

        #region Properties
        public DrawingDocumentData Data { get { return m_data; } }
        public bool SaveMetadata
        {
            get { return m_saveMetadata; }
            set
            {
                m_saveMetadata = value;
                OnPropertyChanged(nameof(SaveMetadata));
            }
        }
        #endregion

        #region Commands
        public ICommand CloseSettingsWindowCommand { get; set; }
        public ICommand WindowMouseLeftButtonDownCommand { get; set; }
        public ICommand OkMetadataCommand { get; set; }
        public ICommand ResetMetadataCommand { get; set; }

        #endregion

        #region Constructors
        public MetaFileSettingsViewModel(DrawingDocumentData data)
        {
            SetCommand();
            m_data = data;
            m_window = new MetaFileSettingsView();
            m_window.Owner = Application.Current.MainWindow;
            m_window.DataContext = this;
            m_window.ShowDialog();

        }
        #endregion

        #region Methods
        private void SetCommand()
        {
            WindowMouseLeftButtonDownCommand = new CommandBase((obj) => DragSettingsWindow(obj as MouseButtonEventArgs));
            CloseSettingsWindowCommand = new CommandBase((_) => CloseSettingsWindow());
            OkMetadataCommand = new CommandBase((_) => OkMetadata());
            ResetMetadataCommand = new CommandBase((_) => ResetMetadata());
        }

        private void DragSettingsWindow(MouseButtonEventArgs e)
        {
            try
            {
                if (m_window.IsMouseOver)
                {
                    m_window.DragMove();
                    e.Handled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                //toDo check why exception was before if
            }
        }

        private void CloseSettingsWindow()
        {
            m_window.Close();
        }

        private void OkMetadata()
        {
            m_window.Close();
        }

        private void ResetMetadata()
        {
            Data.Description = "";
            Data.Title = "";
            SaveMetadata = true;
        }
        #endregion
    }
}
