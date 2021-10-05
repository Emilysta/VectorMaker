using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VectorMaker
{
    public class Observable<T> : INotifyPropertyChanged
    {
        private T m_observableObject;

        public T ObserwableObject
        {
            get
            {
                return m_observableObject;
            }
            set
            {
                m_observableObject = value;
                OnPropertyChanged();
            }
        }

        public Observable(PropertyChangedEventHandler handler) : base()
        {
            PropertyChanged += handler;
        }

        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string property = "")
        {
            OnPropertyChanged(new PropertyChangedEventArgs(property));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
