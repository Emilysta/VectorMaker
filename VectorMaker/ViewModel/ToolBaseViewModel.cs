
using System;
using System.ComponentModel;
using VectorMaker.Intefaces;

namespace VectorMaker.ViewModel
{
    internal abstract class ToolBaseViewModel : INotifyPropertyChanged
    {
        protected IMainWindowViewModel m_interfaceMainWindowVM;
        public event PropertyChangedEventHandler PropertyChanged;
        protected abstract string m_title { get; set; }
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
            get => m_title;
        }

        public ToolBaseViewModel(IMainWindowViewModel mainWindowViewModel)
        {
            m_interfaceMainWindowVM = mainWindowViewModel;
            m_interfaceMainWindowVM.ActiveCanvasChanged += OnActiveCanvasChanged;
        }

        public abstract void OnActiveCanvasChanged(object sender, EventArgs e);

        public void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
