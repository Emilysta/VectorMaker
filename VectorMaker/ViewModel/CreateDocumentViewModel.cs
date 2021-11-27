using System.Windows;
using VectorMaker.Utility;

namespace VectorMaker.ViewModel
{
    internal class CreateDocumentViewModel : NotifyPropertyChangedBase
    {
        #region Fields
        private Visibility m_panelVisibility;
        #endregion

        #region Properties
        public Visibility PanelVisibility
        {
            get => m_panelVisibility;
            set
            {
                m_panelVisibility = value;
                OnPropertyChanged(nameof(PanelVisibility));
            }
        }
        #endregion
    }
}
