using System;

namespace VectorMaker.Utility
{
    class UIPropertyChanged
    {
        public Action PropertyChanged;

        private static UIPropertyChanged m_instance;
        public static UIPropertyChanged Instance
        {
            get
            {
                if (m_instance == null)
                    m_instance = new UIPropertyChanged();
                return m_instance;
            }
        }

        public void InvokePropertyChanged()
        {
            PropertyChanged?.Invoke();
        }
    }
}
