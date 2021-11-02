using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;
using VectorMaker.Drawables;
using VectorMaker.Utility;

namespace VectorMaker.Model
{
    class DrawingCanvasModel : NotifyPropertyChangedBase
    {
        private bool m_isSaved = true;
        private string m_filePath;
        private ObservableCollection<ResizingAdorner> m_selectedObjects = new ObservableCollection<ResizingAdorner>();

        public bool IsSaved
        {
            get => m_isSaved;
            set
            {
                m_isSaved = value;
                OnPropertyChanged("IsSaved");
            }
        }

        public string FilePath
        {
            get => m_filePath;
            set
            {
                m_filePath = value;
                OnPropertyChanged("FileName");
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

        public bool IsOneObjectSelected
        {
            get
            {
                return SelectedObjects.Count == 1 ? true : false; 
            }
        }

        public ObservableCollection<ResizingAdorner> SelectedObjects
        {
            get => m_selectedObjects;
            set{
                m_selectedObjects = value;
                OnPropertyChanged("SelectedObjects");
            }
        }

    }

}
