using System;
using System.ComponentModel;
using System.Windows.Input;
using VectorMaker.Commands;
using VectorMaker.Intefaces;
using VectorMaker.Utility;

namespace VectorMaker.ViewModel
{
    internal abstract class ToolBaseViewModel : NotifyPropertyChangedBase
    {
        #region Fields
        protected IMainWindowViewModel m_interfaceMainWindowVM;
        #endregion

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
            get => m_title;
        }

        protected abstract string m_title { get; set; }
        #endregion

        #region Commands
        public ICommand CloseToolCommand{ get; set; }
        #endregion

        public ToolBaseViewModel(IMainWindowViewModel mainWindowViewModel)
        {
            m_interfaceMainWindowVM = mainWindowViewModel;
            m_interfaceMainWindowVM.ActiveCanvasChanged += OnActiveCanvasChanged;
            CloseToolCommand = new CommandBase((obj) => CloseTool());
        }

        public abstract void OnActiveCanvasChanged(object sender, EventArgs e);

        #region Methods
        private void CloseTool()
        {
            m_interfaceMainWindowVM.CloseTool(this);
        }
        #endregion
    }
}
