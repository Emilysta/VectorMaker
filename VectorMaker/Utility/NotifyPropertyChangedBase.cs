using System.ComponentModel;

namespace VectorMaker.Utility
{
    /// <summary>
    /// Base for all classes that need to implement <see cref="INotifyPropertyChanged"/> interface
    /// </summary>
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Property Changeg event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Method that invokes PropertyChanged event on property.
        /// </summary>
        /// <param name="propertyName">Name of property that changed and should inform app</param>
        public void OnPropertyChanged(string propertyName)
        {
            if(PropertyChanged!=null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Method that invokes PropertyChanged event on all properties in class.
        /// </summary>
        public void OnAllPropertiesChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
    }
}
